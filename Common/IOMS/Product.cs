public class Product
{
    public string Name {get; set;}
    public int Id {get; private set;}

    public double Price {get; set;}


    public Product(string Name, int Id, double Price)
    {
        this.Name = Name;
        this.Id = Id;
        this.Price = Price;
    }
}

