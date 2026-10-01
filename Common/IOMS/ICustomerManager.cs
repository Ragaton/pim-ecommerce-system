namespace Common.IOMS
{
    public interface ICustomerManager
    {
        Task<int> SaveCustomer(Customer customer);

        //Method for the admin
        Task<Customer> GetCustomerInfoByPhone(string customerPhoneNumber);

        //Method for the user so that they can login
        Task<Customer> CustomerLogIn(string username, string password);

        Task<Customer> GetCustomer(int customerID);
        Task UpdateCustomerAddress(int customerid, string newAddress);
        Task UpdateCustomerEmail(int customerId, string newEmail);
        Task UpdateCustomerPassword(int customerId, string newPassword);


    }

}
