using System.Collections.Generic;
using Common.IPIM.DTO;
using Common.IPIM.Interfaces;
using Npgsql;
using PIM.Services;

namespace PIM.Domain;

// This class retrieves all categories from the database and exposes them through
// the ICategoryProvider interface so other components can access them via Common.
public class CategorySupplyService : ICategoryProvider
{
    // Returns a list of all categories in the system including their id, name,
    // whether they are a product category, and their parent category id if they have one.
    public List<CategoryRow> GetCategories()
    {
        List<CategoryRow> categories = new List<CategoryRow>();
        
        NpgsqlConnection connection = DatabaseConnection.GetConnection();
        connection.Open();

        // Fetch all four fields we need — parent_id can be null for top-level categories
        NpgsqlCommand command = new NpgsqlCommand(
            "SELECT id, name, isProductCategory, parent_id FROM pim_categories ORDER BY id", connection);
        
        NpgsqlDataReader reader = command.ExecuteReader();

        while (reader.Read())
        {
            CategoryRow category = new CategoryRow();

            category.id                = reader.GetInt32(0);
            category.name              = reader.GetString(1);
            category.isProductCategory = reader.GetBoolean(2);
            // parent_id is nullable — top-level categories have no parent so we use IsDBNull to check before reading
            category.parentId          = reader.IsDBNull(3) ? null : reader.GetInt32(3);
            
            categories.Add(category);
        }
        
        connection.Close();
        return categories;
    }
}