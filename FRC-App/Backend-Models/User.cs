using SQLite;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FRC_App.Models
{
    public class User
    {
        [PrimaryKey] // Indicates this is the primary key
        public string TeamName { get; set; }
        public string TeamNumber { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string SecurityQuestion { get; set; }
        public string SecurityAnswer { get; set; }
        public bool IsAdmin { get; set; }

        //Below is for the user's complete set of data for their FRC mission:
        public string dataTypes { get; set; }
        public string dataUnits { get; set; }
        public string rawData { get; set; }
    }
}