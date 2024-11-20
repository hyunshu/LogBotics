using FRC_App.Services;

namespace FRC_App
{
    public partial class ReportBugPage : ContentPage
    {
        private readonly BugReportService _bugReportService;

        public ReportBugPage()
        {
            InitializeComponent();
            _bugReportService = new BugReportService(new DeviceInfoProvider());
        }

        private async void SendBugReportAutomatically(object sender, EventArgs e)
        {
            string bugDescription = BugDescriptionEditor.Text;
            string result = await _bugReportService.SendBugReportAsync(bugDescription);

            await DisplayAlert("Bug Report", result, "OK");
        }
    }
}
