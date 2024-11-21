using FRC_App.Models;
using FRC_App.Services;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace FRC_App;

public partial class EditDataPage : ContentPage
{
    public User currentUser { get; private set; }
    public DataContainer dataContainer { get; private set; }
    public List<string> sessionsNames { get; private set; }
    public List<string> dataTypesNames { get; private set; }
    public List<string> dataUnitsNames { get; private set; }
    public Session SelectedSession { get; private set; }
    public DataType SelectedDataType { get; private set; }
    public Column SelectedDataUnit { get; private set; }
    public int SelectedValueIndex { get; private set; }
    public double SelectedValue { get; private set; }

    public EditDataPage()
    {
        InitializeComponent();
        currentUser = UserSession.CurrentUser;
        dataContainer = new DataContainer(currentUser);
        
        sessionsNames = dataContainer.getSessionNames();
        DataSessionPicker.ItemsSource = sessionsNames;
    }
    
    protected override void OnNavigatedTo(NavigatedToEventArgs args)
	{
		base.OnNavigatedTo(args);
        dataContainer = new DataContainer(currentUser);
        sessionsNames = dataContainer.getSessionNames();
        DataSessionPicker.ItemsSource = null; // Force refresh
        DataSessionPicker.ItemsSource = sessionsNames;
	}


    private void OnSliderValueChanged(object sender, ValueChangedEventArgs e)
    {
        SelectedValue = Math.Round(e.NewValue, 2);
        ValueSlider.Value = SelectedValue;
        ValueEntry.Text = SelectedValue.ToString("F2");
        CurrentValueLabel.Text = $"Current Value: {SelectedValue:F2}";
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
            }
        }
    }

    private void OnDataSessionSelected(object sender, EventArgs e) {
        if (DataSessionPicker.SelectedIndex != -1)
        {
            SelectedSession = dataContainer.getSession(sessionsNames[DataSessionPicker.SelectedIndex]);
            dataTypesNames = SelectedSession.getDataTypeNames();
            DataTypePicker.ItemsSource = dataTypesNames;
            DataTypePicker.SelectedIndex = -1;
            DataUnitPicker.ItemsSource = null;
            DataValuePicker.ItemsSource = null;
        }
    }

    private void OnDataTypeSelected(object sender, EventArgs e) {
        if (DataTypePicker.SelectedIndex != -1)
        {
            dataUnitsNames = SelectedSession.getDataType(dataTypesNames[DataTypePicker.SelectedIndex]).getColumnLabels();
            DataUnitPicker.ItemsSource = dataUnitsNames;
            DataUnitPicker.SelectedIndex = -1;
            DataValuePicker.ItemsSource = null;
        }
    }

    private void OnDataUnitSelected(object sender, EventArgs e)
    {
        if (DataUnitPicker.SelectedIndex != -1)
        {
            SelectedDataType = SelectedSession.getDataType(dataTypesNames[DataTypePicker.SelectedIndex]);
            SelectedDataUnit = SelectedDataType.getColumn(dataUnitsNames[DataUnitPicker.SelectedIndex]);

            // Check if the selected data unit is "Time (s)"
            if (dataUnitsNames[DataUnitPicker.SelectedIndex] == "Time (s)")
            {
                ValueSlider.Minimum = 0;
                ValueSlider.Maximum = 100;

                // Optionally reset the slider to a default value within the range
                ValueSlider.Value = 50; // Default to midpoint
                ValueEntry.Text = "50.00";
                CurrentValueLabel.Text = "Current Value: 50.00";
            }
            else if (SelectedDataUnit != null && SelectedDataUnit.Data.Count > 0)
            {
                // Dynamically update the slider range for other data units
                double minValue = SelectedDataUnit.Data.Min();
                double maxValue = SelectedDataUnit.Data.Max();

                ValueSlider.Minimum = minValue;
                ValueSlider.Maximum = maxValue;

                // Optionally reset the slider to the midpoint of the range
                double midpoint = (minValue + maxValue) / 2;
                ValueSlider.Value = midpoint;

                ValueEntry.Text = midpoint.ToString("F2");
                CurrentValueLabel.Text = $"Current Value: {midpoint:F2}";
            }

            // Populate the DataValuePicker
            DataValuePicker.ItemsSource = Enumerable.Range(0, SelectedDataUnit.Data.Count).ToList();
            DataValuePicker.SelectedIndex = -1;
        }
    }


    private void OnDataValueSelected(object sender, EventArgs e) {
        if (DataValuePicker.SelectedIndex != -1)
        {
            SelectedValueIndex = DataValuePicker.SelectedIndex;
            SelectedValue = SelectedDataUnit.Data[SelectedValueIndex];

            // Create the table items
            var tableItems = SelectedDataUnit.Data
                .Select((value, index) => new DataValueRow { Index = index, Value = value })
                .ToList();

            // Bind to the CollectionView
            ValuesTable.ItemsSource = tableItems;

            ValueSlider.Value = SelectedValue;
            ValueEntry.Text = SelectedValue.ToString("F2");
            CurrentValueLabel.Text = $"Current Value: {SelectedValue:F2}";
        }
    }

    private void OnSaveClicked(object sender, EventArgs e)
    {
        // Validate that all dropdowns have a selected item
        if (DataSessionPicker.SelectedIndex == -1)
        {
            DisplayAlert("Error", "Please select a Data Session.", "OK");
            return;
        }
        if (DataTypePicker.SelectedIndex == -1)
        {
            DisplayAlert("Error", "Please select a Data Type.", "OK");
            return;
        }
        if (DataUnitPicker.SelectedIndex == -1)
        {
            DisplayAlert("Error", "Please select a Data Unit.", "OK");
            return;
        }
        if (DataValuePicker.SelectedIndex == -1)
        {
            DisplayAlert("Error", "Please select a Data Value.", "OK");
            return;
        }

        // Ensure SelectedDataUnit and SelectedValueIndex are valid
        if (SelectedDataUnit != null && SelectedValueIndex >= 0)
        {
            // Update the value in the data unit
            SelectedDataUnit.Data[SelectedValueIndex] = SelectedValue;

            // Refresh the table data
            var tableItems = SelectedDataUnit.Data
                .Select((value, index) => new DataValueRow { Index = index, Value = value })
                .ToList();

            ValuesTable.ItemsSource = tableItems;

            // Persist changes to the data container
            dataContainer.storeUpdates();

            DisplayAlert("Success", "Data saved successfully.", "OK");
        }
        else
        {
            DisplayAlert("Error", "An unexpected error occurred while saving data.", "OK");
        }
    }
}

public class DataValueRow
{
    public int Index { get; set; }
    public double Value { get; set; }
}