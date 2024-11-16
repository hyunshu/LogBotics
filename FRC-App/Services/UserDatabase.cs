using System.Data.Common;
using System.Threading.Tasks;
using FRC_App.Models;
using SQLite;
using Npgsql;

namespace FRC_App.Services
{
    public static class UserDatabase {

        static SQLiteAsyncConnection db;
        static string connectionString = "Host=100.69.42.127;Port=5432;Username=Client;Password=1234;Database=postgres";
        //static NpgsqlConnection db;
        static async Task Init()
        {   
            if (db != null) {
                return;
            }

            //string databasePath = Path.Combine(FileSystem.AppDataDirectory, "LogBoticsDatabase.db");  // Might need to change this to a diff directory in the future
            Console.WriteLine(connectionString);

            DatabaseInitializer dbInitializer = new DatabaseInitializer(connectionString);
            dbInitializer.InitializeDatabase();
            
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

            //try
            //{
                // Insert the user into the database
                using (var conn = new NpgsqlConnection(connectionString))
                {
                    conn.Open();

                    using (var cmd = new NpgsqlCommand("INSERT INTO Users (TeamName, TeamNumber, Username, Password, SecurityQuestion, SecurityAnswer, IsAdmin) VALUES (@TeamName, @TeamNumber, @Username, @Password, @SecurityQuestion, @SecurityAnswer, @IsAdmin) RETURNING Id", conn))
                    {
                        cmd.Parameters.AddWithValue("TeamName", user.TeamName);
                        cmd.Parameters.AddWithValue("TeamNumber", user.TeamNumber);
                        cmd.Parameters.AddWithValue("Username", user.Username);
                        cmd.Parameters.AddWithValue("Password", user.Password);
                        cmd.Parameters.AddWithValue("SecurityQuestion", user.SecurityQuestion);
                        cmd.Parameters.AddWithValue("SecurityAnswer", user.SecurityAnswer);
                        cmd.Parameters.AddWithValue("IsAdmin", user.IsAdmin);
                        cmd.Parameters.AddWithValue("sessions", "");
                        cmd.Parameters.AddWithValue("dataTypes", "");
                        cmd.Parameters.AddWithValue("dataUnits", "");
                        cmd.Parameters.AddWithValue("rawData", "");
                        
                        user.Id = (int)cmd.ExecuteScalar();
                    }
                }
            //}
            // catch (Exception ex)
            // {
            //     // Handle any errors during the insertion process
            //     Console.WriteLine($"Error inserting user: {ex.Message}");
            //     throw; // Re-throwing to make sure the caller is aware of the failure
            // }
        }

