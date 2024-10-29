using FRC_App.Models;
using FRC_App.Services;
using Microcharts;
using Microcharts.Maui;
using SkiaSharp;
using PdfSharpCore.Drawing;
using PdfSharpCore.Pdf;
using System.Collections.ObjectModel;

namespace FRC_App;

public partial class HomePage : ContentPage
{
	public User currentUser { get; private set; }
	public DataContainer userData { get; private set; }
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

		try {
			plotDict.Add(newPlot.Title, newPlot);
		} catch (ArgumentException) {
			newPlot.Title = newPlot.Title + "(" + numPlots.ToString() + ")";
			plotDict.Add(newPlot.Title, newPlot);
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
	
}