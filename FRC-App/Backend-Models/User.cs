using SQLite;

namespace FRC_App.Models
{
    public class User
    {
        public string TeamName { get; set; }
        public string TeamNumber { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public bool IsAdmin { get; set; }
    }
}