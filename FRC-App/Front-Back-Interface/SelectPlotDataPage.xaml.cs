namespace FRC_App;

public partial class SelectPlotDataPage : ContentPage
{
	public DataContainer currentUserData { get; set; }
	public SelectPlotDataPage(DataContainer data)
	{
		currentUserData = data;
		InitializeComponent(); 
	}
}