using System;
using Microsoft.Maui.Controls;

using FRC_App.Services;
using FRC_App.Models;

namespace FRC_App
{
    public partial class EditAccountInfoPage : ContentPage
    {
        public User currentUser  { get; private set; }

        public EditAccountInfoPage(User user)
        {
            InitializeComponent();
            currentUser = user;
        }

        // Event handler for saving Team Name
        private async void OnSaveTeamNameClicked(object sender, EventArgs e)
        {
            string teamName = teamNameEntry.Text;
            string confirmTeamName = confirmTeamNameEntry.Text;

            if (string.IsNullOrEmpty(teamName) || string.IsNullOrEmpty(confirmTeamName))
            {
                DisplayAlert("Error", "Please enter and confirm your team name.", "OK");
                return;
            }

            if (teamName == confirmTeamName)
            {
                // Add logic to save team name
                try {
                    await UserDatabase.UpdateTeamName(currentUser, teamName);
                } catch (ArgumentException ex) {
                    DisplayAlert("Error", ex.Message, "OK");
                    return;
                }
                DisplayAlert("Success", "Team name saved successfully!", "OK");
            }
            else
            {
                DisplayAlert("Error", "Team names do not match.", "OK");
            }
        }

        // Event handler for saving Team Number
        private async void OnSaveTeamNumberClicked(object sender, EventArgs e)
        {
            string teamNumber = teamNumberEntry.Text;
            string confirmTeamNumber = confirmTeamNumberEntry.Text;

            if (string.IsNullOrEmpty(teamNumber) || string.IsNullOrEmpty(confirmTeamNumber))
            {
                DisplayAlert("Error", "Please enter and confirm your team number.", "OK");
                return;
            }

            if (teamNumber == confirmTeamNumber)
            {
                // Add logic to save team number
                try
                {
                    await UserDatabase.UpdateTeamNumber(currentUser, teamNumber);
                }
                catch (ArgumentException ex)
                {
                    DisplayAlert("Error", ex.Message, "OK");
                    return;
                }
                DisplayAlert("Success", "Team number saved successfully!", "OK");
            }
            else
            {
                DisplayAlert("Error", "Team numbers do not match.", "OK");
            }
        }

        // Event handler for saving Username
        private async void OnSaveUsernameClicked(object sender, EventArgs e)
        {
            string username = usernameEntry.Text;
            string confirmUsername = confirmUsernameEntry.Text;

            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(confirmUsername))
            {
                DisplayAlert("Error", "Please enter and confirm your username.", "OK");
                return;
            }

            if (username == confirmUsername)
            {
                // Add logic to save username
                try {
                    await UserDatabase.UpdateUsername(currentUser, username);
                } catch (ArgumentException ex) {
                    DisplayAlert("Error", ex.Message, "OK");
                    return;
                }
                DisplayAlert("Success", "Username saved successfully!", "OK");
            }
            else
            {
                DisplayAlert("Error", "Usernames do not match.", "OK");
            }
        }

        // Event handler for saving Password
        private async void OnSavePasswordClicked(object sender, EventArgs e)
        {
            string password = passwordEntry.Text;
            string confirmPassword = confirmPasswordEntry.Text;

            if (string.IsNullOrEmpty(password) || string.IsNullOrEmpty(confirmPassword))
            {
                DisplayAlert("Error", "Please enter and confirm your password.", "OK");
                return;
            }

            if (password == confirmPassword)
            {
                // Add logic to save password
                try {
                    await UserDatabase.UpdatePassword(currentUser, password);
                } catch (ArgumentException ex) {
                    DisplayAlert("Error", ex.Message, "OK");
                    return;
                }
                DisplayAlert("Success", "Password saved successfully!", "OK");
            }
            else
            {
                DisplayAlert("Error", "Passwords do not match.", "OK");
            }
        }

        // Event handler for Cancel button
        private void OnCancelButtonClicked(object sender, EventArgs e)
        {
            // Add logic to handle cancel action, such as navigating back
            Navigation.PopAsync();
        }
    }
}
