using Microsoft.Maui.Controls;
using FRC_App.Models;
using FRC_App.Services;

namespace FRC_App
{
    public partial class InstructionPage : ContentPage
    {
        public User currentUser { get; private set; }

        public InstructionPage(User user)
        {
            InitializeComponent();
            currentUser = user;
        }
    }
}
