using System;
using Microsoft.Maui.Controls;

using FRC_App.Models;

namespace FRC_App
{
    public partial class EditAccountInfoPage : ContentPage
    {
        public User currentUser  {get; private set; }

        public EditAccountInfoPage(User user)
        {
            InitializeComponent();
            currentUser = user;
        }

        // Save button clicked event handler
        private async void OnSaveButtonClicked(object sender, EventArgs e)
        {
            string username = usernameEntry.Text;
            string email = emailEntry.Text;
            string password = passwordEntry.Text;
            string confirmPassword = confirmPasswordEntry.Text;

            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
            {
                await DisplayAlert("Error", "All fields must be filled out.", "OK");
                return;
            }

            if (password != confirmPassword)
            {
                await DisplayAlert("Error", "Passwords do not match.", "OK");
                return;
            }

            // Logic to update account information (e.g., database or API call)

            await DisplayAlert("Success", "Account information updated.", "OK");
            await Navigation.PopAsync(); // Go back to the previous page
        }

        // Cancel button clicked event handler
        private async void OnCancelButtonClicked(object sender, EventArgs e)
        {
            await Navigation.PopAsync(); // Go back to the previous page
        }
    }
}
