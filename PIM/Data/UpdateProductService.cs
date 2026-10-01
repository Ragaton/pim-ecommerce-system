using System.Collections.Generic;
using Npgsql;

namespace PIM.Services;

// This class handles all the database work for editing an existing product.
// It can update the product name, SKU, prices, attribute values,
// and also handles changing the category of a product.
public class UpdateProductService
{
    // This is the main method that saves all the changes the user made in the edit form.
    // It takes the variant id so we know which product to update,
    // and all the new values the user typed in.
    // attributeValues is a dictionary where the key is the attribute id and the value is what the user typed.
    // newCategoryId is optional — if the user changed the category it will have a value, otherwise it is null.
     public void UpdateProduct(
        int variantId,
        int productId,
        string productName,
        string sku,
        decimal price,
        decimal basePrice,
        decimal costPrice,
        Dictionary<int, string> attributeValues,
        int? newCategoryId)
    {
        NpgsqlConnection connection = DatabaseConnection.GetConnection();
        connection.Open();
 
        // Fetch the current state BEFORE making any changes so we can log it
        NpgsqlCommand fetchBeforeCommand = new NpgsqlCommand(
            "SELECT p.name, pv.sku, pv.price, pv.base_price, pv.cost_price " +
            "FROM pim_products p " +
            "JOIN pim_product_variants pv ON pv.product_id = p.id " +
            "WHERE pv.id = @variantId",
            connection);
 
        fetchBeforeCommand.Parameters.AddWithValue("variantId", variantId);
 
        NpgsqlDataReader beforeReader = fetchBeforeCommand.ExecuteReader();
        beforeReader.Read();
 
        string  beforeProductName = beforeReader.GetString(0);
        string  beforeSku         = beforeReader.GetString(1);
        decimal beforePrice       = beforeReader.GetDecimal(2);
        decimal beforeBasePrice   = beforeReader.GetDecimal(3);
        decimal beforeCostPrice   = beforeReader.GetDecimal(4);
 
        beforeReader.Close();
 
        // Step 1: Update the product name
        NpgsqlCommand updateProductCommand = new NpgsqlCommand(
            "UPDATE pim_products SET name = @name WHERE id = @productId",
            connection);
 
        updateProductCommand.Parameters.AddWithValue("name", productName);
        updateProductCommand.Parameters.AddWithValue("productId", productId);
        updateProductCommand.ExecuteNonQuery();
 
        // Step 2: If the category changed, update it and reset attribute values
        if (newCategoryId.HasValue)
        {
            NpgsqlCommand updateCategoryCommand = new NpgsqlCommand(
                "UPDATE pim_products SET category_id = @categoryId WHERE id = @productId",
                connection);
 
            updateCategoryCommand.Parameters.AddWithValue("categoryId", newCategoryId.Value);
            updateCategoryCommand.Parameters.AddWithValue("productId", productId);
            updateCategoryCommand.ExecuteNonQuery();
 
            // Delete all existing attribute values since the new category has different attributes
            NpgsqlCommand deleteAttrsCommand = new NpgsqlCommand(
                "DELETE FROM pim_variant_attribute_values WHERE variant_id = @variantId",
                connection);
 
            deleteAttrsCommand.Parameters.AddWithValue("variantId", variantId);
            deleteAttrsCommand.ExecuteNonQuery();
 
            // Insert the new attribute values from the new category
            foreach (KeyValuePair<int, string> entry in attributeValues)
            {
                if (!string.IsNullOrWhiteSpace(entry.Value))
                {
                    NpgsqlCommand insertAttrCommand = new NpgsqlCommand(
                        "INSERT INTO pim_variant_attribute_values (variant_id, attribute_id, value) " +
                        "VALUES (@variantId, @attributeId, @value)",
                        connection);
 
                    insertAttrCommand.Parameters.AddWithValue("variantId", variantId);
                    insertAttrCommand.Parameters.AddWithValue("attributeId", entry.Key);
                    insertAttrCommand.Parameters.AddWithValue("value", entry.Value);
                    insertAttrCommand.ExecuteNonQuery();
                }
            }
        }
        else
        {
            // Step 3: Category did not change — just update the existing attribute values
            foreach (KeyValuePair<int, string> entry in attributeValues)
            {
                NpgsqlCommand updateAttrCommand = new NpgsqlCommand(
                    "UPDATE pim_variant_attribute_values SET value = @value " +
                    "WHERE variant_id = @variantId AND attribute_id = @attributeId",
                    connection);
 
                updateAttrCommand.Parameters.AddWithValue("value", entry.Value);
                updateAttrCommand.Parameters.AddWithValue("variantId", variantId);
                updateAttrCommand.Parameters.AddWithValue("attributeId", entry.Key);
                updateAttrCommand.ExecuteNonQuery();
            }
        }
 
        // Step 4: Update the variant prices and SKU
        NpgsqlCommand updateVariantCommand = new NpgsqlCommand(
            "UPDATE pim_product_variants SET sku = @sku, price = @price, base_price = @basePrice, cost_price = @costPrice " +
            "WHERE id = @variantId",
            connection);
 
        updateVariantCommand.Parameters.AddWithValue("sku", sku);
        updateVariantCommand.Parameters.AddWithValue("price", price);
        updateVariantCommand.Parameters.AddWithValue("basePrice", basePrice);
        updateVariantCommand.Parameters.AddWithValue("costPrice", costPrice);
        updateVariantCommand.Parameters.AddWithValue("variantId", variantId);
        updateVariantCommand.ExecuteNonQuery();
 
        connection.Close();
 
        // Log the update with before and after state
        ChangeLogService.LogUpdate(
            variantId, productId,
            beforeProductName, productName,
            beforeSku, sku,
            beforePrice, price,
            beforeBasePrice, basePrice,
            beforeCostPrice, costPrice);
    }
}