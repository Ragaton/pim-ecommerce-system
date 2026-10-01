using Npgsql;
using PIM.Services;

namespace PIM.Data;

// This class handles setting a product as active or inactive.
// In the real world, products are rarely hard-deleted because legacy product data
// may still be needed for things like order history. Instead, a product can be
// marked inactive to hide it from the storefront without losing any data.
public class SetProductActiveService
{
    // Marks a product as active (visible/available).
    // Returns true if the product was found and updated, false if no product with that id existed.
    public bool SetActive(int variantId)
    {
        return SetIsActive(variantId, true);
    }
    
    // Marks a product as inactive (hidden/unavailable).
    // Returns true if the product was found and updated, false if no product with that id existed.
    public bool SetInactive(int variantId)
    {
        return SetIsActive(variantId, false);
    }
    
    //This method does the logic for setting the activity status of product variants
    private bool SetIsActive(int variantId, bool isActive)
    {
        NpgsqlConnection connection = DatabaseConnection.GetConnection();
        connection.Open();
 
        // Fetch the current active status before changing it so we can log the before state
        NpgsqlCommand fetchCurrentCommand = new NpgsqlCommand(
            "SELECT isactive FROM pim_product_variants WHERE id = @variantId",
            connection);
 
        fetchCurrentCommand.Parameters.AddWithValue("variantId", variantId);
 
        object currentStatus = fetchCurrentCommand.ExecuteScalar();
 
        if (currentStatus == null) // No variant found with that id
        {
            connection.Close();
            return false;
        }
 
        bool wasActive = (bool)currentStatus;
 
        // Update the isactive flag on the variant
        NpgsqlCommand updateCommand = new NpgsqlCommand(
            "UPDATE pim_product_variants SET isactive = @isActive WHERE id = @variantId",
            connection);
 
        updateCommand.Parameters.AddWithValue("isActive", isActive);
        updateCommand.Parameters.AddWithValue("variantId", variantId);
 
        // ExecuteNonQuery returns the number of rows affected
        int rowsAffected = updateCommand.ExecuteNonQuery();
 
        connection.Close();
 
        if (rowsAffected > 0)
            ChangeLogService.LogActiveStatusChange(variantId, wasActive, isActive);
 
        return rowsAffected > 0;
    }
}
