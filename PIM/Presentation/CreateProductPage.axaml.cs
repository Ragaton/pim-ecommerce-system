using System;
using System.Collections.Generic;
using Avalonia.Controls;
using Avalonia.Interactivity;
using PIM.Services;
using PIM.Presentation;

namespace PIM;


public partial class CreateProductPage : UserControl
{
    public PITPage PitPage { get; set; } //A reference to the PIT page so we can go back after saveing a new product
    
    // We load the 2 services for reading products and saveing products
    private LoadProductService   loadProductService   = new LoadProductService();
    private CreateProductService createProductService = new CreateProductService();

    //This stores dynamicly created attribute boxes the key is the attribute id and the value is what is inside the textbox
    private Dictionary<int, TextBox> attributeInputs = new Dictionary<int, TextBox>();

    public CreateProductPage()
    {
        InitializeComponent();
        LoadCategories(); //This is a method call to fill the dropdown box when the page opens.
    }

    private void LoadCategories() //This gets all product product categories from the database and adds them to the dropdown.
    {
        CategoryBox.Items.Clear(); //This clears any "old" items incase the methods is called more than once

        try
        {
            List<CategoryItem> categories = loadProductService.GetProductCategories(); //Here we try to fetch all product categories from the database useing the methods in the data layer

            foreach (CategoryItem category in categories)
            { 
                ComboBoxItem item = new ComboBoxItem(); //We create the dropbox item for each category we got from the call
                item.Content = category.Name; //This is the name of the category
                item.Tag     = category.Id; // Tag is a kind of hidden storage space that we use to store the category id so we dont have to look it up in the database later

                CategoryBox.Items.Add(item);
            }
        }
        catch (Exception ex)
        {
            StatusText.Text = "Error loading categories: " + ex.Message;
        }
    }

    private void OnCategoryChanged(object sender, SelectionChangedEventArgs e) //This method is called everytime an item in the dropdown is clicked. This is also what we use to build the attribute fields
    {
        if (CategoryBox.SelectedItem == null) //If the user does not select an item it wil do nothing
        {
            return;
        }

        ComboBoxItem selectedItem = (ComboBoxItem)CategoryBox.SelectedItem; //This gets the selected item in the dropdown and the stores tag from earlier
        int categoryId = (int)selectedItem.Tag;

        LoadAttributes(categoryId); //This is a method call that will build the fields the user need to write the in so the attribute matches the category
    }

    private void LoadAttributes(int categoryId) //This is what build the attribut input fields dynamicly based on the selected category from earlier.
    {
        AttributesPanel.Children.Clear(); //Removes the old fields if the category was changed f.eks
        attributeInputs.Clear(); //Also cleares the dictionary so its fresh from earlier tries or creations.

        try
        {
            List<AttributeItem> attributes = loadProductService.GetAttributesForCategory(categoryId); //We use the data layer methods again to gett all the attributes from the selected category

            foreach (AttributeItem attribute in attributes)
            {
                string unitLabel = attribute.Unit != "" ? " (" + attribute.Unit + ")" : ""; //If the attribute has a unit it will add it in the paratheses and if it has no units it will leave it empty
                // here we create the Label for the shown attribute name (could be "Storage (GB)")
                TextBlock label = new TextBlock();
                label.Text = attribute.Name + unitLabel;

                // Creates the textbox where the user can put in the value for example storage
                TextBox input = new TextBox();
                input.Watermark = "Enter " + attribute.Name + unitLabel;
                 // theese 2 lines under adds the textboxes to the GUI 
                AttributesPanel.Children.Add(label);
                AttributesPanel.Children.Add(input);

                attributeInputs.Add(attribute.Id, input); //This stores the text box in our dictionay with the attribute id as the key. Its also what we loop through when the user clicks save
            }
        }
        catch (Exception ex)
        {
            StatusText.Text = "Error loading attributes: " + ex.Message;
        }
    }

    private void OnSaveClick(object sender, RoutedEventArgs e) // This method runs when the user clicks save
    {
        //Read the values the user types into the fields
        string productName   = ProductNameBox.Text;
        string sku           = SkuBox.Text;
        string priceText     = PriceBox.Text;
        string basePriceText = BasePriceBox.Text;
        string costPriceText = CostPriceBox.Text;

        
        //A bunch of if statements that checks if the field is correctly filled in
        if (string.IsNullOrWhiteSpace(productName))
        {
            StatusText.Text = "Please enter a product name.";
            return;
        }

        if (CategoryBox.SelectedItem == null)
        {
            StatusText.Text = "Please select a category.";
            return;
        }

        if (string.IsNullOrWhiteSpace(sku))
        {
            StatusText.Text = "Please enter a SKU.";
            return;
        }

        if (!decimal.TryParse(priceText, out decimal price)) //In price we could also add guards so it can not be negative values
        {
            StatusText.Text = "Please enter a valid price.";
            return;
        }

        if (!decimal.TryParse(basePriceText, out decimal basePrice)) //In price we could also add guards so it can not be negative values
        {
            StatusText.Text = "Please enter a valid base price.";
            return;
        }

        if (!decimal.TryParse(costPriceText, out decimal costPrice)) //In price we could also add guards so it can not be negative values
        {
            StatusText.Text = "Please enter a valid cost price.";
            return;
        }

        ComboBoxItem selectedItem = (ComboBoxItem)CategoryBox.SelectedItem; // This gets the category id from the selected item in the dropdown
        int categoryId = (int)selectedItem.Tag;

        // Loop thorugh all the attributes in the text boxes and collect the ones that was filled in (This does so attribute could be left empty) could be useful for laptops that has integrated graphics and therefor nu GPU
        Dictionary<int, string> attributeValues = new Dictionary<int, string>();

        foreach (KeyValuePair<int, TextBox> entry in attributeInputs)
        {
            if (!string.IsNullOrWhiteSpace(entry.Value.Text))
            {
                attributeValues.Add(entry.Key, entry.Value.Text);
            }
        }

        try
        {
            createProductService.CreateProduct( // Here we give all the data to the CreateProduct method in the datalayer
                productName,
                categoryId,
                sku,
                price,
                basePrice,
                costPrice,
                attributeValues);

            StatusText.Text = "Product saved!";

            if (PitPage != null) //This tells the PIT page to reload all the products when a new product is created so we dont have to click the reload button each time.
            {
                PitPage.LoadProducts();
            }

            Window window = (Window)this.VisualRoot;
            window.Close(); //Close the creation window
        }
        catch (Exception ex)
        {
            StatusText.Text = "Error: " + ex.Message;
        }
    }
}