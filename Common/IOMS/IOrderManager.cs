namespace Common.IOMS
{
    public interface IOrderManager
    {
        Task SaveOrder(Order order);

        Task<string> GetStatusOnOrder(int orderID);

        Task<Order> GetOrderById(int customerID);

        Task<List<Order>> GetOrdersByCustomer(int customerID);
    }
}