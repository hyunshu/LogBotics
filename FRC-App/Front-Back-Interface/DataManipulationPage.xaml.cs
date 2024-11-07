using FRC_App.Models;
using FRC_App.Services;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace FRC_App;

public partial class DataManipulationPage : ContentPage
{
    public User currentUser { get; private set; }

    public DataManipulationPage(User user)
    {
        InitializeComponent();
        currentUser = user;
        BindingContext = new DataTableViewModel(currentUser); // Pass currentUser to the ViewModel
    }
}

public class DataTableViewModel : INotifyPropertyChanged
{
    public ObservableCollection<DataEntry> DataEntries { get; set; }
    public List<string> DataTypes { get; set; }
    public List<List<string>> DataUnits { get; set; }

    private string _selectedDataType;
    private string _selectedDataUnit;
    private DataEntry _selectedDataEntry;

    public string SelectedDataType
    {
        get => _selectedDataType;
        set
        {
            if (_selectedDataType != value)
            {
                _selectedDataType = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(SelectedDataType)));
                UpdateSelectedDataEntry();
            }
        }
    }

    public string SelectedDataUnit
    {
        get => _selectedDataUnit;
        set
        {
            if (_selectedDataUnit != value)
            {
                _selectedDataUnit = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(SelectedDataUnit)));
                UpdateSelectedDataEntry();
            }
        }
    }

    public double SelectedValue
    {
        get => double.TryParse(_selectedDataEntry?.Value, out var value) ? value : 0;
        set
        {
            if (_selectedDataEntry != null && double.TryParse(_selectedDataEntry.Value, out var currentValue) && currentValue != value)
            {
                _selectedDataEntry.Value = value.ToString("F2");
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(SelectedValue)));
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(DataEntries))); // To refresh CollectionView
            }
        }
    }

    // Default constructor for design-time support
    public DataTableViewModel()
    {
        DataEntries = new ObservableCollection<DataEntry>();
        DataTypes = new List<string>(); // Initialize to avoid null reference issues
        DataUnits = new List<List<string>>();
    }

    // Constructor that takes a User parameter
    public DataTableViewModel(User user) : this()
    {
        // Use DataImport to retrieve structured data
        DataImport dataImport = new DataImport();
        List<List<List<double>>> rawData = dataImport.RetrieveRawData(user);

        // Initialize DataTypes and DataUnits from dataImport
        DataTypes = dataImport.dataTypes;
        DataUnits = dataImport.dataUnits;

        // Set default selections
        SelectedDataType = DataTypes.FirstOrDefault();
        SelectedDataUnit = DataUnits.FirstOrDefault()?.FirstOrDefault();

        // Populate DataEntries based on rawData
        for (int i = 0; i < rawData.Count; i++)
        {
            for (int j = 0; j < rawData[i].Count; j++)
            {
                DataEntries.Add(new DataEntry
                {
                    Type = DataTypes[i],
                    Unit = i < DataUnits.Count && j < DataUnits[i].Count ? DataUnits[i][j] : string.Empty,
                    Value = rawData[i][j].FirstOrDefault().ToString("F2") // Displaying the first row in this example
                });
            }
        }

        UpdateSelectedDataEntry();
    }

    private void UpdateSelectedDataEntry()
    {
        _selectedDataEntry = DataEntries.FirstOrDefault(entry => entry.Type == SelectedDataType && entry.Unit == SelectedDataUnit);
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(SelectedValue)));
    }

    public event PropertyChangedEventHandler PropertyChanged;
}

public class DataEntry : INotifyPropertyChanged
{
    private string _value;

    public string Type { get; set; }
    public string Unit { get; set; }
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
