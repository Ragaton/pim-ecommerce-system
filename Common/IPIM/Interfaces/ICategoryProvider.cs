using Common.IPIM.DTO;

namespace Common.IPIM.Interfaces;

public interface ICategoryProvider
{
    List<CategoryRow> GetCategories();
}