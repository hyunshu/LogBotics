using FRC_App.Models;
using FRC_App.Services;
using System.Collections.ObjectModel;
using System.Security.Cryptography.X509Certificates;
//using Microsoft.Maui.Controls.Compatibility.Platform.iOS; //Causes and error on windows

namespace FRC_App;

public partial class ManageDataSessionPage : ContentPage
{
    public User currentUser  { get; private set; }
    public DataContainer dataContainer { get; private set; }
    public List<string> sessionsNames { get; private set; }
    public string selectedSessionName { get; private set; }
    public Session selectedSession { get; private set; } 

	public ManageDataSessionPage()
	{
		InitializeComponent();
        currentUser = UserSession.CurrentUser;

        this.dataContainer = new DataContainer(currentUser);
        sessionsNames = dataContainer.getSessionNames();
        DataSessionPicker.ItemsSource = sessionsNames;
	}

    private async void OnDataSessionSelected(object sender, EventArgs e)
    {
        if (DataSessionPicker.SelectedIndex != -1)
        {
            selectedSessionName = sessionsNames[DataSessionPicker.SelectedIndex];
            selectedSession = dataContainer.getSession(selectedSessionName);
            DataSessionLabel.Text = selectedSessionName;
        }
    }
    
    private async void OnCreateSessionClicked(object sender, EventArgs e)
    {
        // Prompt the user to input a new name
        string newName = await DisplayPromptAsync(
            "Create a New Session",
            $"Enter a name for the new session:",
            placeholder: "New Session Name"
        );

        if (!string.IsNullOrEmpty(newName))
        {
            // Update the session name
            Session newSession = new Session();
            newSession.Name = newName;
            newSession.DataTypes = new List<DataType>{};
            List<Column> emptyCols = new List<Column>{};
            emptyCols.Add(new Column("NULL", new List<double> {0}));
            newSession.DataTypes.Add(new DataType("NULL",emptyCols));
            dataContainer.addSession(newSession);
            dataContainer.storeUpdates();

            Console.WriteLine($"Retrieved Data");
            Console.WriteLine($"Sessions:\n{currentUser.sessions}");
            Console.WriteLine($"Stored Data:\n{currentUser.dataTypes}");
            Console.WriteLine($"{currentUser.dataUnits}");
            Console.WriteLine($"{currentUser.rawData}");

            // Update the sessionsNames list and refresh the Picker
            sessionsNames = dataContainer.getSessionNames();
            DataSessionPicker.ItemsSource = null; // Force refresh
            DataSessionPicker.ItemsSource = sessionsNames;

            await DisplayAlert("Success", $"Session renamed to '{newName}'.", "OK");
        }
        else
        {
            await DisplayAlert("Error", "Session name cannot be empty.", "OK");
        }

    }

    private async void OnRenameSessionClicked(object sender, EventArgs e)
    {
        if (selectedSession == null)
        {
            await DisplayAlert("Error", "Please select a session to rename.", "OK");
            return;
        }

        // Prompt the user to input a new name
        string newName = await DisplayPromptAsync(
            "Rename Data Session",
            $"Enter a new name for session '{selectedSessionName}':",
            initialValue: selectedSessionName,
            placeholder: "New Session Name"
        );

        if (!string.IsNullOrEmpty(newName))
        {
            // Update the session name
            selectedSession.Name = newName;
            dataContainer.storeUpdates();

            // Update the sessionsNames list and refresh the Picker
            int index = sessionsNames.IndexOf(selectedSessionName);
            if (index != -1)
            {
                sessionsNames = dataContainer.getSessionNames();
                DataSessionPicker.ItemsSource = null; // Force refresh
                DataSessionPicker.ItemsSource = sessionsNames;
            }

            await DisplayAlert("Success", $"Session renamed to '{newName}'.", "OK");
        }
        else
        {
            await DisplayAlert("Error", "Session name cannot be empty.", "OK");
        }
    }

    private async void OnDuplicateSessionClicked(object sender, EventArgs e)
    {
        if (selectedSession == null)
        {
            await DisplayAlert("Error", "Please select a session to duplicate.", "OK");
            return;
        }

        // Prompt the user to input a new name
        string newName = await DisplayPromptAsync(
            "Duplicate Data Session",
            $"Enter a name for the duplicated session '{selectedSessionName}':",
            initialValue: selectedSessionName + " (Copy)",
            placeholder: "Duplicate Session Name"
        );

        if (!string.IsNullOrEmpty(newName))
        {
            // Duplicate the session
            Session newSession = selectedSession.Copy();
            newSession.Name = newName;
            dataContainer.addSession(newSession);
            dataContainer.storeUpdates();

            // Update the sessionsNames list and refresh the Picker
            sessionsNames = dataContainer.getSessionNames();
            DataSessionPicker.ItemsSource = null; // Force refresh
            DataSessionPicker.ItemsSource = sessionsNames;

            await DisplayAlert("Success", $"Session duplicated as '{newName}'.", "OK");
        }
        else
        {
            await DisplayAlert("Error", "Session name cannot be empty.", "OK");
        }
    }

    private async void OnDeleteSessionClicked(object sender, EventArgs e)
    {
        if (selectedSession == null)
        {
            await DisplayAlert("Error", "Please select a session to delete.", "OK");
            return;
        }

        bool delete = await DisplayAlert("Delete Data Session",
            $"Are you sure you want to delete the session '{selectedSessionName}'?",
            "Yes", "No");

        if (delete) {
            dataContainer.removeSession(selectedSession);
            dataContainer.storeUpdates();

            // Update the sessionsNames list and refresh the Picker
            sessionsNames = dataContainer.getSessionNames();
            DataSessionPicker.ItemsSource = null; // Force refresh
            DataSessionPicker.ItemsSource = sessionsNames;

            await DisplayAlert("Success", $"Session '{selectedSessionName}' deleted.", "OK");
        } else {
            await DisplayAlert("Cancelled", "Session deletion cancelled.", "OK");
        }
    }

    protected override void OnNavigatedTo(NavigatedToEventArgs args)
	{
		base.OnNavigatedTo(args);
		dataContainer = new DataContainer(currentUser);
        sessionsNames = dataContainer.getSessionNames();
        DataSessionPicker.ItemsSource = null; // Force refresh
        DataSessionPicker.ItemsSource = sessionsNames;
	}


}