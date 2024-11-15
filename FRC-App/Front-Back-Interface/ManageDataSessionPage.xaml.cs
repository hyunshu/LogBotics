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
    public List<string> sessions { get; private set; }
    public Session sessionData { get; private set; } 

	public ManageDataSessionPage(User user)
	{
		InitializeComponent();
        currentUser = user;

        DataContainer dataContainer = new DataContainer(currentUser);
        sessions = dataContainer.getSessionNames();
        DataSessionPicker.ItemsSource = sessions;
	}

    private void OnDataSessionSelected(object sender, EventArgs e)
    {
        if (DataSessionPicker.SelectedIndex != -1)
        {
            sessionData = dataContainer.getSession(sessions[DataSessionPicker.SelectedIndex]);
        }
    }
    
    private void OnCreateSessionClicked(object sender, EventArgs e)
    {
        // Logic to create a new data session
    }

    private void OnRenameSessionClicked(object sender, EventArgs e)
    {
        // Logic to rename the selected data session
    }

    private void OnDuplicateSessionClicked(object sender, EventArgs e)
    {
        // Logic to duplicate the selected data session
    }

    private void OnDeleteSessionClicked(object sender, EventArgs e)
    {
        // Logic to delete the selected data session
    }
}