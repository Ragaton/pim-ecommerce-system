using System;
using System.Collections.Generic;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Media;
using PIM.Services;
using PIM.Presentation;
using PIM.Data;

namespace PIM;

public partial class DeleteProductPage : UserControl
{
    public DPITPage DPitPage { get; set; }
    
    private readonly List<PIM.Data.Product> _selectedProducts;
    private DeleteProductVariantService deleteProductService = new DeleteProductVariantService();

    public DeleteProductPage(List<PIM.Data.Product> selectedProducts)
    {
        InitializeComponent();
        _selectedProducts = selectedProducts;

        string idList = "";
        foreach (PIM.Data.Product p in selectedProducts)
        {
            if (idList != "") idList += ", ";
            idList += p.Id;
        }

        WarningText.Text = $"Are you sure you want to delete products with variant IDs: {idList}?";
    }

    private void OnDeleteClick(object sender, RoutedEventArgs e)
    {
        int deleted = 0;
        int failed  = 0;

        foreach (PIM.Data.Product p in _selectedProducts)
        {
            bool success = deleteProductService.DeleteVariant(p.Id);
            if (success) deleted++;
            else         failed++;
        }

        if (failed == 0)
        {
            StatusText.Foreground = Brushes.Green;
            StatusText.Text       = $"{deleted} product(s) deleted successfully.";
            DPitPage?.LoadProducts();

            Window window = (Window)this.VisualRoot;
            window.Close();
        }
        else
        {
            StatusText.Foreground = Brushes.Red;
            StatusText.Text       = $"{deleted} deleted, {failed} failed (may still be active).";
        }
    }
}