using Npgsql;

public class DatabaseInitializer
{
    private readonly string connectionString;

    public DatabaseInitializer(string connectionString)
    {
        this.connectionString = connectionString;
    }

    public void InitializeDatabase()
    {
        using (var conn = new NpgsqlConnection(this.connectionString))
        {
            conn.Open();

            string createTableQuery = @"
                CREATE TABLE IF NOT EXISTS Users (
                    Id SERIAL PRIMARY KEY,
                    TeamName VARCHAR(128),
                    TeamNumber VARCHAR(128),
                    Username VARCHAR(128),
                    Password VARCHAR(128),
                    SecurityQuestion VARCHAR(256),
                    SecurityAnswer VARCHAR(512),
                    IsAdmin BOOLEAN DEFAULT FALSE,
                    sessions VARCHAR(1048576),
                    dataTypes VARCHAR(1048576),
                    dataUnits VARCHAR(1048576),
                    rawData VARCHAR(1048576)
                );";

            using (var cmd = new NpgsqlCommand(createTableQuery, conn))
            {
                cmd.ExecuteNonQuery();
            }
        }
    }
}