        public static void updateDatabase(User user) {

            using (var conn = new NpgsqlConnection(connectionString))
            {
                conn.Open();

                string updateQuery = @"
                    UPDATE Users
                    SET 
                        TeamName = @teamName,
                        TeamNumber = @teamNumber,
                        Username = @username,
                        Password = @password,
                        SecurityQuestion = @securityQuestion,
                        SecurityAnswer = @securityAnswer,
                        IsAdmin = @isAdmin,
                        sessions = @sessions,
                        dataTypes = @dataTypes,
                        dataUnits = @dataUnits,
                        rawData = @rawData
                    WHERE Id = @id;
                ";

                using (var cmd = new NpgsqlCommand(updateQuery, conn))
                {
                    // Add parameters to prevent SQL injection
                    cmd.Parameters.AddWithValue("id", user.Id);
                    cmd.Parameters.AddWithValue("teamName", user.TeamName ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("teamNumber", user.TeamNumber ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("username", user.Username ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("password", user.Password ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("securityQuestion", user.SecurityQuestion ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("securityAnswer", user.SecurityAnswer ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("isAdmin", user.IsAdmin);
                    cmd.Parameters.AddWithValue("sessions", user.sessions ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("dataTypes", user.dataTypes ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("dataUnits", user.dataUnits ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("rawData", user.rawData ?? (object)DBNull.Value);

                    // Execute the update command
                    int rowsAffected = cmd.ExecuteNonQuery();
                    if (rowsAffected == 0)
                    {
                        Console.WriteLine("No user found with the given ID.");
                    }
                }
            }
        }

        public static async Task clearData(User user)
        {
            await Init();
            
            user.sessions = "";
            user.dataTypes = "";
            user.dataUnits = "";
            user.rawData = "";

            updateDatabase(user);
        }


        public static async Task storeData(User user, DataImport import, List<List<List<double>>> rawData)
        {
            await Init();
            
            import.StoreRawData(rawData,user);

            updateDatabase(user);
        }


        public static async Task<User> GetUser(string username)
        {
            await Init();
            
            using (var conn = new NpgsqlConnection(connectionString))
            {
                conn.Open();

                string query = @"
                    SELECT Id, TeamName, TeamNumber, Username, Password, SecurityQuestion, SecurityAnswer, 
                        IsAdmin, sessions, dataTypes, dataUnits, rawData
                    FROM Users
                    WHERE Username = @username;
                ";

                using (var cmd = new NpgsqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("username", username);

                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.Read()) // Check if a row exists
                        {
                            return new User
                            {
                                Id = reader.GetInt32(reader.GetOrdinal("Id")),
                                TeamName = reader["TeamName"] as string,
                                TeamNumber = reader["TeamNumber"] as string,
                                Username = reader["Username"] as string,
                                Password = reader["Password"] as string,
                                SecurityQuestion = reader["SecurityQuestion"] as string,
                                SecurityAnswer = reader["SecurityAnswer"] as string,
                                IsAdmin = reader.GetBoolean(reader.GetOrdinal("IsAdmin")),
                                sessions = reader["sessions"] as string,
                                dataTypes = reader["dataTypes"] as string,
                                dataUnits = reader["dataUnits"] as string,
                                rawData = reader["rawData"] as string
                            };
                        }
                    }
                }
            }

            // If no user is found, return null
            return null;
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
            
            updateDatabase(user);
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
            
            updateDatabase(user);
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
            
            updateDatabase(user);
        }
        // update password
        public static async Task UpdatePassword(User user, string password)
        {
            await Init();
            user.Password = password;
            
            updateDatabase(user);
        }

        // check if team name exists
        public static async Task<bool> CheckTeamNameExistsAsync(string teamName)
        {
            await Init();
            
            using (var conn = new NpgsqlConnection(connectionString))
            {
                conn.Open();

                string query = @"
                    SELECT COUNT(*) 
                    FROM Users
                    WHERE TeamName = @teamName;
                ";

                using (var cmd = new NpgsqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("teamName", teamName);

                    // Execute the query and retrieve the count
                    long count = (long)cmd.ExecuteScalar();

                    // If count > 0, the TeamName exists
                    return count > 0;
                }
            }
        }
        // check if team number exists
        public static async Task<bool> CheckTeamNumberExistsAsync(string teamNumber)
        {
            await Init();
            
            using (var conn = new NpgsqlConnection(connectionString))
            {
                conn.Open();

                string query = @"
                    SELECT COUNT(*) 
                    FROM Users
                    WHERE TeamNumber = @teamNumber;
                ";

                using (var cmd = new NpgsqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("teamNumber", teamNumber);

                    // Execute the query and retrieve the count
                    long count = (long)cmd.ExecuteScalar();

                    // If count > 0, the TeamNumber exists
                    return count > 0;
                }
            }
        }
        // check if username exists
        public static async Task<bool> CheckUserNameExistsAsync(string username)
        {
            await Init();
            
            using (var conn = new NpgsqlConnection(connectionString))
            {
                conn.Open();

                string query = @"
                    SELECT COUNT(*) 
                    FROM Users
                    WHERE Username = @username;
                ";

                using (var cmd = new NpgsqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("username", username);

                    // Execute the query and retrieve the count
                    long count = (long)cmd.ExecuteScalar();

                    // If count > 0, the username exists
                    return count > 0;
                }
            }
        }


        // get user async (Unused)
        /*
        public static async Task<User> GetUserAsync(string username)
        {
            await Init();
            var user = await db.Table<User>().Where(u => u.Username == username).FirstOrDefaultAsync();

            return user;
        }
        */

        // update user async
        public static async Task UpdateUserAsync(User user)
        {
            await Init();
            
            updateDatabase(user);
        }
    }
}