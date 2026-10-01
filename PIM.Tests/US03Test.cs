using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using Npgsql;
using NUnit.Framework;
using PIM.Services;

namespace PIM.Tests;

/// <summary>
/// Integration tests for User Story – Product Update.
///
/// Acceptance criteria covered:
///   AC-1  Ability to update product name, SKU, prices and attribute values.
///   AC-2  Changing a category deletes the old attribute values and allows new ones.
///   AC-3  Changes are persisted correctly in the database.
///   AC-4  The changelog is written after a successful update.
///
/// [OneTimeSetUp]     seeds the test database with setup_db.sql + data.sql.
/// [OneTimeTearDown]  drops all PIM tables so the test database is clean for next run.
/// [SetUp] / [TearDown] wrap each test in a transaction that is rolled back, keeping tests isolated.
/// </summary>
[TestFixture]
public class US03Test
{
    // ── Constants ─────────────────────────────────────────────────────────────

    private const string TestCategoryName         = "GPU";
    private const string AltCategoryName          = "RAM";
    private const string TestAttributeName        = "VRAM";

    // ── Per-fixture state ─────────────────────────────────────────────────────

    private static int _fixtureCategoryId;
    private static int _fixtureAltCategoryId;
    private static int _fixtureAttributeId;

    // ── Per-test state ────────────────────────────────────────────────────────

    private NpgsqlConnection      _connection     = null!;
    private NpgsqlTransaction     _transaction    = null!;
    private CreateProductService  _createService  = null!;
    private UpdateProductService  _updateService  = null!;

    private int _testCategoryId;
    private int _testAltCategoryId;
    private int _testAttributeId;

    // ─────────────────────────────────────────────────────────────────────────
    // One-time: seed the test database
    // ─────────────────────────────────────────────────────────────────────────

    [OneTimeSetUp]
    public void FixtureSetUp()
    {
        DatabaseConnection.ConnectionString = TestDatabaseConnection.ConnectionString;

        string assemblyDir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)!;
        RunSqlFile(Path.Combine(assemblyDir, "setup_db.sql"));
        RunSqlFile(Path.Combine(assemblyDir, "data.sql"));

