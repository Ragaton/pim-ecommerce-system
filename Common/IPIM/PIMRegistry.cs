using System.Collections.Generic;
using Common.IPIM.Interfaces;
using Common.IPIM.DTO;

namespace Common.IPIM;
 
// Static registry that lives in Common and acts as the bridge between PIM and other components.
// PIM registers its implementation once at startup.
// DAM, OMS, and Shop call this directly - they only need a Common reference, never a PIM reference.
public static class PIMRegistry
{
    // Holds the registered PIM implementation
    private static IProductSummaryProvider _productSummaryProvider;
    private static IProductsProvider _productsProvider;
    private static ICategoryProvider _categoryProvider;
 
    // Called once by PIM at startup to register its implementation
    public static void Register(IProductSummaryProvider provider)
    {
        _productSummaryProvider = provider;
    }
    
    public static void Register(IProductsProvider provider)
    {
        _productsProvider = provider;
    }

    public static void Register(ICategoryProvider provider)
    {
        _categoryProvider = provider;
    }
 
    // Called by DAM, OMS, Shop - forwards to whatever PIM registered
    public static List<ProductVariantSummary> GetAllVariantSummaries()
    {
        if (_productSummaryProvider == null)
            throw new InvalidOperationException("No IProductVariantProvider has been registered. PIM must call PIMRegistry.Register() at startup.");
 
        return _productSummaryProvider.GetAllVariantSummaries();
    }
    
    public static List<ProductRow> GetAllProducts()
    {
        if (_productsProvider == null)
            throw new InvalidOperationException("No IProductsProvider has been registered. PIM must call PIMRegistry.Register() at startup.");

        return _productsProvider.GetAllProducts();
    }

    public static List<ProductRow> GetProductsInCategory(int categoryId)
    {
        if (_productsProvider == null)
            throw new InvalidOperationException("No IProductsProvider has been registered. PIM must call PIMRegistry.Register() at startup.");

        return _productsProvider.GetProductsInCategory(categoryId);
    }
    
    public static ProductRow GetVariantById(int variantId)
    {
        if (_productsProvider == null)
            throw new InvalidOperationException("No IProductsProvider has been registered. PIM must call PIMRegistry.Register() at startup.");

        return _productsProvider.GetVariantById(variantId);
    }

    public static List<CategoryRow> GetCategories()
    {
        if (_categoryProvider == null)
            throw new InvalidOperationException("No ICategoryProvider has been registered. PIM must call PIMRegistry.Register() at startup.");
        
        return _categoryProvider.GetCategories();
    }
}