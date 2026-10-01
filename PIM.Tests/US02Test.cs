using System.Reflection;
using Npgsql;
using NUnit.Framework;
using PIM.Services;
using PIM.Data;

namespace PIM.Tests;

/// <summary>
/// Integration tests for User Story 02 – Product Deletion and Active/Inactive Status.
///
/// Acceptance criteria covered:
///   AC-1  A product variant can be permanently deleted from the database along with its attribute values.
///   AC-2  A product entry is automatically removed when its last variant is deleted.
///   AC-3  An active variant cannot be deleted — it must be set inactive first.
///   AC-4  A product variant can be set active or inactive.
///
/// [OneTimeSetUp]     seeds the Supabase test database with setup_db.sql + data.sql.
/// [OneTimeTearDown]  drops all PIM tables so the test database is clean for next run.
/// [SetUp] / [TearDown] wrap each test in a transaction that is rolled back, keeping tests isolated.
/// </summary>
[TestFixture]
public class US02Test
{
    // ── Constants ─────────────────────────────────────────────────────────────

    private const string TestCategoryName  = "GPU";
    private const string TestAttributeName = "VRAM";

    // ── Per-fixture state (resolved once in OneTimeSetUp) ─────────────────────

    private static int _fixtureCategoryId;
    private static int _fixtureAttributeId;

    // ── Per-test state ────────────────────────────────────────────────────────

    private NpgsqlConnection            _connection   = null!;
    private NpgsqlTransaction           _transaction  = null!;
    private CreateProductService        _createService = null!;
    private DeleteProductVariantService _deleteService = null!;
    private SetProductActiveService     _activeService = null!;

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
        _connection    = DatabaseConnection.GetConnection();
        _connection.Open();
        _transaction   = _connection.BeginTransaction();
        _createService = new CreateProductService();
        _deleteService = new DeleteProductVariantService();
        _activeService = new SetProductActiveService();

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
    // AC-1 · Variant Deletion
    // ─────────────────────────────────────────────────────────────────────────

    /// <summary>
    /// AC-1 – Deleting an inactive variant must remove it from pim_product_variants.
    /// </summary>
    [Test]
    public void DeleteVariant_InactiveVariant_RemovesVariantRow()
    {
        // Arrange: create a product and set it inactive so it can be deleted
        string sku = $"SKU-{Guid.NewGuid()}";
        _createService.CreateProduct($"DeleteTest {Guid.NewGuid()}", _testCategoryId, sku, 100m, 90m, 60m,
            new Dictionary<int, string>());

        int variantId = QuerySingleInt("SELECT id FROM pim_product_variants WHERE sku = @v", sku);
        _activeService.SetInactive(variantId);

        // Act
        bool result = _deleteService.DeleteVariant(variantId);

        // Assert
        Assert.That(result, Is.True, "DeleteVariant should return true when variant is found and deleted.");

        int count = QuerySingleInt("SELECT COUNT(*)::int FROM pim_product_variants WHERE id = @v", variantId);
        Assert.That(count, Is.EqualTo(0), "Variant row should no longer exist after deletion.");
    }

    /// <summary>
    /// AC-1 – Deleting a variant must also cascade and remove its attribute values.
    /// </summary>
    [Test]
    public void DeleteVariant_RemovesAttributeValues()
    {
        // Arrange
        string sku = $"SKU-{Guid.NewGuid()}";
        _createService.CreateProduct($"AttrDeleteTest {Guid.NewGuid()}", _testCategoryId, sku, 200m, 180m, 120m,
            new Dictionary<int, string> { { _testAttributeId, "8" } });

        int variantId = QuerySingleInt("SELECT id FROM pim_product_variants WHERE sku = @v", sku);
        _activeService.SetInactive(variantId);

        // Act
        _deleteService.DeleteVariant(variantId);

        // Assert
        int attrCount = QuerySingleInt(
            "SELECT COUNT(*)::int FROM pim_variant_attribute_values WHERE variant_id = @v", variantId);
        Assert.That(attrCount, Is.EqualTo(0), "Attribute values should be removed when the variant is deleted.");
    }

    /// <summary>
    /// AC-1 – Deleting a non-existent variant must return false without throwing.
    /// </summary>
    [Test]
    public void DeleteVariant_NonExistentVariant_ReturnsFalse()
    {
        // Act
        bool result = _deleteService.DeleteVariant(999999);

        // Assert
        Assert.That(result, Is.False, "DeleteVariant should return false when no variant with that id exists.");
    }

    // ─────────────────────────────────────────────────────────────────────────
    // AC-2 · Orphan Product Cleanup
    // ─────────────────────────────────────────────────────────────────────────

    /// <summary>
    /// AC-2 – When the last variant of a product is deleted, the parent product
    /// row must also be removed from pim_products.
    /// </summary>
    [Test]
    public void DeleteVariant_LastVariant_RemovesParentProduct()
    {
        // Arrange: create a product with a single variant
        string productName = $"OrphanTest {Guid.NewGuid()}";
        string sku         = $"SKU-{Guid.NewGuid()}";
        _createService.CreateProduct(productName, _testCategoryId, sku, 150m, 130m, 90m,
            new Dictionary<int, string>());

        int productId = QuerySingleInt("SELECT id FROM pim_products WHERE name = @v", productName);
        int variantId = QuerySingleInt("SELECT id FROM pim_product_variants WHERE sku = @v", sku);
        _activeService.SetInactive(variantId);

        // Act
        _deleteService.DeleteVariant(variantId);

        // Assert
        int productCount = QuerySingleInt("SELECT COUNT(*)::int FROM pim_products WHERE id = @v", productId);
        Assert.That(productCount, Is.EqualTo(0),
            "Parent product should be removed when its last variant is deleted.");
    }

