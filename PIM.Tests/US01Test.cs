using System.Diagnostics;
using System.Reflection;
using Npgsql;
using NUnit.Framework;
using PIM.Services;

namespace PIM.Tests;

/// <summary>
/// Integration tests for User Story 01 – Product Creation.
///
/// Acceptance criteria covered:
///   AC-1  Ability to create products with specific names, prices and categories.
///   AC-2  Have a functioning database, storing all the information of our products.
///   AC-3  Have product categories set up.
///
/// [OneTimeSetUp]     seeds the Supabase test database with setup_db.sql + data.sql.
/// [OneTimeTearDown]  drops all PIM tables so the test database is clean for next run.
/// [SetUp] / [TearDown] wrap each test in a transaction that is rolled back, keeping tests isolated.
/// </summary>
[TestFixture]
public class US01Test
{
    // ── Constants ─────────────────────────────────────────────────────────────

    private const string TestCategoryName  = "GPU";
    private const string TestAttributeName = "VRAM";

    // ── Per-fixture state (resolved once in OneTimeSetUp) ─────────────────────

    private static int _fixtureCategoryId;
    private static int _fixtureAttributeId;

    // ── Per-test state ────────────────────────────────────────────────────────

    private NpgsqlConnection     _connection  = null!;
    private NpgsqlTransaction    _transaction = null!;
    private CreateProductService _service     = null!;

    private int _testCategoryId;
    private int _testAttributeId;

    // ─────────────────────────────────────────────────────────────────────────
    // One-time: seed the Supabase test database
    // ─────────────────────────────────────────────────────────────────────────

    [OneTimeSetUp]
    public void FixtureSetUp()
    {
        // Point the service at the test database for the entire test run.
        DatabaseConnection.ConnectionString = TestDatabaseConnection.ConnectionString;

        // Find the SQL files next to the test assembly and run them.
        string assemblyDir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)!;
        RunSqlFile(Path.Combine(assemblyDir, "setup_db.sql"));
        RunSqlFile(Path.Combine(assemblyDir, "data.sql"));

