using Common.IPIM;
using PIM.Data;
using PIM.Services;

namespace PIM.Domain;

public static class PimBootstrapper
{
    public static void Register()
    {
        PIMRegistry.Register(new LoadProductVariantSummaryService());
        PIMRegistry.Register(new LoadProductService());
        PIMRegistry.Register(new CategorySupplyService());
    }
}