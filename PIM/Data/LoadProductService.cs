using System.Collections.Generic;
using Npgsql;
using Common.IPIM.Interfaces;
using Common.IPIM.DTO;

namespace PIM.Services;

// This is the class for the categories we use in the dropdown menu on the create page
public class CategoryItem
{
    public int    Id   { get; set; }
    public string Name { get; set; }
}
// This is the class for the attributes (Could be id= 1, name = storage and unit = GB)
// This is what we use for the dynamic attributes
public class AttributeItem
{
    public int    Id   { get; set; }
    public string Name { get; set; }
    public string Unit { get; set; }
}

// Here comes the fun stuff, we load / read the data from the database
public class LoadProductService : IProductsProvider
{
    // Returns ALL products from the database
    public List<ProductRow> GetAllProducts()
    {
        return GetProductsFiltered();
    }

    // Returns only products belonging to the given category id
    public List<ProductRow> GetProductsInCategory(int categoryId)
    {
        return GetProductsFiltered(categoryId: categoryId);
    }

    // Private shared method that both GetAllProducts and GetProductsInCategory use.
    // If categoryId is provided it adds a WHERE clause to filter by category, otherwise it returns everything.
    private List<ProductRow> GetProductsFiltered(int? categoryId = null)
    {
        List<ProductRow> productRows = new List<ProductRow>();

        // Open the database connection
        NpgsqlConnection connection = DatabaseConnection.GetConnection();
        connection.Open();

        // Build the SQL query - same as before but with an optional WHERE clause
        string sql =
            "SELECT p.id, p.name, c.name, pv.id, pv.sku, pv.price, pv.base_price, pv.cost_price, pv.isactive " +
            "FROM pim_products p " +
            "JOIN pim_categories c ON c.id = p.category_id " +
            "JOIN pim_product_variants pv ON pv.product_id = p.id ";

        if (categoryId.HasValue)
            sql += "WHERE p.category_id = @categoryId ";

        sql += "ORDER BY p.id DESC";

        NpgsqlCommand productCommand = new NpgsqlCommand(sql, connection);

        if (categoryId.HasValue)
            productCommand.Parameters.AddWithValue("categoryId", categoryId.Value);

        NpgsqlDataReader productReader = productCommand.ExecuteReader(); // ExecuteReader runs the SQL statement and lets us read through the results one entry at a time

        // This is a list to temporarily save the raw data before we do something to it
        List<(int productId, string productName, string categoryName, int variantId, string sku, decimal price, decimal basePrice, decimal costPrice, bool isActive)> rawProducts =
            new List<(int, string, string, int, string, decimal, decimal, decimal, bool)>();

        // Here we loop through each entry the database gave us back and store the values
        while (productReader.Read())
        {
            int     productId    = productReader.GetInt32(0);
            string  productName  = productReader.GetString(1);
            string  categoryName = productReader.GetString(2);
            int     variantId    = productReader.GetInt32(3);
            string  sku          = productReader.GetString(4);
            decimal price        = productReader.GetDecimal(5);
            decimal basePrice    = productReader.GetDecimal(6);
            decimal costPrice    = productReader.GetDecimal(7);
            bool    isActive     = productReader.GetBoolean(8);

            rawProducts.Add((productId, productName, categoryName, variantId, sku, price, basePrice, costPrice, isActive));
        }

        productReader.Close(); // Close the reader after the while loop as we are now done reading the entries

        foreach (var raw in rawProducts) // In this loop we go over each product we found and also get its unique attributes
        {
            // We fetch the attributes for the specific variant (This could be variant id 1 so it could be attributes like color: black, storage: 128) THIS IS A METHOD CALL TO ANOTHER METHOD DOWN THE FILE
            string attributes = GetAttributesForVariant(connection, raw.variantId);

            // Here we build the ProductRow object with all of the information and add it to our list
            ProductRow row = new ProductRow();
            row.ProductId    = raw.productId;
            row.ProductName  = raw.productName;
            row.CategoryName = raw.categoryName;
            row.VariantId    = raw.variantId;
            row.Sku          = raw.sku;
            row.Price        = raw.price;
            row.BasePrice    = raw.basePrice;
            row.CostPrice    = raw.costPrice;
            row.Attributes   = attributes;
            row.IsActive     = raw.isActive;

            productRows.Add(row);
        }

        connection.Close(); // Close the connection to the database

        return productRows; // Returns the final list
    }

