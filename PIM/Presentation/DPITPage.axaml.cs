using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Avalonia.Controls;
using PIM.Data;
using PIM.Services;
using Avalonia.Interactivity;
using Common.IPIM.DTO;

namespace PIM.Presentation;

public partial class DPITPage : UserControl
{
    public ObservableCollection<PIM.Data.Product> Products { get; set; } = new();
    private LoadProductService loadProductService = new LoadProductService();
    private SetProductActiveService setProductActiveService = new SetProductActiveService();
    private List<PIM.Data.Product> _allProducts = new List<PIM.Data.Product>();

    
    public DPITPage()
    {
        InitializeComponent();
        DataContext = this;
        
        LoadProducts();
    }
    
    private void OnDeleteClick(object sender, RoutedEventArgs e)
    {
        List<PIM.Data.Product> selected = new List<PIM.Data.Product>();

        foreach (PIM.Data.Product product in Products)
        {
            if (product.IsSelected)
            {
                selected.Add(product);
            }
        }

        if (selected.Count == 0) return;

        DeleteProductPage confirmPage = new DeleteProductPage(selected);
        confirmPage.DPitPage = this;

        Window window = new Window();
        window.Title   = "Delete Product";
        window.Width   = 400;
        window.Height  = 300;
        window.Content = confirmPage;
        window.Show();
    }
    
    // This method runs when the "Set Inactive" button is clicked on a row in the table.
    // It finds which product the clicked button belongs to, sets that product as
    // inactive in the database, and then reloads the table so the change is reflected.
    private void OnActivateClick(object sender, RoutedEventArgs e)
    {
        foreach (PIM.Data.Product product in Products)
        {
            if (product.IsSelected)
            {
                setProductActiveService.SetActive(product.Id);
            }
        }

        LoadProducts();
    }
    
    private void OnSearchChanged(object sender, TextChangedEventArgs e)
    {
        string query = ((TextBox)sender).Text?.ToLower() ?? "";

        Products.Clear();

        foreach (PIM.Data.Product product in _allProducts)
        {
            if (product.Name.ToLower().Contains(query) ||
                product.Id.ToString().Contains(query))
            {
                Products.Add(product);
            }
        }
    }
    
    public void LoadProducts()
    {
        Products.Clear();

        try
        {
            List<ProductRow> products = loadProductService.GetAllProducts();

            foreach (ProductRow product in products)
            {
                if (!product.IsActive)
                {
                    Products.Add(new PIM.Data.Product
                    {
                        Id = product.VariantId,
                        Name = product.ProductName,
                        ActualPrice = product.Price.ToString(),
                        BasePrice = product.BasePrice.ToString(),
                        Cost = product.CostPrice.ToString(),
                        Details = product.Attributes,
                        Category = product.CategoryName
                    });
                }
            }
            _allProducts = new List<PIM.Data.Product>(Products);
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error loading products: " + ex.Message);
        }
    }
}

/*.
private void OnLoadClick(object sender, RoutedEventArgs e)
{
    LoadProducts();
}
*/