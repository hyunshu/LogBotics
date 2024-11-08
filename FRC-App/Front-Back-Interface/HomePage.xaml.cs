using FRC_App.Models;
using FRC_App.Services;
using Microcharts;
using Microcharts.Maui;
using SkiaSharp;
using PdfSharpCore.Drawing;
using PdfSharpCore.Pdf;
using System.Diagnostics;

using SkiaSharp.Views.Maui.Controls;
using SkiaSharp.Views.Maui;

using System.Collections.ObjectModel;
using System.Security.Cryptography.X509Certificates;
//using Microsoft.Maui.Controls.Compatibility.Platform.iOS; //Causes and error on windows

namespace FRC_App;

public partial class HomePage : ContentPage
{
	public User currentUser { get; private set; }
	public Session sessionData { get; private set; }
	public ObservableCollection<ChartView> chartViews { get; set; }
	public ObservableCollection<Label> chartLabels { get; set; }
	public ObservableCollection<string> VisibleLabels { get; set; } 
	public Dictionary<string, Plot> plotDict { get; set; }
	public int numPlots;
	

	public HomePage(User user)
	{
		InitializeComponent();
		currentUser = user;
		BindingContext = currentUser;

		chartViews = new ObservableCollection<ChartView>
        {
            chartView1,
            chartView2,
            chartView3,
            chartView4,
            chartView5,
            chartView6
        };

		chartLabels = new ObservableCollection<Label>
		{
			chart1label,
            chart2label,
            chart3label,
            chart4label,
            chart5label,
            chart6label
		};

		VisibleLabels = new ObservableCollection<string>();
		plotDict = new Dictionary<string, Plot>{};
		numPlots = 0;
		loadUserPreferences();
	}

	protected override void OnAppearing()
	{
		base.OnAppearing();
		UpdateChartColors();
		BindingContext = null;
		BindingContext = currentUser;
	}

	private async void AddPlot(object sender, EventArgs e) {

		if (numPlots >= 6) {
			await DisplayAlert("Error", "Max number of plots is 6!", "OK");
			return;
		}

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

		TypesDropDown.ItemsSource = this.sessionData.getDataTypeNames();
		TypesStack.IsVisible = true;
	}


	//Demo for front-end devs:
	private async void changeSession(object sender, EventArgs e) {
		bool hasData = !string.IsNullOrEmpty(currentUser.rawData);

		if (!hasData) {
			await DisplayAlert("Error", "You have no data to display.", "OK");
			return;
		} 

		//TODO: Needs dropdown implementation
		DataContainer dataContainer = new DataContainer(currentUser);
		List<string> sessions = dataContainer.getSessionNames();
		string sessionSelection = sessions[0]; //Chosen from a dropdown

		this.sessionData = dataContainer.getSession(sessionSelection);
	}


	private async void SelectDataType(object sender, EventArgs e) {
		string selectedDataType = TypesDropDown.SelectedItem?.ToString();

		if (string.IsNullOrEmpty(selectedDataType)) {
			await DisplayAlert("Error", "Must Select a Data Type", "OK");
			return;
		}

		DataType dataType = sessionData.getDataType(selectedDataType);

		xDataDropDown.ItemsSource = dataType.getColumnLabels();
		yDataDropDown.ItemsSource = dataType.getColumnLabels();
	}