    private string GetAttributesForVariant(NpgsqlConnection connection, int variantId) // This is the helper method to get the attributes for each item
    // It gets all the attribute values for one specific variant
    // As i wrote earlier this could be for a iPhone 15 variant
    // So it will return "Color: black, Screen_size: 6.1 in, storage: 128GB"
    // It also links the values to the unit
    {
        string attributes = ""; // We start with an empty string we will build along the way as we get the attributes and ids

        // Ask the database for all attribute values for the variant
        // If there is no unit (like in color) it will just leave it empty
        NpgsqlCommand attrCommand = new NpgsqlCommand(
            "SELECT a.attribute_name, vav.value, u.symbol " +
            "FROM pim_variant_attribute_values vav " +
            "JOIN pim_attributes a ON a.id = vav.attribute_id " +
            "LEFT JOIN pim_units u ON u.id = a.unit_id " +
            "WHERE vav.variant_id = @variantId " +
            "ORDER BY a.attribute_name",
            connection);

        attrCommand.Parameters.AddWithValue("variantId", variantId);

        NpgsqlDataReader attrReader = attrCommand.ExecuteReader();

        while (attrReader.Read())
        {
            string attributeName  = attrReader.GetString(0);
            string attributeValue = attrReader.GetString(1);
            string unit           = attrReader.IsDBNull(2) ? "" : " " + attrReader.GetString(2); // IsDBNull checks if the unit is empty in the database

            // We add a comma before each attribute except the first one
            if (attributes != "")
                attributes = attributes + ",  ";

            // Add the attribute to the string
            attributes = attributes + attributeName + ": " + attributeValue + unit;
        }

        attrReader.Close(); // Close the reader like before

        return attributes; // Returns the attributes string
    }

    public List<CategoryItem> GetProductCategories() // This method will get all categories where IsProductCategory is true
    {
        // Very much like earlier methods, we create a list, open a database connection, get the information from the database, assign the information to variables and save it in the list
        List<CategoryItem> categories = new List<CategoryItem>();

        NpgsqlConnection connection = DatabaseConnection.GetConnection();
        connection.Open();

        NpgsqlCommand command = new NpgsqlCommand(
            "SELECT id, name FROM pim_categories WHERE isProductCategory = true ORDER BY name",
            connection);

        NpgsqlDataReader reader = command.ExecuteReader();

        // Read the category one at a time and add it to the list
        while (reader.Read())
        {
            CategoryItem item = new CategoryItem();
            item.Id   = reader.GetInt32(0);
            item.Name = reader.GetString(1);

            categories.Add(item);
        }

        connection.Close();

        return categories;
    }

    // This is a method that fetches all the attributes that belong to a category.
    // We use it for the GUI so it can show the attributes the user needs to fill in for the selected category from the dropdown menu.
    public List<AttributeItem> GetAttributesForCategory(int categoryId)
    {
        // Again like the other ones, it creates a list, opens the database connection, gets the information and adds it to the list with the reader.
        List<AttributeItem> attributes = new List<AttributeItem>();

        NpgsqlConnection connection = DatabaseConnection.GetConnection();
        connection.Open();

        NpgsqlCommand command = new NpgsqlCommand(
            "SELECT a.id, a.attribute_name, u.symbol " +
            "FROM pim_category_attributes ca " +
            "JOIN pim_attributes a ON a.id = ca.attribute_id " +
            "LEFT JOIN pim_units u ON u.id = a.unit_id " +
            "WHERE ca.category_id = @categoryId " +
            "ORDER BY a.attribute_name",
            connection);

        command.Parameters.AddWithValue("categoryId", categoryId);

        NpgsqlDataReader reader = command.ExecuteReader();

        while (reader.Read())
        {
            AttributeItem item = new AttributeItem();
            item.Id   = reader.GetInt32(0);
            item.Name = reader.GetString(1);
            item.Unit = reader.IsDBNull(2) ? "" : reader.GetString(2); // We use IsDBNull again so if there is no unit it will save an empty string instead of crashing

            attributes.Add(item);
        }

        connection.Close();

        return attributes;
    }
    
