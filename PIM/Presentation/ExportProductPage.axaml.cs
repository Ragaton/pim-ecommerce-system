using System;
using System.Collections.Generic;
using System.IO;
using Avalonia.Controls;
using Avalonia.Interactivity;
using PIM.Domain;
using PIM.Presentation;
using PIM.Services;

namespace PIM;

public partial class ExportProductPage : UserControl
{
    public PITPage PitPage { get; set; }

    private ExportToJson      exportToJson      = new ExportToJson();
    private LoadProductService loadProductService = new LoadProductService();

    public ExportProductPage()
    {
        InitializeComponent();
        LoadCategories();
        CategoryPanel.IsVisible = false;
        VariantPanel.IsVisible  = true;
    }

    // Fills the category dropdown with all product categories from the database
    private void LoadCategories()
    {
        CategoryBox.Items.Clear();

        try
        {
            List<CategoryItem> categories = loadProductService.GetProductCategories();

            foreach (CategoryItem category in categories)
            {
                ComboBoxItem item = new ComboBoxItem();
                item.Content = category.Name;
                item.Tag     = category.Id;

                CategoryBox.Items.Add(item);
            }

            StatusText.Text = "Loaded " + CategoryBox.Items.Count + " categories.";
        }
        catch (Exception ex)
        {
            StatusText.Text = "Error loading categories: " + ex.Message;
        }
    }

    // Runs when the user switches between the two radio buttons
    // Shows or hides the correct input panel depending on the selected mode
    

    private void OnModeChanged(object sender, RoutedEventArgs e)
    {
        if (ModeVariant.IsChecked == true)
        {
            VariantPanel.IsVisible  = true;
            CategoryPanel.IsVisible = false;
        }
        else
        {
            VariantPanel.IsVisible  = false;
            CategoryPanel.IsVisible = true;
        }
    }

    // Runs when the user clicks Export to JSON
    // Decides which export method to call based on the selected mode
    private void OnExportClick(object sender, RoutedEventArgs e)
    {
        try
        {
            string exportDirectory = FindExportDirectory();

            if (ModeVariant.IsChecked == true)
            {
                ExportByVariant(exportDirectory);
            }
            else
            {
                ExportByCategory(exportDirectory);
            }
        }
        catch (Exception ex)
        {
            StatusText.Text = "Error: " + ex.Message;
        }
    }

    // Exports a single variant by its id
    private void ExportByVariant(string exportDirectory)
    {
        if (!int.TryParse(VariantIdBox.Text, out int variantId))
        {
            StatusText.Text = "Please enter a valid variant ID.";
            return;
        }

        string savedPath = exportToJson.ExportVariant(variantId, exportDirectory);

        if (savedPath == null)
        {
            StatusText.Text = "No variant found with ID " + variantId + ".";
            return;
        }

        StatusText.Text = "Exported to: " + savedPath;
    }

    // Exports all products in the selected category to one JSON file
    private void ExportByCategory(string exportDirectory)
    {
        if (CategoryBox.SelectedItem == null)
        {
            StatusText.Text = "Please select a category.";
            return;
        }

        ComboBoxItem selectedItem = (ComboBoxItem)CategoryBox.SelectedItem;
        int    categoryId   = (int)selectedItem.Tag;
        string categoryName = selectedItem.Content.ToString();

        string savedPath = exportToJson.ExportCategory(categoryId, categoryName, exportDirectory);

        if (savedPath == null)
        {
            StatusText.Text = "No products found in category: " + categoryName + ".";
            return;
        }

        StatusText.Text = "Exported to: " + savedPath;
    }

    // It will be saved in Bin/Debug/net9.0/
    private string FindExportDirectory()
    {
        string exportDirectory = Path.Combine(AppContext.BaseDirectory, "ExportedFiles");

        if (!Directory.Exists(exportDirectory))
        {
            Directory.CreateDirectory(exportDirectory);
        }

        return exportDirectory;
    }
}