        // Resolve the ids we reuse across all tests.
        using NpgsqlConnection bootstrap = new NpgsqlConnection(TestDatabaseConnection.ConnectionString);
        bootstrap.Open();
        _fixtureCategoryId  = ScalarInt(bootstrap, "SELECT id FROM pim_categories WHERE name = @v",           TestCategoryName);
        _fixtureAttributeId = ScalarInt(bootstrap, "SELECT id FROM pim_attributes  WHERE attribute_name = @v", TestAttributeName);
        bootstrap.Close();
    }

    [OneTimeTearDown]
    public void FixtureTearDown()
    {
        // Drop all PIM tables so the test database is clean for the next run.
        // CASCADE handles foreign key dependencies automatically.
        using NpgsqlConnection conn = new NpgsqlConnection(TestDatabaseConnection.ConnectionString);
        conn.Open();
        using NpgsqlCommand cmd = new NpgsqlCommand(@"
            DROP TABLE IF EXISTS pim_variant_attribute_values CASCADE;
            DROP TABLE IF EXISTS pim_product_variants         CASCADE;
            DROP TABLE IF EXISTS pim_products                 CASCADE;
            DROP TABLE IF EXISTS pim_category_attributes      CASCADE;
            DROP TABLE IF EXISTS pim_attributes               CASCADE;
            DROP TABLE IF EXISTS pim_categories               CASCADE;
            DROP TABLE IF EXISTS pim_units                    CASCADE;",
            conn);
        cmd.ExecuteNonQuery();
        conn.Close();
    }

    // ─────────────────────────────────────────────────────────────────────────
    // Per-test: open connection + transaction, roll back after
    // ─────────────────────────────────────────────────────────────────────────

    [SetUp]
    public void SetUp()
    {
        _connection  = DatabaseConnection.GetConnection();
        _connection.Open();
        _transaction = _connection.BeginTransaction();
        _service     = new CreateProductService();

        _testCategoryId  = _fixtureCategoryId;
        _testAttributeId = _fixtureAttributeId;
    }

    [TearDown]
    public void TearDown()
    {
        _transaction.Rollback();
        _transaction.Dispose();
        _connection.Close();
        _connection.Dispose();
    }

    // ─────────────────────────────────────────────────────────────────────────
    // AC-3 · Category Setup
    // ─────────────────────────────────────────────────────────────────────────

    /// <summary>
    /// AC-3 – The categories table must contain at least one row, confirming
    /// the category hierarchy has been set up in the database.
    /// </summary>
    [Test]
    public void Database_HasCategories_AfterSetup()
    {
        int count = QuerySingleInt("SELECT COUNT(*)::int FROM pim_categories", null);
        Assert.That(count, Is.GreaterThan(0),
            "pim_categories is empty – setup_db.sql + data.sql may not have run.");
    }

    /// <summary>
    /// AC-3 – The GPU category must resolve to a valid positive integer id.
    /// </summary>
    [Test]
    public void Category_GPU_ExistsInDatabase()
    {
        Assert.That(_testCategoryId, Is.GreaterThan(0),
            $"Category '{TestCategoryName}' was not found in pim_categories.");
    }

    /// <summary>
    /// AC-3 – Categories must have at least one attribute linked via
    /// pim_category_attributes.
    /// </summary>
    [Test]
    public void Category_GPU_HasAtLeastOneAttributeLinked()
    {
        int count = QuerySingleInt(
            "SELECT COUNT(*)::int FROM pim_category_attributes WHERE category_id = @v",
            _testCategoryId);

        Assert.That(count, Is.GreaterThan(0),
            $"No attributes are linked to category '{TestCategoryName}'.");
    }

    // ─────────────────────────────────────────────────────────────────────────
    // AC-1 · Product Creation – happy path
    // ─────────────────────────────────────────────────────────────────────────

    /// <summary>
    /// AC-1 + AC-2 – Creating a product with a specific name, price, and
    /// category must persist a row in pim_products.
    /// </summary>
    [Test]
    public void CreateProduct_WithValidData_InsertsProductRow()
    {
        string uniqueName = $"Test GPU {Guid.NewGuid()}";

        _service.CreateProduct(
            productName:     uniqueName,
            categoryId:      _testCategoryId,
            sku:             $"SKU-{Guid.NewGuid()}",
            price:           499.99m,
            basePrice:       450.00m,
            costPrice:       300.00m,
            attributeValues: new Dictionary<int, string> { { _testAttributeId, "8" } });

        int count = QuerySingleInt(
            "SELECT COUNT(*)::int FROM pim_products WHERE name = @v", uniqueName);

        Assert.That(count, Is.EqualTo(1),
            "Expected exactly one product row after CreateProduct.");
    }

    /// <summary>
    /// AC-1 – The product row must store the exact name provided.
    /// </summary>
    [Test]
    public void CreateProduct_StoresCorrectName()
    {
        string uniqueName = $"NameCheck {Guid.NewGuid()}";

        _service.CreateProduct(
            productName:     uniqueName,
            categoryId:      _testCategoryId,
            sku:             $"SKU-{Guid.NewGuid()}",
            price:           199.00m,
            basePrice:       180.00m,
            costPrice:       100.00m,
            attributeValues: new Dictionary<int, string>());

        string storedName = QuerySingleString(
            "SELECT name FROM pim_products WHERE name = @v", uniqueName);

        Assert.That(storedName, Is.EqualTo(uniqueName));
    }

    /// <summary>
    /// AC-1 – The product variant must store the exact price provided.
    /// </summary>
    [Test]
    public void CreateProduct_StoresCorrectPrice()
    {
        string  uniqueSku     = $"SKU-{Guid.NewGuid()}";
        decimal expectedPrice = 749.95m;

        _service.CreateProduct(
            productName:     $"PriceCheck {Guid.NewGuid()}",
            categoryId:      _testCategoryId,
            sku:             uniqueSku,
            price:           expectedPrice,
            basePrice:       700.00m,
            costPrice:       500.00m,
            attributeValues: new Dictionary<int, string>());

        decimal storedPrice = QuerySingleDecimal(
            "SELECT price FROM pim_product_variants WHERE sku = @v", uniqueSku);

        Assert.That(storedPrice, Is.EqualTo(expectedPrice));
    }

    /// <summary>
    /// AC-1 – The product must be linked to the correct category.
    /// </summary>
    [Test]
    public void CreateProduct_StoresCorrectCategoryId()
    {
        string uniqueName = $"CategoryCheck {Guid.NewGuid()}";

        _service.CreateProduct(
            productName:     uniqueName,
            categoryId:      _testCategoryId,
            sku:             $"SKU-{Guid.NewGuid()}",
            price:           299.00m,
            basePrice:       280.00m,
            costPrice:       150.00m,
            attributeValues: new Dictionary<int, string>());

        int storedCategoryId = QuerySingleInt(
            "SELECT category_id FROM pim_products WHERE name = @v", uniqueName);

        Assert.That(storedCategoryId, Is.EqualTo(_testCategoryId));
    }

    // ─────────────────────────────────────────────────────────────────────────
    // AC-2 · Database Persistence
    // ─────────────────────────────────────────────────────────────────────────

    /// <summary>
    /// AC-2 – A variant row must exist in pim_product_variants after creation.
    /// </summary>
    [Test]
    public void CreateProduct_InsertsVariantRow()
    {
        string uniqueSku = $"SKU-{Guid.NewGuid()}";

        _service.CreateProduct(
            productName:     $"VariantTest {Guid.NewGuid()}",
            categoryId:      _testCategoryId,
            sku:             uniqueSku,
            price:           399.00m,
            basePrice:       370.00m,
            costPrice:       250.00m,
            attributeValues: new Dictionary<int, string>());

        int count = QuerySingleInt(
            "SELECT COUNT(*)::int FROM pim_product_variants WHERE sku = @v", uniqueSku);

        Assert.That(count, Is.EqualTo(1),
            "Expected one variant row in pim_product_variants.");
    }

    /// <summary>
    /// AC-2 – Attribute values must be persisted in pim_variant_attribute_values.
    /// </summary>
    [Test]
    public void CreateProduct_PersistsAttributeValues()
    {
        string uniqueSku = $"SKU-{Guid.NewGuid()}";
        string attrValue = "16";

        _service.CreateProduct(
            productName:     $"AttrTest {Guid.NewGuid()}",
            categoryId:      _testCategoryId,
            sku:             uniqueSku,
            price:           599.00m,
            basePrice:       550.00m,
            costPrice:       400.00m,
            attributeValues: new Dictionary<int, string> { { _testAttributeId, attrValue } });

        int variantId = QuerySingleInt(
            "SELECT id FROM pim_product_variants WHERE sku = @v", uniqueSku);

        string storedValue = QuerySingleString(
            $"SELECT value FROM pim_variant_attribute_values WHERE variant_id = @v AND attribute_id = {_testAttributeId}",
            variantId);

        Assert.That(storedValue, Is.EqualTo(attrValue),
            "Attribute value was not stored correctly.");
    }

    /// <summary>
    /// AC-2 – Calling CreateProduct twice with the same name must reuse the
    /// existing product row rather than duplicating it.
    /// </summary>
    [Test]
    public void CreateProduct_DuplicateName_ReusesExistingProduct()
    {
        string sharedName = $"SharedProduct {Guid.NewGuid()}";

        _service.CreateProduct(sharedName, _testCategoryId, $"SKU-A-{Guid.NewGuid()}", 100m, 90m, 60m,
            new Dictionary<int, string>());
        _service.CreateProduct(sharedName, _testCategoryId, $"SKU-B-{Guid.NewGuid()}", 110m, 95m, 65m,
            new Dictionary<int, string>());

        int productCount = QuerySingleInt(
            "SELECT COUNT(*)::int FROM pim_products WHERE name = @v", sharedName);

        Assert.That(productCount, Is.EqualTo(1),
            "Duplicate product name should reuse the existing product row.");
    }

    /// <summary>
    /// AC-2 – Each call must still create a distinct variant even when the
    /// parent product row is reused.
    /// </summary>
    [Test]
    public void CreateProduct_DuplicateName_CreatesTwoVariants()
    {
        string sharedName = $"MultiVariant {Guid.NewGuid()}";
        string sku1 = $"SKU-V1-{Guid.NewGuid()}";
        string sku2 = $"SKU-V2-{Guid.NewGuid()}";

        _service.CreateProduct(sharedName, _testCategoryId, sku1, 100m, 90m, 60m,
            new Dictionary<int, string>());
        _service.CreateProduct(sharedName, _testCategoryId, sku2, 120m, 100m, 70m,
            new Dictionary<int, string>());

        int productId = QuerySingleInt(
            "SELECT id FROM pim_products WHERE name = @v", sharedName);

        int variantCount = QuerySingleInt(
            "SELECT COUNT(*)::int FROM pim_product_variants WHERE product_id = @v", productId);

        Assert.That(variantCount, Is.EqualTo(2),
            "Two variants should exist under the same product.");
    }

    // ─────────────────────────────────────────────────────────────────────────
    // AC-1 · Edge / Boundary cases
    // ─────────────────────────────────────────────────────────────────────────

    /// <summary>
    /// AC-1 – Base price and cost price must be stored accurately.
    /// </summary>
    [Test]
    public void CreateProduct_StoresBasePriceAndCostPrice()
    {
        string sku = $"SKU-{Guid.NewGuid()}";

        _service.CreateProduct(
            productName:     $"PricesCheck {Guid.NewGuid()}",
            categoryId:      _testCategoryId,
            sku:             sku,
            price:           999.99m,
            basePrice:       899.99m,
            costPrice:       599.99m,
            attributeValues: new Dictionary<int, string>());

        using NpgsqlCommand cmd = new NpgsqlCommand(
            "SELECT base_price, cost_price FROM pim_product_variants WHERE sku = @sku",
            _connection, _transaction);
        cmd.Parameters.AddWithValue("sku", sku);

        using NpgsqlDataReader reader = cmd.ExecuteReader();
        Assert.That(reader.Read(),        Is.True,             "No variant row found.");
        Assert.That(reader.GetDecimal(0), Is.EqualTo(899.99m), "base_price mismatch.");
        Assert.That(reader.GetDecimal(1), Is.EqualTo(599.99m), "cost_price mismatch.");
    }

    /// <summary>
    /// AC-1 – Creating a product with zero attribute values must not throw.
    /// </summary>
    [Test]
    public void CreateProduct_WithNoAttributes_DoesNotThrow()
    {
        Assert.DoesNotThrow(() =>
            _service.CreateProduct(
                productName:     $"NoAttrs {Guid.NewGuid()}",
                categoryId:      _testCategoryId,
                sku:             $"SKU-{Guid.NewGuid()}",
                price:           50.00m,
                basePrice:       45.00m,
                costPrice:       30.00m,
                attributeValues: new Dictionary<int, string>()));
    }

    // ─────────────────────────────────────────────────────────────────────────
    // Query helpers (run on the per-test transactional connection)
    // ─────────────────────────────────────────────────────────────────────────

    private int QuerySingleInt(string sql, object? paramValue)
    {
        using NpgsqlCommand cmd = new NpgsqlCommand(sql, _connection, _transaction);
        if (paramValue != null) cmd.Parameters.AddWithValue("v", paramValue);
        return Convert.ToInt32(cmd.ExecuteScalar());
    }

    private string QuerySingleString(string sql, object? paramValue)
    {
        using NpgsqlCommand cmd = new NpgsqlCommand(sql, _connection, _transaction);
        if (paramValue != null) cmd.Parameters.AddWithValue("v", paramValue);
        return (string)(cmd.ExecuteScalar() ?? throw new InvalidOperationException("No row found."));
    }

    private decimal QuerySingleDecimal(string sql, object? paramValue)
    {
        using NpgsqlCommand cmd = new NpgsqlCommand(sql, _connection, _transaction);
        if (paramValue != null) cmd.Parameters.AddWithValue("v", paramValue);
        return Convert.ToDecimal(cmd.ExecuteScalar());
    }

    // ─────────────────────────────────────────────────────────────────────────
    // SQL file helper
    // ─────────────────────────────────────────────────────────────────────────

    private static void RunSqlFile(string filePath)
    {
        string sql = File.ReadAllText(filePath);

        using NpgsqlConnection conn = new NpgsqlConnection(TestDatabaseConnection.ConnectionString);
        conn.Open();
        using NpgsqlCommand cmd = new NpgsqlCommand(sql, conn);
        cmd.ExecuteNonQuery();
        conn.Close();
    }

    private static int ScalarInt(NpgsqlConnection conn, string sql, object param)
    {
        using NpgsqlCommand cmd = new NpgsqlCommand(sql, conn);
        cmd.Parameters.AddWithValue("v", param);
        return Convert.ToInt32(cmd.ExecuteScalar());
    }
}