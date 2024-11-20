
using System.Collections.ObjectModel;
using FRC_App.Models;
using SkiaSharp;
using SkiaSharp.Views.Maui;

namespace FRC_App;

public partial class MapPage : ContentPage
{
    public DataContainer dataContainer { get; private set; }
    private Session sessionData;
    private User currentUser;
    public bool isRenderMap = false;
    public ObservableCollection<string> sessionNames { get; set; } 

    public MapPage()
    {
        InitializeComponent();
        this.currentUser = UserSession.CurrentUser;
        sessionNames = new ObservableCollection<string>();
        // this.sessionData = sessionData;
        // LoadDataTypes();
    }

    private void LoadDataTypes()
    {
        List<string> dataTypeNames = sessionData.getDataTypeNames();
        TypesDropDown.ItemsSource = dataTypeNames;
    }

     private async void AddAccelerometerDataToMap(object sender, EventArgs e)
    {

        bool hasData = !string.IsNullOrEmpty(currentUser.rawData);

		if (!hasData) {
			await DisplayAlert("Error", "You have no data to display.", "OK");
			return;
		} 

		bool noSession = this.sessionData == null;

		if (noSession) {
			await DisplayAlert("Error", "You have no session selected.", "OK");
			return;
		}
        //logic to add the data to the map
        await DisplayAlert("Accelerometer Data", "Please fill out the right panel with your Accelerometer data.", "OK");
        ButtonStack.IsVisible = true;
    }

    private void OnDataTypeSelected(object sender, EventArgs e)
        {
            string selectedDataType = TypesDropDown.SelectedItem as string;

            if (!string.IsNullOrEmpty(selectedDataType))
            {
                DataType selectedData = sessionData.getDataType(selectedDataType);
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
       
        isRenderMap = true;
        canvasView.InvalidateSurface();
    }

    protected override void OnNavigatedTo(NavigatedToEventArgs args)
	{
		base.OnNavigatedTo(args);

		dataContainer = new DataContainer(currentUser);

		sessionNames.Clear();
		foreach (string session in dataContainer.getSessionNames()) {
			sessionNames.Add(session);
		}
		DataSessionPicker.ItemsSource = null;
		DataSessionPicker.ItemsSource = sessionNames;

	}

	private async void OnLoadSessionClicked(object sender, EventArgs e)
	{
		bool hasData = !string.IsNullOrEmpty(currentUser.rawData);
		if (!hasData) {
			await DisplayAlert("Error", "You have no data to load.", "OK");
			return;
		} 

		sessionStack.IsVisible = true;
	}

	private void OnDataSessionSelected(object sender, EventArgs e)
	{
		if (DataSessionPicker.SelectedIndex != -1)
		{
			string selectedSession = DataSessionPicker.SelectedItem as string;
			if (!string.IsNullOrEmpty(selectedSession)) {
				changeSession(selectedSession);
			}
		}

		sessionStack.IsVisible = false;
	}

	private async void changeSession(string selectedSession) {
		bool hasData = !string.IsNullOrEmpty(currentUser.rawData);

		if (!hasData) {
			await DisplayAlert("Error", "You have no data to display.", "OK");
			return;
		} 

		if (this.sessionData != null) {
			bool confirmChange = await DisplayAlert(
				"Confirm Session Change", 
				"Are you sure you want to load a new session? You will lose your current progress.", 
				"Yes", 
				"No"
			);

			if (!confirmChange)
			{
				return;
			}
		}

		DataContainer dataContainer = new DataContainer(currentUser);
		this.sessionData = dataContainer.getSession(selectedSession);

		// remove the robots path from canvas

		await DisplayAlert("Success", "Session Loaded Successfully.", "OK");
	}


    private async void OnCanvasViewPaintSurface(object sender, SKPaintSurfaceEventArgs e)
        {

            if (isRenderMap) {

                string selectedDataType = TypesDropDown.SelectedItem?.ToString();
                string selectedTime = timeDataDropDown.SelectedItem?.ToString();
                string selectedX = xDataDropDown.SelectedItem?.ToString();
                string selectedY = yDataDropDown.SelectedItem?.ToString();
                
                DataType dataType = sessionData.getDataType(selectedDataType);
                Column columnTime = dataType.getColumn(selectedTime);
                Column columnX = dataType.getColumn(selectedX);
                Column columnY = dataType.getColumn(selectedY);

                Map newMap;
                try {
                    newMap = new Map(columnTime, columnX, columnY);
                } catch (AxesDifferentLengthsException) {
                    await DisplayAlert("Error", "The time, x-axis acceleration, and y-axis acceleration must have the same number of elements as each other for mapping.", "OK");
                    isRenderMap = false;
                    return;
                } catch (SameAxisException) {
                    await DisplayAlert("Error", "You cannot select 2 or more of time, xAcceleration, or yAcceleration to be the same as each other.", "OK");
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