        using NpgsqlConnection bootstrap = new NpgsqlConnection(TestDatabaseConnection.ConnectionString);
        bootstrap.Open();
        _fixtureCategoryId    = ScalarInt(bootstrap, "SELECT id FROM pim_categories WHERE name = @v", TestCategoryName);
        _fixtureAltCategoryId = ScalarInt(bootstrap, "SELECT id FROM pim_categories WHERE name = @v", AltCategoryName);
        _fixtureAttributeId   = ScalarInt(bootstrap, "SELECT id FROM pim_attributes  WHERE attribute_name = @v", TestAttributeName);
        bootstrap.Close();
    }

    [OneTimeTearDown]
    public void FixtureTearDown()
    {
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
        _updateService = new UpdateProductService();

        _testCategoryId    = _fixtureCategoryId;
        _testAltCategoryId = _fixtureAltCategoryId;
        _testAttributeId   = _fixtureAttributeId;
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
    // AC-1 · Update product name
    // ─────────────────────────────────────────────────────────────────────────

    /// <summary>
    /// AC-1 – Updating a product name must persist the new name in pim_products.
    /// </summary>
    [Test]
    public void UpdateProduct_ChangesProductName_InDatabase()
    {
        string originalName = $"Original {Guid.NewGuid()}";
        string updatedName  = $"Updated {Guid.NewGuid()}";
        string sku          = $"SKU-{Guid.NewGuid()}";

        _createService.CreateProduct(originalName, _testCategoryId, sku, 100m, 90m, 60m, new Dictionary<int, string>());

        int productId = QuerySingleInt("SELECT id FROM pim_products WHERE name = @v", originalName);
        int variantId = QuerySingleInt("SELECT id FROM pim_product_variants WHERE sku = @v", sku);

        _updateService.UpdateProduct(variantId, productId, updatedName, sku, 100m, 90m, 60m, new Dictionary<int, string>(), null);

        string storedName = QuerySingleString("SELECT name FROM pim_products WHERE id = @v", productId);

        Assert.That(storedName, Is.EqualTo(updatedName), "Product name was not updated in the database.");
    }

    /// <summary>
    /// AC-1 – After updating the name, the old name must no longer exist.
    /// </summary>
    [Test]
    public void UpdateProduct_OldName_NoLongerExistsInDatabase()
    {
        string originalName = $"OldName {Guid.NewGuid()}";
        string updatedName  = $"NewName {Guid.NewGuid()}";
        string sku          = $"SKU-{Guid.NewGuid()}";

        _createService.CreateProduct(originalName, _testCategoryId, sku, 100m, 90m, 60m, new Dictionary<int, string>());

        int productId = QuerySingleInt("SELECT id FROM pim_products WHERE name = @v", originalName);
        int variantId = QuerySingleInt("SELECT id FROM pim_product_variants WHERE sku = @v", sku);

        _updateService.UpdateProduct(variantId, productId, updatedName, sku, 100m, 90m, 60m, new Dictionary<int, string>(), null);

        int oldNameCount = QuerySingleInt("SELECT COUNT(*)::int FROM pim_products WHERE name = @v", originalName);

        Assert.That(oldNameCount, Is.EqualTo(0), "Old product name should no longer exist after update.");
    }

    // ─────────────────────────────────────────────────────────────────────────
    // AC-1 · Update SKU
    // ─────────────────────────────────────────────────────────────────────────

    /// <summary>
    /// AC-1 – Updating a SKU must persist the new SKU on the variant row.
    /// </summary>
    [Test]
    public void UpdateProduct_ChangesSku_InDatabase()
    {
        string originalSku = $"OLD-SKU-{Guid.NewGuid()}";
        string updatedSku  = $"NEW-SKU-{Guid.NewGuid()}";
        string name        = $"Product {Guid.NewGuid()}";

        _createService.CreateProduct(name, _testCategoryId, originalSku, 100m, 90m, 60m, new Dictionary<int, string>());

        int productId = QuerySingleInt("SELECT id FROM pim_products WHERE name = @v", name);
        int variantId = QuerySingleInt("SELECT id FROM pim_product_variants WHERE sku = @v", originalSku);

        _updateService.UpdateProduct(variantId, productId, name, updatedSku, 100m, 90m, 60m, new Dictionary<int, string>(), null);

        string storedSku = QuerySingleString("SELECT sku FROM pim_product_variants WHERE id = @v", variantId);

        Assert.That(storedSku, Is.EqualTo(updatedSku), "SKU was not updated in the database.");
    }

    // ─────────────────────────────────────────────────────────────────────────
    // AC-1 · Update prices
    // ─────────────────────────────────────────────────────────────────────────

    /// <summary>
    /// AC-1 – Updating the price must persist the new price on the variant row.
    /// </summary>
    [Test]
    public void UpdateProduct_ChangesPrice_InDatabase()
    {
        string sku  = $"SKU-{Guid.NewGuid()}";
        string name = $"Product {Guid.NewGuid()}";

        _createService.CreateProduct(name, _testCategoryId, sku, 100m, 90m, 60m, new Dictionary<int, string>());

        int productId = QuerySingleInt("SELECT id FROM pim_products WHERE name = @v", name);
        int variantId = QuerySingleInt("SELECT id FROM pim_product_variants WHERE sku = @v", sku);

        _updateService.UpdateProduct(variantId, productId, name, sku, 299m, 90m, 60m, new Dictionary<int, string>(), null);

        decimal storedPrice = QuerySingleDecimal("SELECT price FROM pim_product_variants WHERE id = @v", variantId);

        Assert.That(storedPrice, Is.EqualTo(299m), "Price was not updated in the database.");
    }

    /// <summary>
    /// AC-1 – Updating base price and cost price must persist both new values.
    /// </summary>
    [Test]
    public void UpdateProduct_ChangesBasePriceAndCostPrice_InDatabase()
    {
        string sku  = $"SKU-{Guid.NewGuid()}";
        string name = $"Product {Guid.NewGuid()}";

        _createService.CreateProduct(name, _testCategoryId, sku, 100m, 90m, 60m, new Dictionary<int, string>());

        int productId = QuerySingleInt("SELECT id FROM pim_products WHERE name = @v", name);
        int variantId = QuerySingleInt("SELECT id FROM pim_product_variants WHERE sku = @v", sku);

        _updateService.UpdateProduct(variantId, productId, name, sku, 100m, 250m, 180m, new Dictionary<int, string>(), null);

        using NpgsqlCommand cmd = new NpgsqlCommand(
            "SELECT base_price, cost_price FROM pim_product_variants WHERE id = @id",
            _connection, _transaction);
        cmd.Parameters.AddWithValue("id", variantId);

        using NpgsqlDataReader reader = cmd.ExecuteReader();
        Assert.That(reader.Read(), Is.True, "No variant row found.");
        Assert.That(reader.GetDecimal(0), Is.EqualTo(250m), "base_price was not updated.");
        Assert.That(reader.GetDecimal(1), Is.EqualTo(180m), "cost_price was not updated.");
    }

    // ─────────────────────────────────────────────────────────────────────────
    // AC-1 · Update attribute values
    // ─────────────────────────────────────────────────────────────────────────

    /// <summary>
    /// AC-1 – Updating an attribute value must persist the new value in pim_variant_attribute_values.
    /// </summary>
    [Test]
    public void UpdateProduct_ChangesAttributeValue_InDatabase()
    {
        string sku  = $"SKU-{Guid.NewGuid()}";
        string name = $"Product {Guid.NewGuid()}";

        _createService.CreateProduct(name, _testCategoryId, sku, 100m, 90m, 60m,
            new Dictionary<int, string> { { _testAttributeId, "8" } });

        int productId = QuerySingleInt("SELECT id FROM pim_products WHERE name = @v", name);
        int variantId = QuerySingleInt("SELECT id FROM pim_product_variants WHERE sku = @v", sku);

        _updateService.UpdateProduct(variantId, productId, name, sku, 100m, 90m, 60m,
            new Dictionary<int, string> { { _testAttributeId, "16" } }, null);

        string storedValue = QuerySingleString(
            $"SELECT value FROM pim_variant_attribute_values WHERE variant_id = @v AND attribute_id = {_testAttributeId}",
            variantId);

        Assert.That(storedValue, Is.EqualTo("16"), "Attribute value was not updated in the database.");
    }

    /// <summary>
    /// AC-1 – Updating multiple attribute values must persist all of them correctly.
    /// </summary>
    [Test]
    public void UpdateProduct_ChangesMultipleAttributeValues_AllPersistedCorrectly()
    {
        string sku  = $"SKU-{Guid.NewGuid()}";
        string name = $"Product {Guid.NewGuid()}";

        // Get two attribute ids for the test category
        int attrId1 = _testAttributeId;
        int attrId2 = QuerySingleInt(
            "SELECT a.id FROM pim_category_attributes ca JOIN pim_attributes a ON a.id = ca.attribute_id WHERE ca.category_id = @v AND a.id != " + attrId1 + " LIMIT 1",
            _testCategoryId);

        if (attrId2 == 0)
        {
            Assert.Ignore("Test category does not have a second attribute — skipping.");
            return;
        }

        _createService.CreateProduct(name, _testCategoryId, sku, 100m, 90m, 60m,
            new Dictionary<int, string> { { attrId1, "8" }, { attrId2, "old" } });

        int productId = QuerySingleInt("SELECT id FROM pim_products WHERE name = @v", name);
        int variantId = QuerySingleInt("SELECT id FROM pim_product_variants WHERE sku = @v", sku);

        _updateService.UpdateProduct(variantId, productId, name, sku, 100m, 90m, 60m,
            new Dictionary<int, string> { { attrId1, "32" }, { attrId2, "new" } }, null);

        string storedValue1 = QuerySingleString(
            $"SELECT value FROM pim_variant_attribute_values WHERE variant_id = @v AND attribute_id = {attrId1}", variantId);
        string storedValue2 = QuerySingleString(
            $"SELECT value FROM pim_variant_attribute_values WHERE variant_id = @v AND attribute_id = {attrId2}", variantId);

        Assert.That(storedValue1, Is.EqualTo("32"),  "First attribute value was not updated.");
        Assert.That(storedValue2, Is.EqualTo("new"), "Second attribute value was not updated.");
    }

    // ─────────────────────────────────────────────────────────────────────────
    // AC-2 · Category change
    // ─────────────────────────────────────────────────────────────────────────

    /// <summary>
    /// AC-2 – Changing the category must update category_id on the product row.
    /// </summary>
    [Test]
    public void UpdateProduct_ChangesCategory_UpdatesCategoryIdOnProduct()
    {
        string sku  = $"SKU-{Guid.NewGuid()}";
        string name = $"Product {Guid.NewGuid()}";

        _createService.CreateProduct(name, _testCategoryId, sku, 100m, 90m, 60m, new Dictionary<int, string>());

        int productId = QuerySingleInt("SELECT id FROM pim_products WHERE name = @v", name);
        int variantId = QuerySingleInt("SELECT id FROM pim_product_variants WHERE sku = @v", sku);

        _updateService.UpdateProduct(variantId, productId, name, sku, 100m, 90m, 60m,
            new Dictionary<int, string>(), _testAltCategoryId);

        int storedCategoryId = QuerySingleInt("SELECT category_id FROM pim_products WHERE id = @v", productId);

        Assert.That(storedCategoryId, Is.EqualTo(_testAltCategoryId), "Category id was not updated on the product.");
    }

    /// <summary>
    /// AC-2 – Changing the category must delete all old attribute values
    /// since they no longer apply to the new category.
    /// </summary>
    [Test]
    public void UpdateProduct_ChangesCategory_DeletesOldAttributeValues()
    {
        string sku  = $"SKU-{Guid.NewGuid()}";
        string name = $"Product {Guid.NewGuid()}";

        _createService.CreateProduct(name, _testCategoryId, sku, 100m, 90m, 60m,
            new Dictionary<int, string> { { _testAttributeId, "8" } });

        int productId = QuerySingleInt("SELECT id FROM pim_products WHERE name = @v", name);
        int variantId = QuerySingleInt("SELECT id FROM pim_product_variants WHERE sku = @v", sku);

        _updateService.UpdateProduct(variantId, productId, name, sku, 100m, 90m, 60m,
            new Dictionary<int, string>(), _testAltCategoryId);

        int attrCount = QuerySingleInt(
            "SELECT COUNT(*)::int FROM pim_variant_attribute_values WHERE variant_id = @v", variantId);

        Assert.That(attrCount, Is.EqualTo(0), "Old attribute values should be deleted when category changes.");
    }

    /// <summary>
    /// AC-2 – Changing the category and providing new attribute values
    /// must insert those new attribute values for the variant.
    /// </summary>
    [Test]
    public void UpdateProduct_ChangesCategory_InsertsNewAttributeValues()
    {
        string sku  = $"SKU-{Guid.NewGuid()}";
        string name = $"Product {Guid.NewGuid()}";

        _createService.CreateProduct(name, _testCategoryId, sku, 100m, 90m, 60m,
            new Dictionary<int, string> { { _testAttributeId, "8" } });

        int productId = QuerySingleInt("SELECT id FROM pim_products WHERE name = @v", name);
        int variantId = QuerySingleInt("SELECT id FROM pim_product_variants WHERE sku = @v", sku);

        // Get an attribute that belongs to the alt category
        int altAttrId = QuerySingleInt(
            "SELECT a.id FROM pim_category_attributes ca JOIN pim_attributes a ON a.id = ca.attribute_id WHERE ca.category_id = @v LIMIT 1",
            _testAltCategoryId);

        if (altAttrId == 0)
        {
            Assert.Ignore("Alt category has no attributes — skipping.");
            return;
        }

        _updateService.UpdateProduct(variantId, productId, name, sku, 100m, 90m, 60m,
            new Dictionary<int, string> { { altAttrId, "DDR5" } }, _testAltCategoryId);

        string storedValue = QuerySingleString(
            $"SELECT value FROM pim_variant_attribute_values WHERE variant_id = @v AND attribute_id = {altAttrId}",
            variantId);

        Assert.That(storedValue, Is.EqualTo("DDR5"), "New attribute value was not inserted after category change.");
    }

    /// <summary>
    /// AC-2 – When the category does NOT change, the old attribute values must
    /// still be present after the update.
    /// </summary>
    [Test]
    public void UpdateProduct_SameCategory_KeepsExistingAttributeValues()
    {
        string sku  = $"SKU-{Guid.NewGuid()}";
        string name = $"Product {Guid.NewGuid()}";

        _createService.CreateProduct(name, _testCategoryId, sku, 100m, 90m, 60m,
            new Dictionary<int, string> { { _testAttributeId, "8" } });

        int productId = QuerySingleInt("SELECT id FROM pim_products WHERE name = @v", name);
        int variantId = QuerySingleInt("SELECT id FROM pim_product_variants WHERE sku = @v", sku);

        // Update without changing category
        _updateService.UpdateProduct(variantId, productId, name, sku, 199m, 90m, 60m,
            new Dictionary<int, string> { { _testAttributeId, "8" } }, null);

        int attrCount = QuerySingleInt(
            "SELECT COUNT(*)::int FROM pim_variant_attribute_values WHERE variant_id = @v", variantId);

        Assert.That(attrCount, Is.GreaterThan(0), "Attribute values should still exist when category did not change.");
    }

    // ─────────────────────────────────────────────────────────────────────────
    // AC-3 · Full update — all fields at once
    // ─────────────────────────────────────────────────────────────────────────

    /// <summary>
    /// AC-3 – Updating all fields at once must persist every change correctly.
    /// </summary>
    [Test]
    public void UpdateProduct_AllFieldsAtOnce_AllPersistedCorrectly()
    {
        string originalName = $"Original {Guid.NewGuid()}";
        string originalSku  = $"OLD-{Guid.NewGuid()}";
        string updatedName  = $"Updated {Guid.NewGuid()}";
        string updatedSku   = $"NEW-{Guid.NewGuid()}";

        _createService.CreateProduct(originalName, _testCategoryId, originalSku, 100m, 90m, 60m,
            new Dictionary<int, string> { { _testAttributeId, "8" } });

        int productId = QuerySingleInt("SELECT id FROM pim_products WHERE name = @v", originalName);
        int variantId = QuerySingleInt("SELECT id FROM pim_product_variants WHERE sku = @v", originalSku);

        _updateService.UpdateProduct(variantId, productId, updatedName, updatedSku, 500m, 450m, 300m,
            new Dictionary<int, string> { { _testAttributeId, "24" } }, null);

        string storedName  = QuerySingleString("SELECT name FROM pim_products WHERE id = @v", productId);
        string storedSku   = QuerySingleString("SELECT sku FROM pim_product_variants WHERE id = @v", variantId);
        decimal storedPrice = QuerySingleDecimal("SELECT price FROM pim_product_variants WHERE id = @v", variantId);
        string storedAttr  = QuerySingleString(
            $"SELECT value FROM pim_variant_attribute_values WHERE variant_id = @v AND attribute_id = {_testAttributeId}",
            variantId);

        Assert.That(storedName,  Is.EqualTo(updatedName), "Name mismatch.");
        Assert.That(storedSku,   Is.EqualTo(updatedSku),  "SKU mismatch.");
        Assert.That(storedPrice, Is.EqualTo(500m),        "Price mismatch.");
        Assert.That(storedAttr,  Is.EqualTo("24"),        "Attribute value mismatch.");
    }

    /// <summary>
    /// AC-3 – Calling UpdateProduct must not throw when all inputs are valid.
    /// </summary>
    [Test]
    public void UpdateProduct_WithValidData_DoesNotThrow()
    {
        string sku  = $"SKU-{Guid.NewGuid()}";
        string name = $"Product {Guid.NewGuid()}";

        _createService.CreateProduct(name, _testCategoryId, sku, 100m, 90m, 60m, new Dictionary<int, string>());

        int productId = QuerySingleInt("SELECT id FROM pim_products WHERE name = @v", name);
        int variantId = QuerySingleInt("SELECT id FROM pim_product_variants WHERE sku = @v", sku);

        Assert.DoesNotThrow(() =>
            _updateService.UpdateProduct(variantId, productId, name, sku, 150m, 130m, 80m,
                new Dictionary<int, string>(), null));
    }

    // ─────────────────────────────────────────────────────────────────────────
    // AC-4 · Changelog
    // ─────────────────────────────────────────────────────────────────────────

    /// <summary>
    /// AC-4 – After a successful update the changelog file must exist.
    /// </summary>
    [Test]
    public void UpdateProduct_WritesChangelogFile()
    {
        string sku  = $"SKU-{Guid.NewGuid()}";
        string name = $"Product {Guid.NewGuid()}";

        _createService.CreateProduct(name, _testCategoryId, sku, 100m, 90m, 60m, new Dictionary<int, string>());

        int productId = QuerySingleInt("SELECT id FROM pim_products WHERE name = @v", name);
        int variantId = QuerySingleInt("SELECT id FROM pim_product_variants WHERE sku = @v", sku);

        _updateService.UpdateProduct(variantId, productId, name, sku, 200m, 180m, 100m,
            new Dictionary<int, string>(), null);

        string logPath = Path.Combine(
            AppDomain.CurrentDomain.BaseDirectory, "..", "..", "..", "..", "PIM", "Log", "changelog.txt");

        Assert.That(File.Exists(logPath), Is.True, "Changelog file was not created after update.");
    }

    /// <summary>
    /// AC-4 – The changelog entry must contain the word UPDATE and the variant id.
    /// </summary>
    [Test]
    public void UpdateProduct_ChangelogContainsUpdateEntryWithVariantId()
    {
        string sku  = $"SKU-{Guid.NewGuid()}";
        string name = $"Product {Guid.NewGuid()}";

        _createService.CreateProduct(name, _testCategoryId, sku, 100m, 90m, 60m, new Dictionary<int, string>());

        int productId = QuerySingleInt("SELECT id FROM pim_products WHERE name = @v", name);
        int variantId = QuerySingleInt("SELECT id FROM pim_product_variants WHERE sku = @v", sku);

        _updateService.UpdateProduct(variantId, productId, name, sku, 200m, 180m, 100m,
            new Dictionary<int, string>(), null);

        string logPath = Path.Combine(
            AppDomain.CurrentDomain.BaseDirectory, "..", "..", "..", "..", "PIM", "Log", "changelog.txt");

        string logContent = File.ReadAllText(logPath);

        Assert.That(logContent, Does.Contain("UPDATE"),        "Changelog does not contain UPDATE operation.");
        Assert.That(logContent, Does.Contain(variantId.ToString()), "Changelog does not contain the variant id.");
    }

    // ─────────────────────────────────────────────────────────────────────────
    // Query helpers
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