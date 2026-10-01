
namespace Common.IPIM.DTO;
// Product row is the class for the rows we show in the product list on the PIT page
public class ProductRow
{
    public int     ProductId    { get; set; }
    public string  ProductName  { get; set; }
    public string  CategoryName { get; set; }
    public int     VariantId    { get; set; }
    public string  Sku          { get; set; }
    public decimal Price        { get; set; }
    public decimal BasePrice    { get; set; }
    public decimal CostPrice    { get; set; }
    public string  Attributes   { get; set; }
    public bool    IsActive     { get; set; }
    public bool    IsSelected   { get; set; }
}
