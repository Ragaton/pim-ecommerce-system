using Common.IPIM.DTO;

namespace Common.IPIM.Interfaces;

public interface IProductsInCategoryProvider
{
    List<ProductRow>  GetProductsInCategory(int categoryId);
}