	private async void SelectXandYData(object sender, EventArgs e) {
		string selectedDataType = TypesDropDown.SelectedItem?.ToString();

		string selectedX = xDataDropDown.SelectedItem?.ToString();
		string selectedY = yDataDropDown.SelectedItem?.ToString();

		if (string.IsNullOrEmpty(selectedX) || string.IsNullOrEmpty(selectedY) || string.IsNullOrEmpty(selectedDataType)) {
			await DisplayAlert("Error", "Must Select X and Y data", "OK");
			return;
		}

		DataType dataType = sessionData.getDataType(selectedDataType);
		Column columnX = dataType.getColumn(selectedX);
		Column columnY = dataType.getColumn(selectedY);

		Plot newPlot;
		try {
			newPlot = new Plot(columnX, columnY);
		} catch (AxesDifferentLengthsException) {
			await DisplayAlert("Error", "The x-axis must have the same number of elements as the y-axis", "OK");
			return;
		}

		if (newPlot.SameAxisCheck()) {
			await DisplayAlert("Warning", "You have selected the x-axis to be the same as the y-axis. This is an unusual request.", "OK");
		}

		try {
			plotDict.Add(newPlot.Title, newPlot);
		} catch (ArgumentException) {
			newPlot.Title = newPlot.Title + "(" + numPlots.ToString() + ")";
			plotDict.Add(newPlot.Title, newPlot);
		}
		renderNewPlot(newPlot);

		TypesStack.IsVisible = false;
	}

	private void renderNewPlot (Plot newPlot) {
		SetChartColors(newPlot);

		for (int i = 0; i < chartViews.Count; i++) {
			var chartView = chartViews[i];
			if (chartView.Chart == null) {
				chartView.Chart = new LineChart { Entries = newPlot.chart };
				var chartLabel = chartLabels[i];
				chartLabel.Text = newPlot.Title;

				chartLabel.IsVisible = true;
				chartView.IsVisible = true;
				break;
			}
		}
		numPlots++;
	}

	private async void DeletePlot(object sender, EventArgs e) {
		if (numPlots <= 0) {
			await DisplayAlert("Error", "No plots to delete.", "OK");
			return;
		}

		VisibleLabels.Clear();
		foreach (Label chartLabel in chartLabels) {
			if (chartLabel.IsVisible) {
				VisibleLabels.Add(chartLabel.Text);
			}
		}
		
		DeleteDropDown.ItemsSource = VisibleLabels;
		DeleteStack.IsVisible = true;
	}

	private async void DeleteSelectedPlot(object sender, EventArgs e) {
		string selectedPlot = DeleteDropDown.SelectedItem?.ToString();

		if (string.IsNullOrEmpty(selectedPlot)) {
			await DisplayAlert("Error", "Must select a plot to delete", "OK");
			return;
		}

		for (int i = 0; i < chartLabels.Count; i++) {
			var chartLabel = chartLabels[i];
			if (string.Equals(selectedPlot, chartLabel.Text)) {
				var chartView = chartViews[i];
				chartView.IsVisible = false;
				chartLabel.IsVisible = false;
				chartView.Chart = null;

				plotDict.Remove(chartLabel.Text);
				break;
			}
		}
		numPlots--;
		DeleteStack.IsVisible = false;
	}

	private async void ImportData(object sender, EventArgs e)
	{
		await Navigation.PushAsync(new ImportData(currentUser));
	}

	private async void ExportData(object sender, EventArgs e)
	{
		if (currentUser.rawData is null) {
			await DisplayAlert("Error", "No data to Export. Import data first.", "OK");
		} else if (this.sessionData is null) {
			await DisplayAlert("Error", "You have no session selected to Export.", "OK");
		} else {
			DataImport exportDataStructure = new DataImport(); //Constuctor override uses fake FRC data structure (should mimic what was imported)
			List<List<List<double>>> retrievedRawData = exportDataStructure.RetrieveRawData(currentUser,sessionData.Name); //Also reconstructs the dataStructure based on the retrieval
			
			//Testing 10/31/2024:
			//exportDataStructure = new DataImport();
			//retrievedRawData = exportDataStructure.GenerateTestData();
			//Testing 10/31/2024

			DataExport export = new DataExport(exportDataStructure);
			export.ToCSV(retrievedRawData,"SampleDemo");  //(FileName should be prompted for not hardcoded)

			await DisplayAlert("Success", "Data Exported", "Continue"); 
		}
	}

	private async void Preference(object sender, EventArgs e)
	{
		await Navigation.PushAsync(new PreferencePage(currentUser));
	}

	private async void Instruction(object sender, EventArgs e)
	{
		await Navigation.PushAsync(new InstructionPage(currentUser));
	}

