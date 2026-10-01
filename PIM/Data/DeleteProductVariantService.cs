using Npgsql;
using PIM.Services;

namespace PIM.Data;

// This class handles "true" deletion of a specific product variant and its attribute values.
// Because pim_variant_attribute_values has ON DELETE CASCADE from pim_product_variants,
// deleting the variant will automatically remove all its attribute values in the database.
// After deleting the variant, we check if the parent product has any variants left.
// If not, the product entry itself is also deleted, as a product without variants is meaningless.

//This feature is not supposed to be used a lot - the primary way to remove a product from active assortment
// is the "SetProductActiveService" - this feature is simply here to add possibilities and fulfill the project description
public class DeleteProductVariantService
{
    // Takes the id of the variant to delete
    // Returns true if the variant was found and deleted, false if no variant with that id existed.
    public bool DeleteVariant(int variantId)
    {
        NpgsqlConnection connection = DatabaseConnection.GetConnection();
        connection.Open();
 
        // Step 1: Check that the variant exists and grab its parent product_id and sku for the changelog
        NpgsqlCommand checkVariantCommand = new NpgsqlCommand(
            "SELECT product_id, sku FROM pim_product_variants WHERE id = @variantId",
            connection);
 
        checkVariantCommand.Parameters.AddWithValue("variantId", variantId);
 
        NpgsqlDataReader checkReader = checkVariantCommand.ExecuteReader();
 
        if (!checkReader.Read()) // No variant found with that id, nothing to delete
        {
            checkReader.Close();
            connection.Close();
            return false;
        }
 
        int    productId = checkReader.GetInt32(0);
        string sku       = checkReader.GetString(1);
        checkReader.Close();
 
        // Check if the variant is active - active variants cannot be deleted
        NpgsqlCommand checkActiveCommand = new NpgsqlCommand(
            "SELECT isactive FROM pim_product_variants WHERE id = @variantId",
            connection);
 
        checkActiveCommand.Parameters.AddWithValue("variantId", variantId);
        bool isActive = (bool)checkActiveCommand.ExecuteScalar();
 
        if (isActive) // Cannot delete an active variant
        {
            connection.Close();
            return false;
        }
 
        // Step 2: Delete the variant
        // The database will automatically cascade and delete all rows in
        // pim_variant_attribute_values that reference this variant id
        NpgsqlCommand deleteVariantCommand = new NpgsqlCommand(
            "DELETE FROM pim_product_variants WHERE id = @variantId",
            connection);
 
        deleteVariantCommand.Parameters.AddWithValue("variantId", variantId);
        deleteVariantCommand.ExecuteNonQuery();
 
        // Step 3: Check if the parent product still has any remaining variants
        NpgsqlCommand checkRemainingVariantsCommand = new NpgsqlCommand(
            "SELECT COUNT(*) FROM pim_product_variants WHERE product_id = @productId",
            connection);
 
        checkRemainingVariantsCommand.Parameters.AddWithValue("productId", productId);
 
        long remainingVariants = (long)checkRemainingVariantsCommand.ExecuteScalar();
 
        if (remainingVariants == 0) // No variants left, the parent product entry should also be removed
        {
            NpgsqlCommand deleteProductCommand = new NpgsqlCommand(
                "DELETE FROM pim_products WHERE id = @productId",
                connection);
 
            deleteProductCommand.Parameters.AddWithValue("productId", productId);
            deleteProductCommand.ExecuteNonQuery();
        }
 
        connection.Close();
 
        // Log the deletion after all database work is done
        ChangeLogService.LogDelete(variantId, productId, sku);
 
        return true;
    }
}