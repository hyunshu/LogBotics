using SQLite;

namespace FRC_App.Models
{
    public class User
    {   
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        public string Username { get; set; }
        public string Password { get; set; }

        // distinguish admin users
        public bool IsAdmin { get; set; }
    }
}