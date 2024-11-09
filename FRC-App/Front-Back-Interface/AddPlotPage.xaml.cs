using FRC_App.Models;
using System.Collections.ObjectModel;
using Microcharts.Maui;
using Microcharts;
using LiveChartsCore;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Drawing.Geometries;

namespace FRC_App;

public partial class AddPlotPage : ContentPage
{

	public User currentUser { get; private set; }
	public DataContainer userData { get; private set; }
	public ObservableCollection<ChartView> chartViews { get; set; }
	public ObservableCollection<Label> chartLabels { get; set; }
	public ObservableCollection<string> VisibleLabels { get; set; } 
	public Dictionary<string, Plot> plotDict { get; set; }
	public int numPlots;
	public AddPlotPage(User user)
	{
		InitializeComponent();
		currentUser = user;

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

	private async void ExportToJpeg(object sender, EventArgs e) {

	}

	private async void ExportToPdf(object sender, EventArgs e) {

	}
}