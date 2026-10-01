using System.Collections.Generic;
using Npgsql;

namespace PIM.Services;


public class BulkUpdateCategoryService
{

    public int UpdateCategoryForVariants(List<int> variantIds, int newCategoryId)
    {
        int updatedCount = 0;

        NpgsqlConnection connection = DatabaseConnection.GetConnection();
        connection.Open();

        foreach (int variantId in variantIds)
        {
            // First check that the variant exists and get its product id
            NpgsqlCommand getProductIdCommand = new NpgsqlCommand(
                "SELECT product_id FROM pim_product_variants WHERE id = @variantId",
                connection);

            getProductIdCommand.Parameters.AddWithValue("variantId", variantId);

            object result = getProductIdCommand.ExecuteScalar();

            if (result == null)
            {
                // No variant found with this id — skip it and move on
                continue;
            }

            int productId = (int)result;

            // Delete all attribute values for this variant since the new category
            // has different attributes and the old ones no longer apply
            NpgsqlCommand deleteAttrsCommand = new NpgsqlCommand(
                "DELETE FROM pim_variant_attribute_values WHERE variant_id = @variantId",
                connection);

            deleteAttrsCommand.Parameters.AddWithValue("variantId", variantId);
            deleteAttrsCommand.ExecuteNonQuery();

            // Update the category on the product
            NpgsqlCommand updateCategoryCommand = new NpgsqlCommand(
                "UPDATE pim_products SET category_id = @categoryId WHERE id = @productId",
                connection);

            updateCategoryCommand.Parameters.AddWithValue("categoryId", newCategoryId);
            updateCategoryCommand.Parameters.AddWithValue("productId", productId);
            updateCategoryCommand.ExecuteNonQuery();

            updatedCount++;
        }

        connection.Close();

        return updatedCount;
    }

    // Moves ALL products from one category to another.
    // Finds every product in the source category, deletes their attribute values
    // and updates them to the new category.
    // Returns the number of products that were updated.
    public int UpdateCategoryForAllProductsInCategory(int sourceCategoryId, int newCategoryId)
    {
        NpgsqlConnection connection = DatabaseConnection.GetConnection();
        connection.Open();

        // Get all product ids in the source category
        NpgsqlCommand getProductsCommand = new NpgsqlCommand(
            "SELECT id FROM pim_products WHERE category_id = @categoryId",
            connection);

        getProductsCommand.Parameters.AddWithValue("categoryId", sourceCategoryId);

        NpgsqlDataReader productReader = getProductsCommand.ExecuteReader();

        List<int> productIds = new List<int>();

        while (productReader.Read())
        {
            productIds.Add(productReader.GetInt32(0));
        }

        productReader.Close();

        if (productIds.Count == 0)
        {
            connection.Close();
            return 0;
        }

        // For each product, get all its variant ids and delete their attribute values
        foreach (int productId in productIds)
        {
            // Get all variant ids for this product
            NpgsqlCommand getVariantsCommand = new NpgsqlCommand(
                "SELECT id FROM pim_product_variants WHERE product_id = @productId",
                connection);

            getVariantsCommand.Parameters.AddWithValue("productId", productId);

            NpgsqlDataReader variantReader = getVariantsCommand.ExecuteReader();

            List<int> variantIds = new List<int>();

            while (variantReader.Read())
            {
                variantIds.Add(variantReader.GetInt32(0));
            }

            variantReader.Close();

            // Delete all attribute values for each variant
            foreach (int variantId in variantIds)
            {
                NpgsqlCommand deleteAttrsCommand = new NpgsqlCommand(
                    "DELETE FROM pim_variant_attribute_values WHERE variant_id = @variantId",
                    connection);

                deleteAttrsCommand.Parameters.AddWithValue("variantId", variantId);
                deleteAttrsCommand.ExecuteNonQuery();
            }

            // Update the category on the product
            NpgsqlCommand updateCategoryCommand = new NpgsqlCommand(
                "UPDATE pim_products SET category_id = @categoryId WHERE id = @productId",
                connection);

            updateCategoryCommand.Parameters.AddWithValue("categoryId", newCategoryId);
            updateCategoryCommand.Parameters.AddWithValue("productId", productId);
            updateCategoryCommand.ExecuteNonQuery();
        }

        connection.Close();

        return productIds.Count;
    }
}