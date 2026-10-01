using System;
using Avalonia.Controls;
using Avalonia.Interactivity;
using PIM.Presentation;

namespace PIM;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
        
        MainContent.Content = new TextBlock {Text = "Default page"};
    }
    
    private void PITButton(object sender, RoutedEventArgs e)
    {
        Console.WriteLine("PITButton");
        MainContent.Content = new PITPage();
    }

    private void DPITButton(object sender, RoutedEventArgs e)
    {
        Console.WriteLine("DPITButton");
        MainContent.Content = new DPITPage();
    }
    

    private void ChangeLogButton(object sender, RoutedEventArgs e)
    {
        Console.WriteLine("ChangeLogButton");
        MainContent.Content = new ChangeLog();
    }
}