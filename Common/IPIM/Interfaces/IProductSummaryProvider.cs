using System.Collections.Generic;

namespace Common.IPIM.Interfaces;

// DAM uses this to retrieve product variant data for linking media collections.
public interface IProductSummaryProvider
{
    List<ProductVariantSummary> GetAllVariantSummaries();
}