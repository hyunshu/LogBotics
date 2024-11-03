using SQLite;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

using FRC_App.Services;

namespace FRC_App.Models
{
    public class User
    {
        private string _teamName;
        private string _teamNumber;
        private string _username;
        private string _password;

        // New integer primary key
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        public string TeamName
        {
            get => _teamName;
            set {
                _teamName = value;
            }
        }

        public string TeamNumber
        {
            get => _teamNumber;
            set
            {
                if (!int.TryParse(value, out int teamNum) || teamNum < 1 || teamNum > 99999)
                {
                    throw new ArgumentException("Team Number must be a valid number between 1 and 99999.");
                }
                _teamNumber = value;
            }
        }

        public string Username
        {
            get => _username;
            set {
                _username = value;
            }
        }

        public string Password
        {
            get => _password;
            set
            {
                if (value.Length < 4)
                {
                    throw new ArgumentException("Password must be at least 4 characters long.");
                }
                _password = value;
            }
        }

        public string SecurityQuestion { get; set; }
        public string SecurityAnswer { get; set; }
        public bool IsAdmin { get; set; }

        // Below is for the user's complete set of data for their FRC mission:
        public string dataTypes { get; set; }
        public string dataUnits { get; set; }
        public string rawData { get; set; }
    }
}