using System.Threading.Tasks;
using FRC_App.Models;
using FRC_App.Import;
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
            Console.WriteLine(databasePath);

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
                Password = password,
                IsAdmin = isAdmin  // Store the admin status
            };

            var id = await db.InsertAsync(user);
        }


        public static async Task storeData(User user, DataImport import, List<List<List<double>>> rawData)
        {
            await Init();
            
            import.StoreRawData(rawData,user);

            var id = await db.InsertAsync(user);
        }


        public static async Task<User> GetUser(string username)
        {
            await Init();
            var user = await db.Table<User>().Where(u => u.Username == username).FirstOrDefaultAsync();

            return user;
        }
    }

}