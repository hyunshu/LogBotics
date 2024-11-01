using Microsoft.Maui.Controls;
using FRC_App.Models;
using SkiaSharp;
using SkiaSharp.Views.Maui;

namespace FRC_App;

public partial class MapPage : ContentPage
{
    private DataContainer dataContainer;
    private User currentUser;
    public bool isRenderMap = false;

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
        DisplayAlert("Accelerometer Data", "Please fill out the right panel with your Accelerometer data.", "OK");
        ButtonStack.IsVisible = true;
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
       
        isRenderMap = true;
        canvasView.InvalidateSurface();
    }


    private async void OnCanvasViewPaintSurface(object sender, SKPaintSurfaceEventArgs e)
        {

            if (isRenderMap) {

                string selectedDataType = TypesDropDown.SelectedItem?.ToString();
                string selectedTime = timeDataDropDown.SelectedItem?.ToString();
                string selectedX = xDataDropDown.SelectedItem?.ToString();
                string selectedY = yDataDropDown.SelectedItem?.ToString();
                
                DataType dataType = dataContainer.getDataType(selectedDataType);
                Column columnTime = dataType.getColumn(selectedTime);
                Column columnX = dataType.getColumn(selectedX);
                Column columnY = dataType.getColumn(selectedY);

                Map newMap;
                try {
                    newMap = new Map(columnTime, columnX, columnY);
                } catch (AxesDifferentLengthsException) {
                    await DisplayAlert("Error", "The x-axis must have the same number of elements as the y-axis", "OK");
                    isRenderMap = false;
                    return;
                }

                
                var canvas = e.Surface.Canvas;
                SKPath testPath = newMap.GeneratePath(e.Info);
                canvas.Clear(SKColors.Black);

    
                var paint = new SKPaint
                {
                    Style = SKPaintStyle.Stroke,
                    Color = SKColors.Red,
                    StrokeWidth = 5,
                    IsAntialias = true
                };

    
                canvas.DrawPath(testPath, paint);


                // var paint = new SKPaint
                // {
                //     Style = SKPaintStyle.Stroke,
                //     Color = SKColors.White,
                //     StrokeWidth = 5,
                //     IsAntialias = true
                // };

                // var info = e.Info;

                // float centerX = info.Width / 2;
                // float centerY = info.Height / 2;
                // float radius = Math.Min(info.Width, info.Height) / 3;

                // var path = new SKPath();
                // for (int i = 0; i < 5; i++)
                // {
                //     float angle = i * 144 * (float)Math.PI / 180;
                //     float x = centerX + radius * (float)Math.Cos(angle);
                //     float y = centerY - radius * (float)Math.Sin(angle);
                //     if (i == 0)
                //     {
                //         path.MoveTo(x, y);
                //     }
                //     else
                //     {
                //         path.LineTo(x, y);
                //     }
                // }
                // path.Close();

                // canvas.DrawPath(path, paint);

            }
            
			// var canvas = e.Surface.Canvas;
			// var info = e.Info;

			// canvas.Clear(SKColors.White);

			// var paint = new SKPaint
			// {
			// 	Style = SKPaintStyle.Stroke,
			// 	Color = SKColors.Black,
			// 	StrokeWidth = 5,
			// 	IsAntialias = true
			// };

			// float centerX = info.Width / 2;
			// float centerY = info.Height / 2;
			// float radius = Math.Min(info.Width, info.Height) / 3;

			// var path = new SKPath();
			// for (int i = 0; i < 5; i++)
			// {
			// 	float angle = i * 144 * (float)Math.PI / 180;
			// 	float x = centerX + radius * (float)Math.Cos(angle);
			// 	float y = centerY - radius * (float)Math.Sin(angle);
			// 	if (i == 0)
			// 	{
			// 		path.MoveTo(x, y);
			// 	}
			// 	else
			// 	{
			// 		path.LineTo(x, y);
			// 	}
			// }
			// path.Close();

			// canvas.DrawPath(path, paint);
        }
}
