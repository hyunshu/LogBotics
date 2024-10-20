using System.Threading.Tasks;
using FRC_App.Models;
using SQLite;

namespace FRC_App.Services
{
    public static class UserDatabase {

        static SQLiteAsyncConnection db;
        static async Task Init()
        {   
            if (db != null) {
                return;
            }

            string databasePath = Path.Combine(FileSystem.AppDataDirectory, "LogBoticsDatabase.db");  // Might need to change this to a diff directory in the future
            //Console.WriteLine(databasePath);
            db = new SQLiteAsyncConnection(databasePath);

            await db.CreateTableAsync<User>();
        }

        public static async Task AddUser(string teamName, string teamNumber, string name, string password, bool isAdmin = false)
        {
            await Init();
            var user = new User
            {
                TeamName = teamName,
                TeamNumber = teamNumber,
                Username = name,
                Password = password, // You may want to hash the password before storing it
                IsAdmin = isAdmin // Store the admin status
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


        public static async Task storeData(User user, DataImport import, List<List<List<double>>> rawData)
        {
            await Init();
            
            import.StoreRawData(rawData,user);

            await db.UpdateAsync(user);
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
            var existingUser = await db.Table<User>().Where(u => u.Username == username).FirstOrDefaultAsync();
            return existingUser != null;
        }
    }

}