	public void loadUserPreferences() {
		string userThemeKey = $"{currentUser.Username}_{currentUser.TeamNumber}_theme";

		string userTheme = Preferences.Get(userThemeKey, "Dark Theme");
		((App)Application.Current).LoadTheme(userTheme);

		UpdateChartColors();
	}

	public void UpdateChartColors() {
		string userKey = $"{currentUser.Username}_{currentUser.TeamNumber}_chartcolor";
		string chartColor =  Preferences.Get(userKey, "Default");

		string color = chartColor switch
		{
			"Black" => "#FF000000",
			"Red" => "#FFFF0000",
			"Green" => "#FF008000",
			"Blue" => "#FF0000FF",
			"Orange" => "#FFFFA500",
			"Purple" => "#FF800080",
			_ => "#FF000000" // Fallback
		};

		for (int i = 0; i < chartViews.Count; i++) {
			ChartView chartView = chartViews[i];
			Label chartLabel = chartLabels[i];
			if (chartView != null && chartLabel.IsVisible) {
				string key = chartLabel.Text;
				Plot plot = plotDict[key];			
				
				// update the color
				string plotColor = plot.getRandomHexColor();
				foreach (var entry in plot.chart) {
					if (string.Equals(chartColor, "Default")) {
						entry.Color = SKColor.Parse(plotColor);
					} else {
						Console.WriteLine(chartColor);
						entry.Color = SKColor.Parse(color);
					}
				}
				
				// Re-render the charts
				if (chartView.Chart is LineChart) {
					chartView.Chart = new LineChart {Entries = plot.chart };
				} else if (chartView.Chart is PointChart) {
					chartView.Chart = new PointChart {Entries = plot.chart };
				} else if (chartView.Chart is RadarChart) {
					chartView.Chart = new RadarChart {Entries = plot.chart };
				}
			}
		}
	}

	public void SetChartColors(Plot newPlot) {
		string userKey = $"{currentUser.Username}_{currentUser.TeamNumber}_chartcolor";
		string chartColor =  Preferences.Get(userKey, "Default");

		if (string.Equals(chartColor, "Default")) {
			return;
		}

		string color = chartColor switch
		{
			"Black" => "#FF000000",
			"Red" => "#FFFF0000",
			"Green" => "#FF008000",
			"Blue" => "#FF0000FF",
			"Orange" => "#FFFFA500",
			"Purple" => "#FF800080",
			_ => "#FF000000" // Fallback
		};

		// update the color
		foreach (var entry in newPlot.chart) {
			entry.Color = SKColor.Parse(color);
		}
	}

	private async void LogOut(object sender, EventArgs e)
	{
		bool answer = await DisplayAlert("Log Out", "Are you sure you want to log out?", "Yes", "No");
		if (answer)
		{
			((App)Application.Current).LoadTheme("Dark Theme");
			Application.Current.MainPage = new NavigationPage(new LoginPage());	
		}
	}

