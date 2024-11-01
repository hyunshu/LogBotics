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
       
        isRenderMap = true;
    }


    private void OnCanvasViewPaintSurface(object sender, SKPaintSurfaceEventArgs e)
        {
            canvasView.IsVisible = false;

            if (isRenderMap) {

                string selectedDataType = TypesDropDown.SelectedItem?.ToString();
                string selectedTime = timeDataDropDown.SelectedItem?.ToString();
                string selectedX = xDataDropDown.SelectedItem?.ToString();
                string selectedY = yDataDropDown.SelectedItem?.ToString();
                
                DataType dataType = dataContainer.getDataType(selectedDataType);
                Column columnTime = dataType.getColumn(selectedTime);
                Column columnX = dataType.getColumn(selectedX);
                Column columnY = dataType.getColumn(selectedY);

                Map newMap = new Map(columnTime, columnX, columnY);
                
                SKPoint[] testPath = newMap.GeneratePath();
                SKBitmap testGrid = newMap.GenerateGrid();

                // Draw the grid
                e.Surface.Canvas.DrawBitmap(testGrid, new SKRect(0, 0, e.Info.Width, e.Info.Height));

                // Draw the path
                var pathPaint = new SKPaint
                {
                    Style = SKPaintStyle.Stroke,
                    Color = SKColors.Red,
                    StrokeWidth = 2,
                    IsAntialias = true
                };

                var path = new SKPath();
                path.MoveTo(testPath[0]);
                for (int i = 1; i < testPath.Length; i++)
                {
                    path.LineTo(testPath[i]);
                }

                e.Surface.Canvas.DrawPath(path, pathPaint);

                canvasView.IsVisible = true;
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
