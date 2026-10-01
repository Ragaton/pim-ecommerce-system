using System;
using System.Collections.Generic;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Media;
using Common.IPIM.DTO;
using PIM.Services;

namespace PIM.Presentation;

public partial class EditProductPage : UserControl
{
    // Loads the services for loading products into the fields and updateing products
    private readonly LoadProductService   _loadProductService   = new LoadProductService();
    private readonly UpdateProductService _updateProductService = new UpdateProductService();

    // Holds the product data that was loaded when the user clicked Load Product
    private ProductRow? _loadedRow;

    // Tracks the original category id so we can detect if the user changed it
    private int _originalCategoryId;

    // Stores the attribute text boxes — key = attribute id, value = text box
    private Dictionary<int, TextBox> _attributeInputs = new Dictionary<int, TextBox>();

    // Set by PITPage when it opens this window so we can refresh the list after saving
    public PITPage? PitPage { get; set; }

    public EditProductPage()
    {
        InitializeComponent();
        LoadCategories();
    }

    // Fills the category dropdown with all product categories from the database
    private void LoadCategories()
    {
        CategoryBox.Items.Clear();

        List<CategoryItem> categories = _loadProductService.GetProductCategories();

        foreach (CategoryItem category in categories)
        {
            // We store the category id in Tag so we can retrieve it later without another database call
            ComboBoxItem item = new ComboBoxItem();
            item.Content = category.Name;
            item.Tag     = category.Id;

            CategoryBox.Items.Add(item);
        }
    }

    // Runs when the user clicks Load Product
    // Looks up the variant by id and fills in all the form fields
    private void OnLoadClick(object? sender, RoutedEventArgs e)
    {
        StatusText.Foreground = Brushes.Gray;
        FormPanel.IsVisible   = false;
        AttributesPanel.Children.Clear();
        _attributeInputs.Clear();

        if (!int.TryParse(VariantIdBox.Text, out int variantId))
        {
            StatusText.Text = "Please enter a valid numeric variant ID.";
            return;
        }

        _loadedRow = _loadProductService.GetProductRowById(variantId);

        if (_loadedRow == null)
        {
            StatusText.Text = $"No product found with variant ID {variantId}.";
            return;
        }

        // Fill in the simple fields
        ProductNameBox.Text = _loadedRow.ProductName;
        SkuBox.Text         = _loadedRow.Sku;
        PriceBox.Text       = _loadedRow.Price.ToString();
        BasePriceBox.Text   = _loadedRow.BasePrice.ToString();
        CostPriceBox.Text   = _loadedRow.CostPrice.ToString();

        // Find the category id for this product so we can pre-select it in the dropdown
        // and remember it so we can detect if the user changes it
        _originalCategoryId = GetCategoryIdByName(_loadedRow.CategoryName);

        // Pre-select the correct category in the dropdown
        foreach (ComboBoxItem item in CategoryBox.Items)
        {
            if ((int)item.Tag == _originalCategoryId)
            {
                CategoryBox.SelectedItem = item;
                break;
            }
        }

        // Build the attribute fields pre-filled with the current values
        LoadAttributeFieldsWithValues(variantId);

        StatusText.Text     = $"Loaded: {_loadedRow.ProductName} (Variant {_loadedRow.VariantId})";
        FormPanel.IsVisible = true;
    }

    // Helper to look up a category id by its name from the dropdown items
    private int GetCategoryIdByName(string categoryName)
    {
        foreach (ComboBoxItem item in CategoryBox.Items)
        {
            if (item.Content.ToString() == categoryName)
            {
                return (int)item.Tag;
            }
        }

        return 0;
    }

    // Builds the attribute text boxes pre-filled with the current saved values
    private void LoadAttributeFieldsWithValues(int variantId)
    {
        AttributesPanel.Children.Clear();
        _attributeInputs.Clear();

        List<(int AttributeId, string AttributeName, string Value, string Unit)> rawAttributes
            = _loadProductService.GetRawAttributesForVariant(variantId);

        foreach (var attr in rawAttributes)
        {
            string unitLabel = attr.Unit != "" ? $" ({attr.Unit})" : "";

            TextBlock label = new TextBlock();
            label.Text = attr.AttributeName + unitLabel;

            TextBox input = new TextBox();
            input.Text = attr.Value;

            AttributesPanel.Children.Add(label);
            AttributesPanel.Children.Add(input);

            _attributeInputs.Add(attr.AttributeId, input);
        }
    }

