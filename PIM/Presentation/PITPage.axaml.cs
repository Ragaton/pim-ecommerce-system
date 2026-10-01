using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Avalonia.Controls;
using PIM.Data;
using PIM.Services;
using Avalonia.Interactivity;
using Common.IPIM.DTO;

namespace PIM.Presentation;

public partial class PITPage : UserControl
{
    public ObservableCollection<PIM.Data.Product> Products { get; set; } = new();
    private LoadProductService loadProductService = new LoadProductService();
    private SetProductActiveService setProductActiveService = new SetProductActiveService();
    private List<PIM.Data.Product> _allProducts = new List<PIM.Data.Product>();

    
    public PITPage()
    {
        InitializeComponent();
        DataContext = this;
        
        LoadProducts();
    }
    
    private void OnCreateClick(object sender, RoutedEventArgs e)
    {
        CreateProductPage createPage = new CreateProductPage(); //We "Create" the CreateProdduct page and saves a reference back to this PIT page
        createPage.PitPage = this;

        Window window = new Window(); //This creates the new window
        window.Title   = "Create Product";
        window.Width   = 400;
        window.Height  = 800;
        window.Content = createPage;
        window.Show(); //This means it opens the window on top of the main window
    }
    
    private void OnEditClick(object sender, RoutedEventArgs e)
    {
        EditProductPage editPage = new EditProductPage();
        editPage.PitPage = this;

        Window window = new Window();
        window.Title   = "Edit Product";
        window.Width   = 400;
        window.Height  = 800;
        window.Content = editPage;
        window.Show();
    }
    
    private void OnExportClick(object sender, RoutedEventArgs e)
    {
        ExportProductPage exportPage = new ExportProductPage(); //We "Create" the CreateProdduct page and saves a reference back to this PIT page
        exportPage.PitPage = this;

        Window window = new Window(); //This creates the new window
        window.Title   = "Create Product";
        window.Width   = 400;
        window.Height  = 800;
        window.Content = exportPage;
        window.Show(); //This means it opens the window on top of the main window
    }

    // This method runs when the "Set Inactive" button is clicked on a row in the table.
    // It finds which product the clicked button belongs to, sets that product as
    // inactive in the database, and then reloads the table so the change is reflected.
    private void OnDeactivateClick(object sender, RoutedEventArgs e)
    { 
        foreach (PIM.Data.Product product in Products)
        {
            if (product.IsSelected)
            {
                setProductActiveService.SetInactive(product.Id);
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
                if (product.IsActive)
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

/*
private void OnLoadClick(object sender, RoutedEventArgs e)
{
    LoadProducts();
}
*/