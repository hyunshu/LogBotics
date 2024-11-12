using FRC_App.Models;
using System.Collections.ObjectModel;
using LiveChartsCore.SkiaSharpView.Maui;
using SkiaSharp;
using LiveChartsCore.SkiaSharpView.SKCharts;
using PdfSharpCore.Pdf;
using PdfSharpCore.Drawing;
using LiveChartsCore.SkiaSharpView.VisualElements;

namespace FRC_App;

public partial class AddPlotPage : ContentPage
{

	public User currentUser { get; private set; }
	public DataContainer userData { get; private set; }
	public Dictionary<string, Plot> plotDict { get; set; }
	public ObservableCollection<CartesianChart> chartCollection { get; set; }
	public ObservableCollection<string> chartTitles { get; set; } 
	public ObservableCollection<Grid> chartGrids { get; set; } 
	
	public int numPlots;
	public AddPlotPage(User user)
	{
		InitializeComponent();
		currentUser = user;
		plotDict = new Dictionary<string, Plot>{};
		chartCollection = new ObservableCollection<CartesianChart>
		{
			chart1,
			chart2,
			chart3,
			chart4,
			chart5,
			chart6
		};

		chartGrids = new ObservableCollection<Grid> 
		{
			ChartGrid1,
			ChartGrid2,
			ChartGrid3,
			ChartGrid4,
			ChartGrid5,
			ChartGrid6
		};
		numPlots = 0;
		BindingContext = this;
		chartTitles = new ObservableCollection<string>();
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

		if (newPlot.SameAxisCheck()) {
			await DisplayAlert("Warning", "You have selected the x-axis to be the same as the y-axis. This is an unusual request.", "OK");
		}

		// try {
		// 	plotDict.Add(newPlot.Title, newPlot);
		// } catch (ArgumentException) {
		// 	newPlot.Title = newPlot.Title + "(" + numPlots.ToString() + ")";
		// 	plotDict.Add(newPlot.Title, newPlot);
		// }
		renderNewPlot(newPlot);

		TypesStack.IsVisible = false;
	}

	private void renderNewPlot (Plot newPlot) {
		CartesianChart newChart = newPlot.GetScatterChart();

		for (int i = 0; i < chartCollection.Count; i++) {
			var currChart = chartCollection[i];
			var currGrid = chartGrids[i];
			if (currGrid.IsVisible == false) {
				currChart.Series = newChart.Series;
				currChart.Title = newChart.Title; 
				currGrid.IsVisible = true;
				break;
			}
		}
			
		numPlots++;
	}


	private async void OnDeleteChart1Clicked(object sender, EventArgs e) {
		chartGrids[0].IsVisible = false;
		numPlots--;
	}

	private async void OnDeleteChart2Clicked(object sender, EventArgs e) {
		chartGrids[1].IsVisible = false;
		numPlots--;
	}

	private async void OnDeleteChart3Clicked(object sender, EventArgs e) {
		chartGrids[2].IsVisible = false;
		numPlots--;
	}

	private async void OnDeleteChart4Clicked(object sender, EventArgs e) {
		chartGrids[3].IsVisible = false;
		numPlots--;
	}

	private async void OnDeleteChart5Clicked(object sender, EventArgs e) {
		chartGrids[4].IsVisible = false;
		numPlots--;
	}

	private async void OnDeleteChart6Clicked(object sender, EventArgs e) {
		chartGrids[5].IsVisible = false;
		numPlots--;
	}

	private async void ExportToJpeg(object sender, EventArgs e) {
		if (numPlots == 0) {
			await DisplayAlert("Error", "There are no plots to export.", "OK");
			return;
		}

		int exportCount = 0;
		for (int i = 0; i < chartCollection.Count; i++) {
			CartesianChart chart = chartCollection[i];
			if (chartGrids[i].IsVisible) {
				string desktopPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
				var skChart = new SKCartesianChart
				{
					Series = chart.Series,   
					Title = chart.Title,      
				};

				var filePath = Path.Combine(desktopPath, "chart" + exportCount.ToString() + ".jpeg");
				skChart.SaveImage(filePath);
				exportCount++;
			}
		}
	}

	private async void ExportToPdf(object sender, EventArgs e) {
    if (numPlots == 0) {
        await DisplayAlert("Error", "There are no plots to export.", "OK");
        return;
    }

    string desktopPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);

    using (var pdf = new PdfDocument()) {
        for (int i = 0; i < chartCollection.Count; i++) {
            CartesianChart chart = chartCollection[i];

            if (chartGrids[i].IsVisible) {
                var skChart = new SKCartesianChart
                {
                    Series = chart.Series,
                    Title = chart.Title,
                };

                // Increase bitmap size for higher resolution
                int width = 1200;  // Increase width
                int height = 800;  // Increase height

                using (var bitmap = new SKBitmap(width, height))
                using (var canvas = new SKCanvas(bitmap)) {
                    canvas.Clear(SKColors.White);

                    // Draw content and fit it entirely
                    skChart.DrawOnCanvas(canvas);

                    using (var image = SKImage.FromBitmap(bitmap))
                    using (var data = image.Encode(SKEncodedImageFormat.Png, 100)) {
                        // Add a new page to the PDF
                        var pdfPage = pdf.AddPage();
                        var graphics = XGraphics.FromPdfPage(pdfPage);

                        using (var ms = new MemoryStream(data.ToArray()))
                        using (var img = XImage.FromStream(() => new MemoryStream(ms.ToArray()))) {
                            // Calculate scaling to maintain aspect ratio
                            double pdfAspect = pdfPage.Width / pdfPage.Height;
                            double imageAspect = img.PixelWidth / (double)img.PixelHeight;

                            double newWidth, newHeight;
                            if (pdfAspect > imageAspect) {
                                // Fit by height
                                newHeight = pdfPage.Height;
                                newWidth = img.PixelWidth * (newHeight / img.PixelHeight);
                            } else {
                                // Fit by width
                                newWidth = pdfPage.Width;
                                newHeight = img.PixelHeight * (newWidth / img.PixelWidth);
                            }

                            double xPosition = (pdfPage.Width - newWidth) / 2;
                            double yPosition = (pdfPage.Height - newHeight) / 2;

                            // Draw the image on the PDF page
                            graphics.DrawImage(img, xPosition, yPosition, newWidth, newHeight);
                        }
                    }
                }
            }
        }

        var pdfPath = Path.Combine(desktopPath, "charts.pdf");
        pdf.Save(pdfPath);
        await DisplayAlert("Export Successful", $"PDF saved to {pdfPath}", "OK");
    }
}

}