	private async void ExportToJpeg(object sender, EventArgs e)
	{
		if (numPlots == 0) {
			await DisplayAlert("Error", "There are no plots to export.", "OK");
			return;
		}

		int exportCount = 0;
		foreach (ChartView chartView in chartViews) {
			if (chartView.Chart != null) {
				var chart = chartView.Chart;
				string desktopPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);

				using (var bitmap = new SKBitmap(600, 400)) // set the width and height as needed
				{
					using (var canvas = new SKCanvas(bitmap))
					{
						canvas.Clear(SKColors.White);
						chart.DrawContent(canvas, bitmap.Width, bitmap.Height);

						using (var image = SKImage.FromBitmap(bitmap))
						using (var data = image.Encode(SKEncodedImageFormat.Jpeg, 100))
						{
							var filePath = Path.Combine(desktopPath, "chart" + exportCount.ToString() + ".jpeg");
							using (var stream = File.OpenWrite(filePath))
							{
								data.SaveTo(stream);
							}

							await DisplayAlert("Export Successful", $"JPEG saved to {filePath}", "OK");
						}
					}
				}
				exportCount++;
			}
		}
	}

	private async void ExportToPdf(object sender, EventArgs e)
	{

		if (numPlots == 0) {
			await DisplayAlert("Error", "There are no plots to export.", "OK");
			return;
		}

		string desktopPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);

		// Create a new PDF document
		using (var pdf = new PdfDocument())
		{
			// Loop over each chart you want to export
			foreach (var chartView in chartViews)
			{
				if (chartView.Chart == null) continue; 

				// Create a bitmap for each chart
				using (var bitmap = new SKBitmap(600, 400))
				{
					using (var canvas = new SKCanvas(bitmap))
					{
						canvas.Clear(SKColors.White);
						chartView.Chart.DrawContent(canvas, bitmap.Width, bitmap.Height);

						using (var image = SKImage.FromBitmap(bitmap))
						using (var data = image.Encode(SKEncodedImageFormat.Png, 100))
						{
							// Add a new page to the PDF
							var pdfPage = pdf.AddPage();
							var graphics = XGraphics.FromPdfPage(pdfPage);

							using (var ms = new MemoryStream(data.ToArray()))
							using (var img = XImage.FromStream(() => new MemoryStream(ms.ToArray())))
							{
								// Scaling to maintain aspect ratio
								double scaleFactor = Math.Min(pdfPage.Width / img.PixelWidth, pdfPage.Height / img.PixelHeight);
								double newWidth = img.PixelWidth * scaleFactor;
								double newHeight = img.PixelHeight * scaleFactor;

								double xPosition = (pdfPage.Width - newWidth) / 2;
								double yPosition = (pdfPage.Height - newHeight) / 2;

								graphics.DrawImage(img, xPosition, yPosition, newWidth, newHeight);
							}
						}
					}
				}
			}

			// Save the PDF document to the desktop
			var pdfPath = Path.Combine(desktopPath, "charts.pdf");
			pdf.Save(pdfPath);

			await DisplayAlert("Export Successful", $"PDF saved to {pdfPath}", "OK");
		}
	}


	private async void RenderLineChart(object sender, EventArgs e){
		if (numPlots == 0) {
			await DisplayAlert("Error", "There are no plots to visualize.", "OK");
			return;
		}

		if (currentUser.rawData is null) {
			await DisplayAlert("Error", "No data to visualize. Import data first.", "OK");
			return;
		}

		for (int i = 0; i < chartViews.Count; i++) {
			ChartView chartView = chartViews[i];
			Label chartLabel = chartLabels[i];
			if (chartView != null && chartLabel.IsVisible) {
				string key = chartLabel.Text;
				Plot plot = plotDict[key];			
				chartView.Chart = new LineChart {Entries = plot.chart };
			}
		}
	}

	private async void RenderPointChart(object sender, EventArgs e){
		if (numPlots == 0) {
			await DisplayAlert("Error", "There are no plots to visualize.", "OK");
			return;
		}

		if (currentUser.rawData is null) {
			await DisplayAlert("Error", "No data to visualize. Import data first.", "OK");
			return;
		}

		for (int i = 0; i < chartViews.Count; i++) {
			ChartView chartView = chartViews[i];
			Label chartLabel = chartLabels[i];
			if (chartView != null && chartLabel.IsVisible) {
				string key = chartLabel.Text;
				Plot plot = plotDict[key];			
				chartView.Chart = new PointChart {Entries = plot.chart };
			}
		}
	}

	private async void RenderRadarChart(object sender, EventArgs e){
		if (numPlots == 0) {
			await DisplayAlert("Error", "There are no plots to visualize.", "OK");
			return;
		}

		if (currentUser.rawData is null) {
			await DisplayAlert("Error", "No data to visualize. Import data first.", "OK");
			return;
		}

		for (int i = 0; i < chartViews.Count; i++) {
			ChartView chartView = chartViews[i];
			Label chartLabel = chartLabels[i];
			if (chartView != null && chartLabel.IsVisible) {
				string key = chartLabel.Text;
				Plot plot = plotDict[key];			
				chartView.Chart = new RadarChart {Entries = plot.chart };
			}
		}
	}

