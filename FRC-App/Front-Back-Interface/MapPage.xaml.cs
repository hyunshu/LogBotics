using Microsoft.Maui.Controls;
using System.ComponentModel;
using FRC_App.Models;
using FRC_App.Services;

namespace FRC_App
{
    public partial class MapPage : ContentPage
    {
       private DataContainer dataContainer;
       public User currentUser { get; private set; }
        public MapPage(User user)
        {
            InitializeComponent();
            currentUser = user;
            dataContainer = new DataContainer(currentUser);
            LoadDataTypes();
        }
        
        private void LoadDataTypes()
        {
            List<string> dataTypes = dataContainer.getDataTypeNames();
            foreach (var dataType in dataTypes)
        {
            Button dataTypeButton = new Button
            {
                Text = dataType,
                BackgroundColor = Colors.White,
                TextColor = Colors.Black,
                Command = new Command(() => OnDataTypeButtonClicked(dataType))
            };
            DataTypeButtons.Children.Add(dataTypeButton);
        }
    }

    private void OnDataTypeButtonClicked(string dataType)
    {
        DataType selectedDataType = dataContainer.getDataType(dataType);
        if (selectedDataType != null)
        {
            List<string> columnLabels = selectedDataType.getColumnLabels();
            xDataDropDown.ItemsSource = columnLabels;
            yDataDropDown.ItemsSource = columnLabels;
        }
    }
    private void AddAccelerometerDataToMap(object sender, EventArgs e)
    {
        //add the accelerometer data to the map
        DisplayAlert("Accelerometer Data", "Accelerometer data added to the map.", "OK");
    }
    private async void OnLoadMapClicked(object sender, EventArgs e)
    {
     //add the logic for loading the map
        await DisplayAlert("Map Loading", "Loading map based on the selected data...", "OK");
        MapImage.IsVisible = true;
    }


    }
}
