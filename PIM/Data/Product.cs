namespace PIM.Data;

public class Product
{
    public int Id { get; set; }
    public string? Name { get; set; }
    public string? Category { get; set; }
    public string? ActualPrice { get; set; }
    public string? BasePrice { get; set; }
    public string? Cost { get; set; }
    public string? Details { get; set; }
    public bool IsActive { get; set; }
    public bool IsSelected { get; set; }
}