    // Runs when the user picks a different category in the dropdown
    // Clears the attribute fields and loads the new category's attributes empty
    private void OnCategoryChanged(object? sender, SelectionChangedEventArgs e)
    {
        if (CategoryBox.SelectedItem == null || _loadedRow == null)
        {
            return;
        }

        ComboBoxItem selectedItem = (ComboBoxItem)CategoryBox.SelectedItem;
        int selectedCategoryId = (int)selectedItem.Tag;

        // Only rebuild the attribute fields if the category actually changed
        if (selectedCategoryId == _originalCategoryId)
        {
            // Category is the same as original — reload with the saved values
            LoadAttributeFieldsWithValues(_loadedRow.VariantId);
        }
        else
        {
            // Category changed — show empty fields for the new category's attributes
            LoadAttributeFieldsEmpty(selectedCategoryId);
        }
    }

    // Builds empty attribute text boxes for a given category
    // Used when the user switches to a different category
    private void LoadAttributeFieldsEmpty(int categoryId)
    {
        AttributesPanel.Children.Clear();
        _attributeInputs.Clear();

        List<AttributeItem> attributes = _loadProductService.GetAttributesForCategory(categoryId);

        foreach (AttributeItem attribute in attributes)
        {
            string unitLabel = attribute.Unit != "" ? $" ({attribute.Unit})" : "";

            TextBlock label = new TextBlock();
            label.Text = attribute.Name + unitLabel;

            TextBox input = new TextBox();
            input.Watermark = "Enter " + attribute.Name + unitLabel;

            AttributesPanel.Children.Add(label);
            AttributesPanel.Children.Add(input);

            _attributeInputs.Add(attribute.Id, input);
        }
    }

    // Runs when the user clicks Save Changes
    // Validates the form and sends everything to the UpdateProductService
    private void OnSaveClick(object? sender, RoutedEventArgs e)
    {
        if (_loadedRow == null)
        {
            return;
        }

        string productName   = ProductNameBox.Text;
        string sku           = SkuBox.Text;
        string priceText     = PriceBox.Text;
        string basePriceText = BasePriceBox.Text;
        string costPriceText = CostPriceBox.Text;

        if (string.IsNullOrWhiteSpace(productName)) { StatusText.Text = "Please enter a product name."; return; }
        if (string.IsNullOrWhiteSpace(sku))          { StatusText.Text = "Please enter a SKU.";         return; }

        if (!decimal.TryParse(priceText,     out decimal price))     { StatusText.Text = "Please enter a valid price.";      return; }
        if (!decimal.TryParse(basePriceText, out decimal basePrice)) { StatusText.Text = "Please enter a valid base price."; return; }
        if (!decimal.TryParse(costPriceText, out decimal costPrice)) { StatusText.Text = "Please enter a valid cost price."; return; }

        // Collect all attribute values the user filled in
        Dictionary<int, string> attributeValues = new Dictionary<int, string>();

        foreach (KeyValuePair<int, TextBox> entry in _attributeInputs)
        {
            if (!string.IsNullOrWhiteSpace(entry.Value.Text))
            {
                attributeValues.Add(entry.Key, entry.Value.Text);
            }
        }

        // Check if the user changed the category
        ComboBoxItem selectedItem = (ComboBoxItem)CategoryBox.SelectedItem;
        int selectedCategoryId = (int)selectedItem.Tag;
        int? newCategoryId = selectedCategoryId != _originalCategoryId ? selectedCategoryId : null;

        try
        {
            _updateProductService.UpdateProduct(
                _loadedRow.VariantId,
                _loadedRow.ProductId,
                productName,
                sku,
                price,
                basePrice,
                costPrice,
                attributeValues,
                newCategoryId);

            StatusText.Foreground = Brushes.Green;
            StatusText.Text       = "Product updated successfully.";

            // Refresh the PIT page product list
            if (PitPage != null)
            {
                PitPage.LoadProducts();
            }
        }
        catch (Exception ex)
        {
            StatusText.Foreground = Brushes.Red;
            StatusText.Text       = "Error saving: " + ex.Message;
        }
    }
}