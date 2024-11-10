using FRC_App.Models;
using FRC_App.Services;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace FRC_App;

public partial class DataManipulationPage : ContentPage
{
    private User currentUser;
    private List<string> DataTypes;
    private List<List<string>> DataUnits;
    private List<int> DataValueNumbers;
    private string SelectedDataType;
    private string SelectedDataUnit;
    private int SelectedDataValueNumber;
    private double SelectedValue;
    private ObservableCollection<ValueItem> ValuesTableItems;

    public DataManipulationPage(User user)
    {
        InitializeComponent();
        currentUser = user;

        // Initialize data types and units from currentUser
        var dataImport = new DataImport();
        var rawData = dataImport.RetrieveRawData(currentUser);

        DataTypes = dataImport.dataTypes;
        DataUnits = dataImport.dataUnits;

        // Initialize ValuesTableItems as an empty collection
        ValuesTableItems = new ObservableCollection<ValueItem>();
        ValuesTable.ItemsSource = ValuesTableItems;

        // Set the initial values for pickers
        DataTypePicker.ItemsSource = DataTypes;
        SelectedDataType = DataTypes.FirstOrDefault();
        UpdateFilteredDataUnits();
    }

    private void OnDataTypeSelected(object sender, EventArgs e)
    {
        if (DataTypePicker.SelectedIndex != -1)
        {
            SelectedDataType = DataTypes[DataTypePicker.SelectedIndex];
            UpdateFilteredDataUnits();
        }
    }

    private void UpdateFilteredDataUnits()
    {
        int index = DataTypes.IndexOf(SelectedDataType);
        var filteredUnits = index >= 0 && index < DataUnits.Count ? DataUnits[index] : new List<string>();
        DataUnitPicker.ItemsSource = filteredUnits;
        SelectedDataUnit = filteredUnits.FirstOrDefault();
        UpdateDataValueNumbers();
    }

    private void OnDataUnitSelected(object sender, EventArgs e)
    {
        if (DataUnitPicker.SelectedIndex != -1)
        {
            SelectedDataUnit = DataUnitPicker.ItemsSource[DataUnitPicker.SelectedIndex] as string;
            UpdateDataValueNumbers();
        }
    }

    private void UpdateDataValueNumbers()
    {
        var entry = GetDataEntry(SelectedDataType, SelectedDataUnit);
        DataValueNumbers = entry != null ? Enumerable.Range(1, entry.Values.Count).ToList() : new List<int>();
        DataValuePicker.ItemsSource = DataValueNumbers;
        SelectedDataValueNumber = 1; // Reset to 1
        DataValuePicker.SelectedItem = SelectedDataValueNumber;
        UpdateSelectedValue();
        UpdateValuesTable(entry);
    }

    private void UpdateValuesTable(DataEntry entry)
    {
        ValuesTableItems.Clear();
        if (entry != null)
        {
            foreach (var (value, index) in entry.Values.Select((v, i) => (v, i)))
            {
                ValuesTableItems.Add(new ValueItem
                {
                    Order = index + 1,
                    Value = value
                });
            }
        }
    }

    private void OnDataValueSelected(object sender, EventArgs e)
    {
        if (DataValuePicker.SelectedIndex != -1)
        {
            SelectedDataValueNumber = DataValueNumbers[DataValuePicker.SelectedIndex];
            UpdateSelectedValue();
        }
    }

    private void UpdateSelectedValue()
    {
        var entry = GetDataEntry(SelectedDataType, SelectedDataUnit);
        if (entry != null && SelectedDataValueNumber - 1 < entry.Values.Count)
        {
            SelectedValue = double.TryParse(entry.Values[SelectedDataValueNumber - 1], out var value) ? value : 0;
            ValueSlider.Value = SelectedValue; // Sync Slider
            ValueEntry.Text = SelectedValue.ToString("F2"); // Sync Entry
            CurrentValueLabel.Text = $"Current Value: {SelectedValue:F2}";
        }
    }

