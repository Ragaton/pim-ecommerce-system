namespace Common.IPIM;
// Simple data class to hold the three fields DAM needs per variant.
// Name comes from pim_products and is included purely for human readability.
// Id and SKU are the unique identifiers DAM will actually use for linking media.
public class ProductVariantSummary
{
    public int  Id { get; set; }
    public string Sku { get; set; }
    public string ProductName { get; set; }
}