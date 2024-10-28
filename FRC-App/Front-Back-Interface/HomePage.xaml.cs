using FRC_App.Models;
using FRC_App.Services;
using Microcharts;
using Microcharts.Maui;
using SkiaSharp;
using PdfSharpCore.Drawing;
using PdfSharpCore.Pdf;
using System.Formats.Asn1;

namespace FRC_App;

public partial class HomePage : ContentPage
{
	public User currentUser { get; private set; }
	public DataContainer userData { get; private set; }
	public ChartEntry[] motorEntry;
	public ChartEntry[] sensorEntry;
	public ChartEntry[] controlSystemEntry;
	public List<ChartView> chartViews { get; set; }
	public List<Label> chartLabels;
	public int numPlots;
	

	public HomePage(User user)
	{
		InitializeComponent();
		currentUser = user;
		BindingContext = currentUser;

		chartViews = new List<ChartView>
        {
            chartView1,
            chartView2,
            chartView3,
            chartView4,
            chartView5,
            chartView6
        };

		chartLabels = new List<Label>
		{
			chart1label,
            chart2label,
            chart3label,
            chart4label,
            chart5label,
            chart6label
		};
		numPlots = 0;

		loadUserPreferences();
	}

	private async void AddPlot(object sender, EventArgs e) {

		if (numPlots >= 6) {
			await DisplayAlert("Error", "Max num of plots is 6!", "OK");
			return;
		}

		bool hasData = !string.IsNullOrEmpty(currentUser.rawData);

		if (!hasData) {
			await DisplayAlert("Error", "You have no data to display.", "OK");
			return;
		} 

		userData = new DataContainer(currentUser);
		TypesDropDown.ItemsSource = userData.getDataTypeNames();
		TypesStack.IsVisible = true;
	}

	private async void SelectDataType(object sender, EventArgs e) {
		string selectedDataType = TypesDropDown.SelectedItem?.ToString();

		if (string.IsNullOrEmpty(selectedDataType)) {
			await DisplayAlert("Error", "Must Select a Data Type", "OK");
			return;
		}

		DataType dataType = userData.getDataType(selectedDataType);

		xDataDropDown.ItemsSource = dataType.getColumnLabels();
		yDataDropDown.ItemsSource = dataType.getColumnLabels();

		xDataDropDown.IsVisible = true;
		yDataDropDown.IsVisible = true;
		SelectXandYDataButton.IsVisible = true;
	}

	private async void SelectXandYData(object sender, EventArgs e) {
		string selectedDataType = TypesDropDown.SelectedItem?.ToString();

		string selectedX = xDataDropDown.SelectedItem?.ToString();
		string selectedY = yDataDropDown.SelectedItem?.ToString();

		if (string.IsNullOrEmpty(selectedX) || string.IsNullOrEmpty(selectedY) || string.IsNullOrEmpty(selectedDataType)) {
			await DisplayAlert("Error", "Must Select X and Y data", "OK");
			return;
		}

		DataType dataType = userData.getDataType(selectedDataType);
		Column columnX = dataType.getColumn(selectedX);
		Column columnY = dataType.getColumn(selectedY);

		Plot newPlot;
		try {
			newPlot = new Plot(columnX, columnY);
		} catch (AxesDifferentLengthsException) {
			await DisplayAlert("Error", "The x-axis must have the same number of elements as the y-axis", "OK");
			return;
		}

		renderNewPlot(newPlot);

		TypesStack.IsVisible = false;
		xDataDropDown.IsVisible = false;
		yDataDropDown.IsVisible = false;
		SelectXandYDataButton.IsVisible = false;
	}

	private void renderNewPlot (Plot newPlot) {
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

		List<string> visibleLabels = new List<string>{};
		foreach (Label chartLabel in chartLabels) {
			if (chartLabel.IsVisible) {
				visibleLabels.Add(chartLabel.Text);
			}
		}
		
		DeleteDropDown.ItemsSource = visibleLabels;
		DeleteStack.IsVisible = true;
	}