    private void OnSliderValueChanged(object sender, ValueChangedEventArgs e)
    {
        // Sync Entry with Slider changes and update the ValuesTable dynamically
        SelectedValue = e.NewValue;
        ValueEntry.Text = SelectedValue.ToString("F2");
        CurrentValueLabel.Text = $"Current Value: {SelectedValue:F2}";
        UpdateTableValue();
    }

    private void OnValueEntryChanged(object sender, TextChangedEventArgs e)
    {
        // Sync Slider with Entry changes and update the ValuesTable dynamically
        if (double.TryParse(ValueEntry.Text, out var parsedValue) && SelectedValue != parsedValue)
        {
            SelectedValue = parsedValue;
            ValueSlider.Value = SelectedValue; // Sync Slider
            CurrentValueLabel.Text = $"Current Value: {SelectedValue:F2}";
            UpdateTableValue();
        }
    }

    private void UpdateTableValue()
    {
        // Update the corresponding item in ValuesTableItems
        if (SelectedDataValueNumber - 1 < ValuesTableItems.Count && SelectedDataValueNumber - 1 >= 0)
        {
            ValuesTableItems[SelectedDataValueNumber - 1].Value = SelectedValue.ToString("F2");
        }
    }

    private void OnSaveClicked(object sender, EventArgs e)
    {
        int typeIndex = DataTypes.IndexOf(SelectedDataType);
        int unitIndex = DataUnits[typeIndex].IndexOf(SelectedDataUnit);

        if (typeIndex != -1 && unitIndex != -1 && SelectedDataValueNumber - 1 >= 0)
        {
            var filesData = currentUser.rawData.Split("_", StringSplitOptions.RemoveEmptyEntries).ToList();
            var columnsData = filesData[typeIndex].Split(";", StringSplitOptions.RemoveEmptyEntries).ToList();
            var rowsData = columnsData[unitIndex].Split(",", StringSplitOptions.RemoveEmptyEntries).ToList();

            rowsData[SelectedDataValueNumber - 1] = SelectedValue.ToString("F2");

            columnsData[unitIndex] = string.Join(",", rowsData);
            filesData[typeIndex] = string.Join(";", columnsData);
            currentUser.rawData = string.Join("_", filesData);
            
            UserDatabase.UpdateUserAsync(currentUser);

            DisplayAlert("Save", "Value has been saved successfully.", "OK");
        }
    }

    private DataEntry GetDataEntry(string dataType, string dataUnit)
    {
        int typeIndex = DataTypes.IndexOf(dataType);
        if (typeIndex == -1) return null;

        int unitIndex = DataUnits[typeIndex].IndexOf(dataUnit);
        if (unitIndex == -1) return null;

        List<double> values = currentUser.rawData.Split("_", StringSplitOptions.RemoveEmptyEntries)
            .ElementAtOrDefault(typeIndex)?
            .Split(";", StringSplitOptions.RemoveEmptyEntries)
            .ElementAtOrDefault(unitIndex)?
            .Split(",", StringSplitOptions.RemoveEmptyEntries)
            .Select(val => double.Parse(val))
            .ToList() ?? new List<double>();

        return new DataEntry
        {
            Type = dataType,
            Unit = dataUnit,
            Values = new ObservableCollection<string>(values.Select(v => v.ToString("F2")))
        };
    }
}

public class DataEntry
{
    public string Type { get; set; }
    public string Unit { get; set; }
    public ObservableCollection<string> Values { get; set; }

    public DataEntry()
    {
        Values = new ObservableCollection<string>();
    }
}

public class ValueItem : INotifyPropertyChanged
{
    private string _value;

    public int Order { get; set; }

    public string Value
    {
        get => _value;
        set
        {
            if (_value != value)
            {
                _value = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Value)));
            }
        }
    }

    public event PropertyChangedEventHandler PropertyChanged;
}