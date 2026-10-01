namespace Common.IPIM.DTO;

public class CategoryRow
{
    public int id { get; set; }
    
    public string name { get; set; }
    
    public bool isProductCategory { get; set; }
    
    public int? parentId { get; set; }
}
