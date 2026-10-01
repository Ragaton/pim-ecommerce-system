using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using Common.IPIM.DTO;
using PIM.Services;

namespace PIM.Domain;

public class ExportToJson
{
    private LoadProductService loadProductService = new LoadProductService();

    // Exports a single variant by its id to a JSON file named after its SKU.
    // Returns the file path it was saved to, or null if the variant was not found.
    public string ExportVariant(int variantId, string outputDirectory)
    {
        ProductRow match = loadProductService.GetVariantById(variantId);

        if (match == null)
        {
            return null;
        }

        // Build and save a JSON file for this single variant
        object jsonObject = BuildSingleVariantJson(match);

        string jsonString = JsonSerializer.Serialize(jsonObject, new JsonSerializerOptions
        {
            WriteIndented = true
        });

        string fileName = match.Sku + ".json";
        string fullPath = Path.Combine(outputDirectory, fileName);

        File.WriteAllText(fullPath, jsonString);

        return fullPath;
    }

    // Exports all products in a category to a single JSON file named after the category.
    // Variants that belong to the same product are grouped together under that product.
    // Returns the file path it was saved to, or null if no products were found in the category.
    public string ExportCategory(int categoryId, string categoryName, string outputDirectory)
    {
        // Load all product rows for this category
        List<ProductRow> rows = loadProductService.GetProductsInCategory(categoryId);

        if (rows == null || rows.Count == 0)
        {
            return null;
        }

        // Group the variants by product name so all variants of the same product
        // end up in one block with a list of variants inside it.
        // For example "iPhone 15" with 128GB Black and 256GB Red becomes one product entry
        // with two variant blocks inside it.
        Dictionary<string, List<ProductRow>> groupedByProduct = new Dictionary<string, List<ProductRow>>();

        foreach (ProductRow row in rows)
        {
            if (!groupedByProduct.ContainsKey(row.ProductName))
            {
                groupedByProduct[row.ProductName] = new List<ProductRow>();
            }

            groupedByProduct[row.ProductName].Add(row);
        }

        // Build the full JSON structure for the category
        List<object> productList = new List<object>();

        foreach (KeyValuePair<string, List<ProductRow>> group in groupedByProduct)
        {
            string productName      = group.Key;
            List<ProductRow> variants = group.Value;

            // Build a variant block for each variant under this product
            List<object> variantBlocks = new List<object>();

            foreach (ProductRow variant in variants)
            {
                Dictionary<string, string> attributeValues = ParseAttributeString(variant.Attributes);

                variantBlocks.Add(new Dictionary<string, object>
                {
                    ["sku"]             = variant.Sku,
                    ["price"]           = variant.Price,
                    ["basePrice"]       = variant.BasePrice,
                    ["costPrice"]       = variant.CostPrice,
                    ["isActive"]        = variant.IsActive,
                    ["attributeValues"] = attributeValues
                });
            }

            productList.Add(new Dictionary<string, object>
            {
                ["productName"] = productName,
                ["variants"]    = variantBlocks
            });
        }

        // Wrap everything in a category block
        object jsonObject = new Dictionary<string, object>
        {
            ["categoryName"] = categoryName,
            ["products"]     = productList
        };

        string jsonString = JsonSerializer.Serialize(jsonObject, new JsonSerializerOptions
        {
            WriteIndented = true
        });

        // Save the file named after the category
        string fileName = categoryName.Replace(" ", "_") + ".json";
        string fullPath = Path.Combine(outputDirectory, fileName);

        File.WriteAllText(fullPath, jsonString);

        return fullPath;
    }

    // Builds the JSON structure for a single variant export
    private object BuildSingleVariantJson(ProductRow row)
    {
        Dictionary<string, string> attributeValues = ParseAttributeString(row.Attributes);

        var variantBlock = new Dictionary<string, object>
        {
            ["sku"]             = row.Sku,
            ["price"]           = row.Price,
            ["basePrice"]       = row.BasePrice,
            ["costPrice"]       = row.CostPrice,
            ["isActive"]        = row.IsActive,
            ["attributeValues"] = attributeValues
        };

        return new Dictionary<string, object>
        {
            ["productName"]  = row.ProductName,
            ["categoryName"] = row.CategoryName,
            ["variants"]     = new List<object> { variantBlock }
        };
    }

    // The Attributes string from ProductRow looks like "Color: Black,  Storage: 128 GB"
    // This splits it back into a proper key/value dictionary for the JSON
    private Dictionary<string, string> ParseAttributeString(string attributes)
    {
        Dictionary<string, string> result = new Dictionary<string, string>();

        if (string.IsNullOrWhiteSpace(attributes))
        {
            return result;
        }

        string[] pairs = attributes.Split(",  ");

        foreach (string pair in pairs)
        {
            int colonIndex = pair.IndexOf(':');

            if (colonIndex <= 0)
            {
                continue;
            }

            string key   = pair.Substring(0, colonIndex).Trim();
            string value = pair.Substring(colonIndex + 1).Trim();

            result[key] = value;
        }

        return result;
    }
}