    /// <summary>
    /// AC-2 – When one of two variants is deleted, the parent product must remain.
    /// </summary>
    [Test]
    public void DeleteVariant_OneOfTwoVariants_KeepsParentProduct()
    {
        // Arrange: create a product with two variants
        string productName = $"MultiVariant {Guid.NewGuid()}";
        string sku1        = $"SKU-A-{Guid.NewGuid()}";
        string sku2        = $"SKU-B-{Guid.NewGuid()}";

        _createService.CreateProduct(productName, _testCategoryId, sku1, 100m, 90m, 60m,
            new Dictionary<int, string>());
        _createService.CreateProduct(productName, _testCategoryId, sku2, 110m, 95m, 65m,
            new Dictionary<int, string>());

        int productId  = QuerySingleInt("SELECT id FROM pim_products WHERE name = @v", productName);
        int variantId1 = QuerySingleInt("SELECT id FROM pim_product_variants WHERE sku = @v", sku1);
        _activeService.SetInactive(variantId1);

        // Act
        _deleteService.DeleteVariant(variantId1);

        // Assert
        int productCount = QuerySingleInt("SELECT COUNT(*)::int FROM pim_products WHERE id = @v", productId);
        Assert.That(productCount, Is.EqualTo(1),
            "Parent product should remain when at least one variant still exists.");
    }

    // ─────────────────────────────────────────────────────────────────────────
    // AC-3 · Active Guard
    // ─────────────────────────────────────────────────────────────────────────

    /// <summary>
    /// AC-3 – Attempting to delete an active variant must return false and leave
    /// the variant in the database untouched.
    /// </summary>
    [Test]
    public void DeleteVariant_ActiveVariant_ReturnsFalseAndDoesNotDelete()
    {
        // Arrange: create a product — variants default to active
        string sku = $"SKU-{Guid.NewGuid()}";
        _createService.CreateProduct($"ActiveGuard {Guid.NewGuid()}", _testCategoryId, sku, 100m, 90m, 60m,
            new Dictionary<int, string>());

        int variantId = QuerySingleInt("SELECT id FROM pim_product_variants WHERE sku = @v", sku);

        // Act: attempt to delete without setting inactive first
        bool result = _deleteService.DeleteVariant(variantId);

        // Assert
        Assert.That(result, Is.False, "DeleteVariant should return false for an active variant.");

        int count = QuerySingleInt("SELECT COUNT(*)::int FROM pim_product_variants WHERE id = @v", variantId);
        Assert.That(count, Is.EqualTo(1), "Active variant should still exist after failed delete attempt.");
    }

    // ─────────────────────────────────────────────────────────────────────────
    // AC-4 · Active / Inactive Status
    // ─────────────────────────────────────────────────────────────────────────

    /// <summary>
    /// AC-4 – SetInactive must set isactive to false in the database.
    /// </summary>
    [Test]
    public void SetInactive_SetsIsActiveFalse()
    {
        // Arrange
        string sku = $"SKU-{Guid.NewGuid()}";
        _createService.CreateProduct($"InactiveTest {Guid.NewGuid()}", _testCategoryId, sku, 100m, 90m, 60m,
            new Dictionary<int, string>());

        int variantId = QuerySingleInt("SELECT id FROM pim_product_variants WHERE sku = @v", sku);

        // Act
        bool result = _activeService.SetInactive(variantId);

        // Assert
        Assert.That(result, Is.True, "SetInactive should return true when variant exists.");

        bool isActive = QuerySingleBool("SELECT isactive FROM pim_product_variants WHERE id = @v", variantId);
        Assert.That(isActive, Is.False, "isactive should be false after SetInactive.");
    }

    /// <summary>
    /// AC-4 – SetActive must set isactive to true in the database.
    /// </summary>
    [Test]
    public void SetActive_SetsIsActiveTrue()
    {
        // Arrange: create and then deactivate
        string sku = $"SKU-{Guid.NewGuid()}";
        _createService.CreateProduct($"ActiveTest {Guid.NewGuid()}", _testCategoryId, sku, 100m, 90m, 60m,
            new Dictionary<int, string>());

        int variantId = QuerySingleInt("SELECT id FROM pim_product_variants WHERE sku = @v", sku);
        _activeService.SetInactive(variantId);

        // Act
        bool result = _activeService.SetActive(variantId);

        // Assert
        Assert.That(result, Is.True, "SetActive should return true when variant exists.");

        bool isActive = QuerySingleBool("SELECT isactive FROM pim_product_variants WHERE id = @v", variantId);
        Assert.That(isActive, Is.True, "isactive should be true after SetActive.");
    }

    /// <summary>
    /// AC-4 – SetInactive on a non-existent variant must return false.
    /// </summary>
    [Test]
    public void SetInactive_NonExistentVariant_ReturnsFalse()
    {
        bool result = _activeService.SetInactive(999999);
        Assert.That(result, Is.False, "SetInactive should return false when no variant with that id exists.");
    }

    /// <summary>
    /// AC-4 – SetActive on a non-existent variant must return false.
    /// </summary>
    [Test]
    public void SetActive_NonExistentVariant_ReturnsFalse()
    {
        bool result = _activeService.SetActive(999999);
        Assert.That(result, Is.False, "SetActive should return false when no variant with that id exists.");
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

    private bool QuerySingleBool(string sql, object? paramValue)
    {
        using NpgsqlCommand cmd = new NpgsqlCommand(sql, _connection, _transaction);
        if (paramValue != null) cmd.Parameters.AddWithValue("v", paramValue);
        return Convert.ToBoolean(cmd.ExecuteScalar());
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