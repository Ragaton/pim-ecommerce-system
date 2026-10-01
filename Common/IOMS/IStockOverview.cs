namespace Common.IOMS
{
    public interface IStockOverview
    {
        // Return types changes to Task to make methods async 
        Task<string> GetStockAmount(int productid);
        Task<string> UpdateStock(int id, int amount);
    }
}
