using Microsoft.Maui.Controls;
using FRC_App.Models;

namespace FRC_App;

public partial class MapPage : ContentPage
{
    private DataContainer dataContainer;
    private User currentUser;

    public MapPage(User user)
    {
        InitializeComponent();
        currentUser = user;
        dataContainer = new DataContainer(currentUser);
        LoadDataTypes();
    }

    private void LoadDataTypes()
    {
        List<string> dataTypeNames = dataContainer.getDataTypeNames();
        TypesDropDown.ItemsSource = dataTypeNames;
    }

    private void AddAccelerometerDataToMap(object sender, EventArgs e)
    {
        //logic to add the data to the map
        DisplayAlert("Accelerometer Data", "Accelerometer data added to the map.", "OK");
    }

    private void OnDataTypeSelected(object sender, EventArgs e)
        {
            string selectedDataType = TypesDropDown.SelectedItem as string;

            if (!string.IsNullOrEmpty(selectedDataType))
            {
                DataType selectedData = dataContainer.getDataType(selectedDataType);
                if (selectedData != null)
                {
                    List<string> columnLabels = selectedData.getColumnLabels();
                    xDataDropDown.ItemsSource = columnLabels;
                    yDataDropDown.ItemsSource = columnLabels;
                    timeDataDropDown.ItemsSource = columnLabels;
                }
            }
        }

    private async void OnLoadMapClicked(object sender, EventArgs e)
    {
        string selectedDataType = TypesDropDown.SelectedItem?.ToString();
        string selectedX = xDataDropDown.SelectedItem?.ToString();
        string selectedY = yDataDropDown.SelectedItem?.ToString();

        if (string.IsNullOrEmpty(selectedX) || string.IsNullOrEmpty(selectedY) || string.IsNullOrEmpty(selectedDataType))
        {
            await DisplayAlert("Error", "Must select X and Y data", "OK");
            return;
        }

        DataType dataType = dataContainer.getDataType(selectedDataType);
        Column columnX = dataType.getColumn(selectedX);
        Column columnY = dataType.getColumn(selectedY);

        if (columnX.Data.Count != columnY.Data.Count)
        {
            await DisplayAlert("Error", "The x-axis and y-axis must have the same number of elements for mapping.", "OK");
            return;
        }
        RenderMap(columnX, columnY);
    }

    private void RenderMap(Column xData, Column yData)
    {
        // Placeholder code to render the map
        MapImage.IsVisible = true;
    }
}
