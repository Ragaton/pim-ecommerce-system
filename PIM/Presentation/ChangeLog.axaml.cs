using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Avalonia.Controls;
using Avalonia.Interactivity;
using PIM.Services;

namespace PIM;

public partial class ChangeLog : UserControl
{
    // All entries loaded from the JSON files — kept in full so filtering does not require re-reading files
    private List<ChangeLogEntry> _allEntries = new List<ChangeLogEntry>();

    // The filtered list the DataGrid binds to
    public ObservableCollection<ChangeLogEntry> Entries { get; set; } = new ObservableCollection<ChangeLogEntry>();

    private bool _initialized = false;
    
    public ChangeLog()
    {
        InitializeComponent();
        DataContext = this;
        _initialized = true;  // only set after InitializeComponent finishes
        LoadEntries();
    }

    // Reads all JSON entries from the Log/entries folder and loads them into the grid
    private void LoadEntries()
    {
        _allEntries = ChangeLogService.ReadAllEntries(); // Already sorted newest first by the service
        ApplyFilter();
    }

    // Filters the displayed entries based on the selected operation type in the dropdown
    private void ApplyFilter()
    {
        if (!_initialized || _allEntries == null || Entries == null)
            return;

        Entries.Clear();

        ComboBoxItem? selectedItem = FilterBox.SelectedItem as ComboBoxItem;
        string filter = selectedItem?.Content?.ToString() ?? "All";

        foreach (ChangeLogEntry entry in _allEntries)
        {
            if (filter == "All" || entry.OperationType == filter)
                Entries.Add(entry);
        }
    }

    // Called when the user changes the filter dropdown selection
    private void OnFilterChanged(object sender, SelectionChangedEventArgs e)
    {
        ApplyFilter();
    }

    // Called when the user clicks Refresh — re-reads the JSON files in case new entries were added
    private void OnRefreshClick(object sender, RoutedEventArgs e)
    {
        LoadEntries();
    }
}