using System.Collections.Generic;
using Npgsql;
using PIM.Services; //remove after namespace fix
using Common.IPIM;
using Common.IPIM.Interfaces;

namespace PIM.Data;

// This class is intended for use by DAM.
// It provides a list of all product variants with just enough information
// for DAM to link their media collections to the correct products.

public class LoadProductVariantSummaryService :  IProductSummaryProvider
{
    // Returns a list of all product variants across all products.
    // Each entry contains the variant id, sku, and the parent product name.
    // Id and SKU are unique and are what DAM should use for actual linking.

    public List<ProductVariantSummary> GetAllVariantSummaries()
    {
        // Make the database connection
        NpgsqlConnection connection = DatabaseConnection.GetConnection();
        connection.Open();
        
        // JOIN pim_product_variants with pim_products to get the product name alongside the variant data
        // pim_products does not have is_active so we return all variants regardless of active status
        // DAM can filter on their end if needed
        NpgsqlCommand command = new NpgsqlCommand(
            "SELECT v.id, v.sku, p.name " +
            "FROM pim_product_variants v " +
            "JOIN pim_products p ON v.product_id = p.id " +
            "ORDER BY p.\"name\", v.sku",
            connection);
        // NpgsqlDataReader is used here instead of ExecuteScalar because we're returning multiple rows and multiple columns
        NpgsqlDataReader reader = command.ExecuteReader();

        // Read each row and map it to a ProductVariantSummary object
        List<ProductVariantSummary> summaries = new List<ProductVariantSummary>();
        
        while (reader.Read())
        {
            ProductVariantSummary summary = new ProductVariantSummary
            {
                Id          = reader.GetInt32(0),
                Sku         = reader.GetString(1),
                ProductName = reader.GetString(2)
            };
 
            summaries.Add(summary);
        }
        // Close the connection as we are done
        connection.Close();
        return summaries;
    }
}