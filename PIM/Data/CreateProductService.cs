using System.Collections.Generic;
using Npgsql;

namespace PIM.Services;

// This class creates new products in the database from the CreateProduct avalonia page
public class CreateProductService
{
    
    //Its one long method that creates the products in the database in steps. The variabels in the start is information that is given as parameteres in the avalonia page
    public void CreateProduct(
        string productName,
        int categoryId,
        string sku,
        decimal price,
        decimal basePrice,
        decimal costPrice,
        Dictionary<int, string> attributeValues)
    {
        NpgsqlConnection connection = DatabaseConnection.GetConnection();
        connection.Open();
 
        // Step 1: Check if product already exists, if so reuse it, if not create it
        NpgsqlCommand checkProductCommand = new NpgsqlCommand(
            "SELECT id FROM pim_products WHERE name = @name AND category_id = @categoryId",
            connection);
 
        checkProductCommand.Parameters.AddWithValue("name", productName);
        checkProductCommand.Parameters.AddWithValue("categoryId", categoryId);
 
        object existingProduct = checkProductCommand.ExecuteScalar();
 
        int productId;
 
        if (existingProduct != null)
        {
            productId = (int)existingProduct;
        }
        else
        {
            // brand is now included in the insert
            NpgsqlCommand insertProductCommand = new NpgsqlCommand(
                "INSERT INTO pim_products (name, category_id) VALUES (@name, @categoryId) RETURNING id",
                connection);
 
            insertProductCommand.Parameters.AddWithValue("name", productName);
            insertProductCommand.Parameters.AddWithValue("categoryId", categoryId);
 
            productId = (int)insertProductCommand.ExecuteScalar();
        }
 
        // Step 2: Insert the variant
        NpgsqlCommand insertVariantCommand = new NpgsqlCommand(
            "INSERT INTO pim_product_variants (product_id, sku, price, base_price, cost_price) " +
            "VALUES (@productId, @sku, @price, @basePrice, @costPrice) RETURNING id",
            connection);
 
        insertVariantCommand.Parameters.AddWithValue("productId", productId);
        insertVariantCommand.Parameters.AddWithValue("sku", sku);
        insertVariantCommand.Parameters.AddWithValue("price", price);
        insertVariantCommand.Parameters.AddWithValue("basePrice", basePrice);
        insertVariantCommand.Parameters.AddWithValue("costPrice", costPrice);
 
        int variantId = (int)insertVariantCommand.ExecuteScalar();
 
        // Step 3: Insert each attribute value
        foreach (KeyValuePair<int, string> entry in attributeValues)
        {
            int    attributeId = entry.Key;
            string value       = entry.Value;
 
            NpgsqlCommand insertAttrCommand = new NpgsqlCommand(
                "INSERT INTO pim_variant_attribute_values (variant_id, attribute_id, value) " +
                "VALUES (@variantId, @attributeId, @value)",
                connection);
 
            insertAttrCommand.Parameters.AddWithValue("variantId", variantId);
            insertAttrCommand.Parameters.AddWithValue("attributeId", attributeId);
            insertAttrCommand.Parameters.AddWithValue("value", value);
            insertAttrCommand.ExecuteNonQuery();
        }
 
        connection.Close();
 
        // Log the creation after all database work is done
        ChangeLogService.LogCreate(productName, categoryId, sku, price, basePrice, costPrice);
    }
}