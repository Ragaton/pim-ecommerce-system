using Common.IPIM.DTO;

namespace Common.IPIM.Interfaces;


// Other groups uses this to retrieve all product variant data.
public interface IProductsProvider
{
    List<ProductRow> GetAllProducts();
    List<ProductRow> GetProductsInCategory(int categoryId);

    ProductRow GetVariantById(int variantId);
}