	private async void DeleteSelectedPlot(object sender, EventArgs e) {
		string selectedPlot = DeleteDropDown.SelectedItem?.ToString();

		if (string.IsNullOrEmpty(selectedPlot)) {
			await DisplayAlert("Error", "Must Select a plot to delete", "OK");
			return;
		}

		for (int i = 0; i < chartLabels.Count; i++) {
			var chartLabel = chartLabels[i];
			if (string.Equals(selectedPlot, chartLabel.Text)) {
				chartLabel.IsVisible = false;
				var chartView = chartViews[i];
				chartView.IsVisible = false;
				chartView.Chart = null;
				break;
			}
		}
		numPlots--;

		DeleteStack.IsVisible = false;
	}


	DataImport dataStructure;
	public List<List<List<double>>> rawData;

	private async void ImportData(object sender, EventArgs e)
	{
		await Navigation.PushAsync(new ImportData(currentUser));
		 
	}

	private async void LoadData(object sender, EventArgs e)
	{
		await DisplayAlert("Remember", "Changing functionality", "OK");
	}

	private async void ExportData(object sender, EventArgs e)
	{
		if (currentUser.rawData is null) {
			await DisplayAlert("Error", "No data to Export. Import data first.", "OK");
		} else {
			DataImport exportDataStructure = new DataImport(); //Constuctor override uses fake FRC data structure (should mimic what was imported)
			List<List<List<double>>> retrievedRawData = exportDataStructure.RetrieveRawData(currentUser); //Also reconstructs the dataStructure based on the retrieval


			//Run Data Storage Test cases:
			if (dataStructure == null)
			{
				// Generate Fake Test FRC Data to compare with retrieved stored data if no new data is imported this session:
        		dataStructure = new DataImport(); //Constuctor override uses fake FRC data structure
        		rawData = dataStructure.GenerateTestData();  //Testing FRC data (not real)
			}

			Console.WriteLine($"Retrieved Data:\nRunning Test Cases . . .");
			int i = 0;
			foreach (string type in exportDataStructure.dataTypes)
			{
				if (!type.Equals(dataStructure.dataTypes[i]))
				{
					Console.WriteLine($"{dataStructure.dataTypes[i]}: Data Types Storage Failure!");
				} else {
					Console.WriteLine($"{dataStructure.dataTypes[i]}: Data Types Storage Passed.");
				}
				i++;
			}

			i = 0;
			foreach (List<string> file in exportDataStructure.dataUnits)
			{
				int j = 0;
				int errors = 0;
				foreach (string unit in file) 
				{
					if (!unit.Equals(dataStructure.dataUnits[i][j]))
					{
						Console.WriteLine($"{dataStructure.dataTypes[i]}: Data Units Storage Failure!\nColumn: {j+1}");
						errors++;
					}
					j++;
				}
				if (errors == 0)
					Console.WriteLine($"{dataStructure.dataTypes[i]}: Data Units Storage Passed.");
				i++;
			}

			i = 0;
			foreach (List<List<double>> file in retrievedRawData)
			{
				int j = 0;
				int errors = 0;
				foreach (List<double> column in file) 
				{
					int k = 0;
					foreach (double x in column) 
					{
						if (x != rawData[i][j][k]) 
						{
							Console.WriteLine($"{dataStructure.dataTypes[i]}: Raw Data Storage Failure!\nColumn: {j+1}\nEntry: {k+1}");
							errors++;
						}
						k++;
					}
					j++;
				}
				if (errors == 0)
					Console.WriteLine($"{dataStructure.dataTypes[i]}: Raw Data Storage Passed.");
				i++;
			}


			DataExport export = new DataExport(dataStructure);
			export.ToCSV(retrievedRawData,"SampleDemo");  //(FileName should be prompted for not hardcoded)

			await DisplayAlert("Success", "Data Exported", "Continue"); 
		}
	}

	private async void Preference(object sender, EventArgs e)
	{
		await Navigation.PushAsync(new PreferencePage(currentUser));
	}

