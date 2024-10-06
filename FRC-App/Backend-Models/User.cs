using SQLite;
using FRC_App.Import;

namespace FRC_App.Models
{
    public class User
    {
        public string TeamName { get; set; }
        public string TeamNumber { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public bool IsAdmin { get; set; }

        //Below is for the user's complete set of data for their FRC mission:
        //May want to include an array of data in the future if a user does work on
        //multiple robots
        public DataImport DataStructure { get; set; }
        public List<List<List<double>>> rawData { get; set; }
    }
}