/*	private async void LaunchNetworkTablesClient(object sender, EventArgs e)
{
    try
    {
        await Task.Run(() =>
        {
            Console.WriteLine("Attempting to launch NetworkTables client...");
            
            var processInfo = new ProcessStartInfo
            {
                FileName = @"C:\tools\dart-sdk\bin\dart.exe",
                Arguments = "run ./networkTablesClient.dart",
                WorkingDirectory = @"C:\Users\Jenna\Documents\GitHub\LogBotics\networkTables",
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };

            var process = Process.Start(processInfo);
            if (process != null)
            {
                using (var outputReader = process.StandardOutput)
                {
                    string result = outputReader.ReadToEnd();
                    Console.WriteLine(result);
                }

                using (var errorReader = process.StandardError)
                {
                    string errorResult = errorReader.ReadToEnd();
                    if (!string.IsNullOrEmpty(errorResult))
                    {
                        Console.WriteLine($"Error: {errorResult}");
                    }
                }

                process.WaitForExit();
            }
            else
            {
                Console.WriteLine("Failed to start the process.");
            }
        });
    }
    catch (Exception ex)
    {
        await DisplayAlert("Error", $"Failed to launch NetworkTables client: {ex.Message}", "OK");
    }
} 
*/

private async void RunNetworkTablesClient(object sender, EventArgs e)
{
	DataImport dataStructure = new DataImport();
	string directoryPath = "../";
	string fileName = "RealRobotData.txt";
	List<List<List<double>>> rawData = dataStructure.FromRobot(directoryPath, fileName);



	//TODO: Need popup to name the session of data imported from the Robot
	string sessionName = "textboxEntry";



	await UserDatabase.storeData(currentUser,dataStructure,rawData,sessionName);

	Console.WriteLine($"Stored Data:\n{currentUser.dataTypes}");
	Console.WriteLine($"{currentUser.dataUnits}");
	Console.WriteLine($"{currentUser.rawData}");

	await DisplayAlert("Success", "Data Recieved from Robot", "Continue");

	/* Doesn't work on every machine as of 10/31/2024 - James Gilliam
    try
    {
        string dartExePath = @"C:\tools\dart-sdk\bin\dart.exe"; // Updated Dart executable path
        string scriptPath = @"..\networkTables\networkTablesClientToFile.dart";

        ProcessStartInfo startInfo = new ProcessStartInfo
        {
            FileName = dartExePath,
            Arguments = $"\"{scriptPath}\"",
            UseShellExecute = false,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            CreateNoWindow = true
        };

        using (Process process = Process.Start(startInfo))
        {
            string output = await process.StandardOutput.ReadToEndAsync();
            string error = await process.StandardError.ReadToEndAsync();

            Dispatcher.Dispatch(() =>
            {
                if (!string.IsNullOrEmpty(output))
                {
                    OutputLabel.Text = $"Output: {output}";
                }
                else if (!string.IsNullOrEmpty(error))
                {
                    OutputLabel.Text = $"Error: {error}";
                }
                else
                {
                    OutputLabel.Text = "NetworkTables Client executed with no output.";
                }
            });

            process.WaitForExit();
        }

        await DisplayAlert("Success", "NetworkTables Client script executed successfully.", "OK");
    }
    catch (Exception ex)
    {
        await DisplayAlert("Error", $"An error occurred: {ex.Message}", "OK");
    }
	*/
}

private async void OpenMapPage(object sender, EventArgs e)
{
	if (currentUser.rawData is null) {
		await DisplayAlert("Error", "You have no data to display.", "OK");
		return;
	} else if (this.sessionData is null) {
		await DisplayAlert("Error", "You have no session selected.", "OK");
		return;
	} else {
    await Navigation.PushAsync(new MapPage(currentUser,this.sessionData));
	}
}
	
}