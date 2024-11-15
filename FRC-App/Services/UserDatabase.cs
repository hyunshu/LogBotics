using System.Data.Common;
using System.Threading.Tasks;
using FRC_App.Models;
using SQLite;
using Npgsql;

namespace FRC_App.Services
{
    public static class UserDatabase {

        static SQLiteAsyncConnection db;
        //static NpgsqlConnection db;
        static async Task Init()
        {   
            if (db != null) {
                return;
            }

            //string databasePath = Path.Combine(FileSystem.AppDataDirectory, "LogBoticsDatabase.db");  // Might need to change this to a diff directory in the future
            string connectionString = "host=10.186.94.82 port=5432 dbname=postgres user=postgres password=1234 connect_timeout=10 sslmode=prefer";
            Console.WriteLine(connectionString);

            DatabaseInitializer dbInitializer = new DatabaseInitializer(connectionString);
            dbInitializer.InitializeDatabase();
            
            string databasePath = "jgff";
            db = new SQLiteAsyncConnection(databasePath);

            await db.CreateTableAsync<User>();
        }

        public static async Task AddUser(string teamName, string teamNumber, string name, string password, string securityQuestion, string securityAnswer, bool isAdmin = false)
        {
            await Init();
            // cannot create a user with an existing team name, team number, or username
            if (await CheckTeamNameExistsAsync(teamName))
            {
                throw new ArgumentException("Team name already exists.");
            }
            if (await CheckTeamNumberExistsAsync(teamNumber))
            {
                throw new ArgumentException("Team number already exists.");
            }
            if (await CheckUserNameExistsAsync(name))
            {
                throw new ArgumentException("Username already exists.");
            }

            var user = new User
            {
                TeamName = teamName,
                TeamNumber = teamNumber,
                Username = name,
                Password = password,
                SecurityQuestion = securityQuestion,
                SecurityAnswer = securityAnswer,
                IsAdmin = isAdmin  // Store the admin status
            };

            try
            {
                // Insert the user into the database
                var id = await db.InsertAsync(user);
            }
            catch (Exception ex)
            {
                // Handle any errors during the insertion process
                Console.WriteLine($"Error inserting user: {ex.Message}");
                throw; // Re-throwing to make sure the caller is aware of the failure
            }
        }

        public static async Task clearData(User user)
        {
            await Init();
            
            user.sessions = "";
            user.dataTypes = "";
            user.dataUnits = "";
            user.rawData = "";

            db.UpdateAsync(user);
        }


        public static async Task storeData(User user, DataImport import, List<List<List<double>>> rawData)
        {
            await Init();
            
            import.StoreRawData(rawData,user);

            db.UpdateAsync(user);
        }


        public static async Task<User> GetUser(string username)
        {
            await Init();
            var user = await db.Table<User>().Where(u => u.Username == username).FirstOrDefaultAsync();

            return user;
        }

        public static async Task<bool> CheckUserExistsAsync(string username)
        {
            // Assuming you're using SQLite or similar, adjust this query to your actual DB structure
            await Init();
            var existingUser = await db.Table<User>().Where(u => u.Username == username).FirstOrDefaultAsync();
            return existingUser != null;
        }


        // update team name
        public static async Task UpdateTeamName(User user, string teamName)
        {
            await Init();
            //cannot create a user with an existing team name
            if (await CheckTeamNameExistsAsync(teamName))
            {
                throw new ArgumentException("Team name already exists.");
            }
            user.TeamName = teamName;
            await db.UpdateAsync(user);
        }
        // update team number
        public static async Task UpdateTeamNumber(User user, string teamNumber)
        {
            await Init();
            //cannot create a user with an existing team number
            if (await CheckTeamNumberExistsAsync(teamNumber))
            {
                throw new ArgumentException("Team number already exists.");
            }
            user.TeamNumber = teamNumber;
            await db.UpdateAsync(user);
        }
        // update username
        public static async Task UpdateUsername(User user, string username)
        {
            await Init();
            //cannot create a user with an existing username
            if (await CheckUserNameExistsAsync(username))
            {
                throw new ArgumentException("Username already exists.");
            }
            user.Username = username;
            await db.UpdateAsync(user);
        }
        // update password
        public static async Task UpdatePassword(User user, string password)
        {
            await Init();
            user.Password = password;
            await db.UpdateAsync(user);
        }

        // check if team name exists
        public static async Task<bool> CheckTeamNameExistsAsync(string teamName)
        {
            await Init();
            var existingTeamName = await db.Table<User>().Where(u => u.TeamName == teamName).FirstOrDefaultAsync();
            return existingTeamName != null;
        }
        // check if team number exists
        public static async Task<bool> CheckTeamNumberExistsAsync(string teamNumber)
        {
            await Init();
            var existingTeamNumber = await db.Table<User>().Where(u => u.TeamNumber == teamNumber).FirstOrDefaultAsync();
            return existingTeamNumber != null;
        }
        // check if username exists
        public static async Task<bool> CheckUserNameExistsAsync(string username)
        {
            await Init();
            var existingUsername = await db.Table<User>().Where(u => u.Username == username).FirstOrDefaultAsync();
            return existingUsername != null;
        }


        // get user async
        public static async Task<User> GetUserAsync(string username)
        {
            await Init();
            var user = await db.Table<User>().Where(u => u.Username == username).FirstOrDefaultAsync();

            return user;
        }

        // update user async
        public static async Task UpdateUserAsync(User user)
        {
            await Init();
            await db.UpdateAsync(user);
        }
    }
}