    // fetches a single product's data by variant ID to pre-fill the form in Edit
    public ProductRow? GetProductRowById(int variantId)
{
    NpgsqlConnection connection = DatabaseConnection.GetConnection();
    connection.Open();

    NpgsqlCommand command = new NpgsqlCommand(
        "SELECT p.id, p.name, c.name, pv.id, pv.sku, pv.price, pv.base_price, pv.cost_price, pv.isactive " +
        "FROM pim_products p " +
        "JOIN pim_categories c ON c.id = p.category_id " +
        "JOIN pim_product_variants pv ON pv.product_id = p.id " +
        "WHERE pv.id = @variantId",
        connection);

    command.Parameters.AddWithValue("variantId", variantId);
    NpgsqlDataReader reader = command.ExecuteReader();

    if (!reader.Read())
    {
        connection.Close();
        return null;
    }

    int     productId      = reader.GetInt32(0);
    string  productName    = reader.GetString(1);
    string  categoryName   = reader.GetString(2);
    int     foundVariantId = reader.GetInt32(3);
    string  sku            = reader.GetString(4);
    decimal price          = reader.GetDecimal(5);
    decimal basePrice      = reader.GetDecimal(6);
    decimal costPrice      = reader.GetDecimal(7);
    bool    isActive       = reader.GetBoolean(8);

    reader.Close();

    string attributes = GetAttributesForVariant(connection, foundVariantId);
    connection.Close();

    ProductRow row = new ProductRow();
    row.ProductId    = productId;
    row.ProductName  = productName;
    row.CategoryName = categoryName;
    row.VariantId    = foundVariantId;
    row.Sku          = sku;
    row.Price        = price;
    row.BasePrice    = basePrice;
    row.CostPrice    = costPrice;
    row.Attributes   = attributes;
    row.IsActive     = isActive;

    return row;
}

    // fetches the individual attribute values so each one can fill its own TextBox:
public List<(int AttributeId, string AttributeName, string Value, string Unit)> GetRawAttributesForVariant(int variantId)
{
    var result = new List<(int, string, string, string)>();

    NpgsqlConnection connection = DatabaseConnection.GetConnection();
    connection.Open();

    NpgsqlCommand command = new NpgsqlCommand(
        "SELECT a.id, a.attribute_name, vav.value, u.symbol " +
        "FROM pim_variant_attribute_values vav " +
        "JOIN pim_attributes a ON a.id = vav.attribute_id " +
        "LEFT JOIN pim_units u ON u.id = a.unit_id " +
        "WHERE vav.variant_id = @variantId " +
        "ORDER BY a.attribute_name",
        connection);

    command.Parameters.AddWithValue("variantId", variantId);
    NpgsqlDataReader reader = command.ExecuteReader();

    while (reader.Read())
    {
        int    attributeId   = reader.GetInt32(0);
        string attributeName = reader.GetString(1);
        string value         = reader.GetString(2);
        string unit          = reader.IsDBNull(3) ? "" : reader.GetString(3);

        result.Add((attributeId, attributeName, value, unit));
    }

    reader.Close();
    connection.Close();

    return result;
}
public ProductRow GetVariantById(int variantId) //This function is used to Get a specific cariant based on the VariantID
    {
        NpgsqlConnection connection = DatabaseConnection.GetConnection();
        connection.Open();

        NpgsqlCommand command = new NpgsqlCommand(
            "SELECT p.id, p.name, c.name, pv.id, pv.sku, pv.price, pv.base_price, pv.cost_price, pv.isactive " +
            "FROM pim_product_variants pv " +
            "JOIN pim_products p ON p.id = pv.product_id " +
            "JOIN pim_categories c ON c.id = p.category_id " +
            "WHERE pv.id = @variantId",
            connection);

        command.Parameters.AddWithValue("variantId", variantId);

        NpgsqlDataReader reader = command.ExecuteReader();

        if (!reader.Read()) // No variant found with that ID
        {
            reader.Close();
            connection.Close();
            return null;
        }

        int     productId    = reader.GetInt32(0);
        string  productName  = reader.GetString(1);
        string  categoryName = reader.GetString(2);
        int     varId        = reader.GetInt32(3);
        string  sku          = reader.GetString(4);
        decimal price        = reader.GetDecimal(5);
        decimal basePrice    = reader.GetDecimal(6);
        decimal costPrice    = reader.GetDecimal(7);
        bool    isActive     = reader.GetBoolean(8);

        reader.Close();

        // Reuse the existing private helper to get the attributes string
        string attributes = GetAttributesForVariant(connection, variantId);

        connection.Close();

        ProductRow row = new ProductRow();
        row.ProductId    = productId;
        row.ProductName  = productName;
        row.CategoryName = categoryName;
        row.VariantId    = varId;
        row.Sku          = sku;
        row.Price        = price;
        row.BasePrice    = basePrice;
        row.CostPrice    = costPrice;
        row.Attributes   = attributes;
        row.IsActive     = isActive;

        return row;
    }

}