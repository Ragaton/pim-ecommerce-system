using System.ComponentModel.DataAnnotations;
using System.Net;

public class Order
{

    public int Id { get; set; }
    public double TotalPrice { get; private set; }

    public Orderstatus Status { get; set; }
    public string DeliveryAddress { get; private set; }
    public Customer customer { get; private set; }
    public DateTime date { get; private set; }  //TimeStamp
    public Dictionary<Product, int> Products { get; private set; }
    public string BuyerName { get; private set; }



    //Used when an order is placed from the webshop
    public Order(Customer customer, Dictionary<Product, int> Products, string address)
    {
        this.customer = customer;
        Status = Orderstatus.Pending;
        date = DateTime.Now;
        this.Products = Products;
        DeliveryAddress = address;
        TotalPrice = CalculateTotalPrice(Products);
        BuyerName = customer.name;
    }

    
    //Used for returning an order from the database
    public Order(int id, Customer customer, DateTime date, Dictionary<Product, int> Products, int status, string address)
    {
        this.Id = id;
        this.customer = customer;
        Status = (Orderstatus)status;
        this.date = date;
        this.Products = Products;
        DeliveryAddress = address;
        TotalPrice = CalculateTotalPrice(Products);
        BuyerName = customer.name;
    }


    public override string ToString()
    {
        return $"Customer = {customer}, Buyer's name = {BuyerName}, Status = {Status}, Date = {date}, Products = {Products}, Delivery Address = {DeliveryAddress}, Total Price = {TotalPrice}";
    }

    private double CalculateTotalPrice(Dictionary<Product, int> products)
    {
        double price = 0;
        foreach (Product product in products.Keys)
        {
            price += product.Price * Products[product];
        }
        return price;
    }

    public enum Orderstatus
    {
        NonExistent = 0,
        Pending = 1,
        Shipped = 2,
        Delivered = 3,
        Cancelled = 4
    }
}
