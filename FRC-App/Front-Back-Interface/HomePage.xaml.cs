using FRC_App.Models;
using FRC_App.Services;
using Microcharts;
using SkiaSharp;
using PdfSharpCore.Drawing;
using PdfSharpCore.Pdf;


namespace FRC_App;

public partial class HomePage : ContentPage
{
	public User currentUser { get; private set; }

	public ChartEntry[] motorEntry;
	public ChartEntry[] sensorEntry;
	public ChartEntry[] controlSystemEntry;
	

	public HomePage(User user)
	{
		InitializeComponent();
		currentUser = user;

		Console.WriteLine($"Logged in user: {currentUser.Username}");  // testing if user data was passed from loginPage

		BindingContext = currentUser;

		loadUserPreferences();
		loadUserData();
	}

	public void loadUserData() {
		bool hasData = !string.IsNullOrEmpty(currentUser.rawData);

		if (hasData) {
			DataImport dataStructure = new DataImport();
			List<List<List<double>>> rawData = dataStructure.RetrieveRawData(currentUser);
			int motorEntrySize = rawData[0][0].Count;
			int sensorEntrySize = rawData[1][0].Count;
			int controlSystemEntrySize = rawData[2][0].Count;
			
			motorEntry = new ChartEntry[motorEntrySize];
			sensorEntry = new ChartEntry[sensorEntrySize];
			controlSystemEntry = new ChartEntry[controlSystemEntrySize];

			for (int i = 0; i < motorEntrySize; i++)
			{
				motorEntry[i] = new ChartEntry((float)rawData[0][1][i])
				{
					Label = rawData[0][0][i].ToString(),  // time
					ValueLabel = rawData[0][1][i].ToString().Substring(0,5), // spin angle
					Color = SKColor.Parse("#3498db")
				};
			}
			for (int i = 0; i < sensorEntrySize; i++)
			{
				sensorEntry[i] = new ChartEntry((float)rawData[1][1][i])
				{
					Label = rawData[1][0][i].ToString(),  // time
					ValueLabel = rawData[1][1][i].ToString().Substring(0,5),  // measurement (ft)
					Color = SKColor.Parse("#77d065")
				};
			}
			for (int i = 0; i < controlSystemEntrySize; i++)
			{
				controlSystemEntry[i] = new ChartEntry((float)rawData[2][1][i])
				{
					Label = rawData[2][0][i].ToString(),  // time
					ValueLabel = rawData[2][1][i].ToString(),  // forward input
					Color = SKColor.Parse("#b455b6")
				};
			}
			chartView1.Chart = new LineChart { Entries = motorEntry};
			chartView2.Chart = new LineChart { Entries = sensorEntry};
			chartView3.Chart = new LineChart { Entries = controlSystemEntry};

			MotorDataLabel1.IsVisible = true;
			SensorDataLabel1.IsVisible = true;
			ControlSystemDataLabel1.IsVisible = true;

			chartView1.IsVisible = true;
			chartView2.IsVisible = true;
			chartView3.IsVisible = true;

		} else {
			DisplayAlert("No Data", "No data in your database.", "OK");
			MotorDataLabel1.IsVisible = false;
			SensorDataLabel1.IsVisible = false;
			ControlSystemDataLabel1.IsVisible = false;

			chartView1.IsVisible = false;
			chartView2.IsVisible = false;
			chartView3.IsVisible = false;
		}
	}



	DataImport dataStructure;
	public List<List<List<double>>> rawData;

	private async void ImportData(object sender, EventArgs e)
	{
		await Navigation.PushAsync(new ImportData(currentUser));
		 
	}

	private async void LoadData(object sender, EventArgs e)
	{
		loadUserData();
	}

	// private async void ImportFakeData(object sender, EventArgs e)
	// {
	// 	motorEntryTitle = "MotorData";
	// 	sensorEntryTitle = "SensorData";
	// 	constrolSystemEntryTitle = "ControlSystemData";
	// 	// Generate Fake Test FRC Data:
    //     dataStructure = new DataImport(); //Constuctor override uses fake FRC data structure
    //     rawData = dataStructure.GenerateTestData();  //Testing FRC data (not real)

	// 	await UserDatabase.storeData(currentUser,dataStructure,rawData);

	// 	Console.WriteLine($"Stored Data:\n{currentUser.dataTypes}");
	// 	Console.WriteLine($"{currentUser.dataUnits}");
	// 	Console.WriteLine($"{currentUser.rawData}");

	// 	loadUserData();
	// 	await DisplayAlert("Success", "Fake Data Created", "Continue"); 
	// }




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

		string userTheme = Preferences.Get(userThemeKey, "Dark Theme");
		((App)Application.Current).LoadTheme(userTheme);
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

	private async void AddPlot(object sender, EventArgs e){
		await DisplayAlert("Add Plot", "Feature not implemented yet.", "OK");
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