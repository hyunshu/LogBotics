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
    public ObservableCollection<string> DataSessions { get; set; }
    private int SelectedSessionIndex;

    public DataManipulationPage(User user)
    {
        InitializeComponent();
        currentUser = user;

        // Initialize sessions and bind to the DataSessionPicker
        LoadSessions();
        DataSessionPicker.ItemsSource = DataSessions;

        // Initialize ValuesTableItems as an empty collection
        ValuesTableItems = new ObservableCollection<ValueItem>();
        ValuesTable.ItemsSource = ValuesTableItems;
    }

    private void LoadSessions()
    {
        // Parse sessions from the user object
        DataSessions = new ObservableCollection<string>(currentUser.sessions.Split('|'));
    }

    private void OnDataSessionSelected(object sender, EventArgs e)
    {
        if (DataSessionPicker.SelectedIndex != -1)
        {
            SelectedSessionIndex = DataSessionPicker.SelectedIndex;
            LoadSessionData();
        }
    }

    private void LoadSessionData()
    {
        // Load data types and units for the selected session
        DataTypes = currentUser.dataTypes.Split('|')[SelectedSessionIndex].Split('_').ToList();
        DataUnits = currentUser.dataUnits.Split('|')[SelectedSessionIndex]
                       .Split('_')
                       .Select(unitString => unitString.Split(';').ToList())
                       .ToList();

        // Update pickers with new data
        DataTypePicker.ItemsSource = DataTypes;
        DataTypePicker.SelectedIndex = -1;
        DataUnitPicker.ItemsSource = null;
        DataValuePicker.ItemsSource = null;

        // Clear table and current value display
        ValuesTableItems.Clear();
        CurrentValueLabel.Text = "Current Value: ";
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
        DataUnitPicker.SelectedIndex = -1;
        DataValuePicker.ItemsSource = null;
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
        SelectedDataValueNumber = 1;
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
            ValueSlider.Value = SelectedValue;
            ValueEntry.Text = SelectedValue.ToString("F2");
            CurrentValueLabel.Text = $"Current Value: {SelectedValue:F2}";
        }
    }

    private void OnSliderValueChanged(object sender, ValueChangedEventArgs e)
    {
        SelectedValue = e.NewValue;
        ValueEntry.Text = SelectedValue.ToString("F2");
        CurrentValueLabel.Text = $"Current Value: {SelectedValue:F2}";
        UpdateTableValue();
    }
    
    private void OnValueEntryChanged(object sender, TextChangedEventArgs e)
    {
        string input = ValueEntry.Text;
        if (System.Text.RegularExpressions.Regex.IsMatch(input, @"^\d+\.\d{2}$"))
        {
            if (double.TryParse(input, out var parsedValue) && SelectedValue != parsedValue)
            {
                SelectedValue = parsedValue;
                ValueSlider.Value = SelectedValue;
                CurrentValueLabel.Text = $"Current Value: {SelectedValue:F2}";
                UpdateTableValue();
            }
        }
    }


    private void UpdateTableValue()
    {
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
            var filesData = currentUser.rawData.Split("|", StringSplitOptions.RemoveEmptyEntries)[SelectedSessionIndex]
                            .Split("_", StringSplitOptions.RemoveEmptyEntries).ToList();
            var columnsData = filesData[typeIndex].Split(";", StringSplitOptions.RemoveEmptyEntries).ToList();
            var rowsData = columnsData[unitIndex].Split(",", StringSplitOptions.RemoveEmptyEntries).ToList();

            rowsData[SelectedDataValueNumber - 1] = SelectedValue.ToString("F2");

            columnsData[unitIndex] = string.Join(",", rowsData);
            filesData[typeIndex] = string.Join(";", columnsData);
            currentUser.rawData = string.Join("|", currentUser.rawData.Split("|", StringSplitOptions.RemoveEmptyEntries)
                                   .Select((data, index) => index == SelectedSessionIndex ? string.Join("_", filesData) : data));

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

        List<double> values = currentUser.rawData.Split("|", StringSplitOptions.RemoveEmptyEntries)[SelectedSessionIndex]
            .Split("_", StringSplitOptions.RemoveEmptyEntries)
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