	public void loadUserPreferences() {
		string userThemeKey = $"{currentUser.Username}_{currentUser.TeamNumber}_theme";
		string userFontSizeKey = $"{currentUser.Username}_{currentUser.TeamNumber}_fontSize";
		string userLayoutKey = $"{currentUser.Username}_{currentUser.TeamNumber}_layoutStyle";

		string userTheme = Preferences.Get(userThemeKey, "Dark Theme");
		((App)Application.Current).LoadTheme(userTheme);

		string userFontSize = Preferences.Get(userFontSizeKey, "Medium");
		((App)Application.Current).SetAppFontSize(userFontSize);

		string userLayout = Preferences.Get(userLayoutKey, "Spacious");
		((App)Application.Current).SetAppLayoutStyle(userLayout);
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
		if (chartView1.Chart != null)
		{
			var chart = chartView1.Chart;
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
						var filePath = Path.Combine(desktopPath, "chart.jpeg");
						using (var stream = File.OpenWrite(filePath))
						{
							data.SaveTo(stream);
						}

						await DisplayAlert("Export Successful", $"JPEG saved to {filePath}", "OK");
					}
				}
			}
		}
		else
		{
			await DisplayAlert("No Data", "No chart data available to export.", "OK");
		}
	}

	private async void ExportToPdf(object sender, EventArgs e)
	{
		if (chartView1.Chart != null)
		{
			string desktopPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop); // may not work for mac

			using (var bitmap = new SKBitmap(600, 400))
			{
				using (var canvas = new SKCanvas(bitmap))
				{
					canvas.Clear(SKColors.White);
					chartView1.Chart.DrawContent(canvas, bitmap.Width, bitmap.Height);

					using (var image = SKImage.FromBitmap(bitmap))
					using (var data = image.Encode(SKEncodedImageFormat.Png, 100))
					{
						var pdfPath = Path.Combine(desktopPath, "chart.pdf");

						using (var pdf = new PdfDocument())
						{
							var pdfPage = pdf.AddPage();
							var graphics = XGraphics.FromPdfPage(pdfPage);

							using (var ms = new MemoryStream(data.ToArray()))
							using (var img = XImage.FromStream(() => new MemoryStream(ms.ToArray()))){
								// Scaling code to maintain aspect ratio
								double scaleFactor = Math.Min(pdfPage.Width / img.PixelWidth, pdfPage.Height / img.PixelHeight);
								double newWidth = img.PixelWidth * scaleFactor;
								double newHeight = img.PixelHeight * scaleFactor;

								double xPosition = (pdfPage.Width - newWidth) / 2;
								double yPosition = (pdfPage.Height - newHeight) / 2;

								graphics.DrawImage(img, xPosition, yPosition, newWidth, newHeight);
							}

							pdf.Save(pdfPath);
						}

						await DisplayAlert("Export Successful", $"PDF saved to {pdfPath}", "OK");
					}
				}
			}
		}
		else
		{
			await DisplayAlert("No Data", "No chart data available to export.", "OK");
		}
	}

	private async void RenderLineChart(object sender, EventArgs e){
		if (currentUser.rawData is null) {
			await DisplayAlert("Error", "No data to Visualize. Import data first.", "OK");
		} else {
			chartView1.Chart = new LineChart { Entries = motorEntry};
			chartView2.Chart = new LineChart { Entries = sensorEntry};
			chartView3.Chart = new LineChart { Entries = controlSystemEntry};
		}

		
	}
	private async void RenderPointChart(object sender, EventArgs e){
		if (currentUser.rawData is null) {
			await DisplayAlert("Error", "No data to Visualize. Import data first.", "OK");
		} else {
			chartView1.Chart = new PointChart { Entries = motorEntry};
			chartView2.Chart = new PointChart { Entries = sensorEntry};
			chartView3.Chart = new PointChart { Entries = controlSystemEntry};
		}
	}
	private async void RenderRadarChart(object sender, EventArgs e){
		if (currentUser.rawData is null) {
			await DisplayAlert("Error", "No data to Visualize. Import data first.", "OK");
		} else {
			chartView1.Chart = new RadarChart { Entries = motorEntry};
			chartView2.Chart = new RadarChart { Entries = sensorEntry};
			chartView3.Chart = new RadarChart { Entries = controlSystemEntry};
		}
	}
	
}