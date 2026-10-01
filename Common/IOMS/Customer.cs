using System.Data.Common;

public class Customer
{

    public int id { get; set; }
    public string name { get; set; }
    public string address { get; set; }
    public string email { get; set; }
    public string phone { get; set; }
    public string username { get; set; }
    public string password { get; set; }

    /// <summary>
    /// Used for saving a customer to the database
    /// </summary>
    /// <param name="name"></param>
    /// <param name="address"></param>
    /// <param name="email"></param>
    /// <param name="phone"></param>
    /// <param name="username"></param>
    /// <param name="password"></param>
    public Customer(string name, string address, string email, string phone, string username, string password)
    {
        this.name = name;
        this.address = address;
        this.email = email;
        this.phone = phone;
        this.username = username;
        this.password = password;
    }
    /// <summary>
    /// Used for retrieving customer info
    /// </summary>
    /// <param name="id"></param>
    /// <param name="name"></param>
    /// <param name="address"></param>
    /// <param name="email"></param>
    /// <param name="phone"></param>
    /// <param name="username"></param>
    /// <param name="password"></param>
    public Customer(int id, string name, string address, string email, string phone, string username, string password)
    {
        this.id = id;
        this.name = name;
        this.address = address;
        this.email = email;
        this.phone = phone;
        this.username = username;
        this.password = password;
    }

    public override string ToString()
    {
        return $"Name: {name}\nPhone: {phone}\nAddress: {address}\nEmail: {email}\nUsername: {username}\nPassword: {password}";
    }


}