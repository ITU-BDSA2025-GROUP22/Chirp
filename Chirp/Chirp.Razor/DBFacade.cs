using Microsoft.Data.Sqlite;

namespace Chirp.Razor;

public class DBFacade
{
    private readonly string _connectionString;

    public DBFacade()
    {
        // 1. Try get env variable
        var dbPath = Environment.GetEnvironmentVariable("CHIRPDBPATH");

        // 2. If none found, use default in temp folder
        if (string.IsNullOrEmpty(dbPath))
        {
            var tempDir = Path.GetTempPath();
            dbPath = Path.Combine(tempDir, "chirp.db");
        }

        // 3. Build connection string
        _connectionString = $"Data Source={dbPath}";
    }

    public List<CheepViewModel> GetPagedCheeps(int page, int pageSize = 32)
    {
        var cheeps = new List<CheepViewModel>();
        int offset = (page - 1) * pageSize;
        var sqlQuery = @"SELECT u.username, m.text, m.pub_date 
                     FROM ""message"" m 
                     JOIN ""user"" u ON m.author_id = u.user_id 
                     ORDER BY m.pub_date DESC
                     LIMIT @pageSize OFFSET @offset";
        using (var connection = new SqliteConnection(_connectionString))
        {
            connection.Open();

            var command = connection.CreateCommand();
            command.CommandText = sqlQuery;
            command.Parameters.AddWithValue("@pageSize", pageSize);
            command.Parameters.AddWithValue("@offset", offset);

            using var reader = command.ExecuteReader();
            while (reader.Read())
            {
                var author = reader.GetString(0);
                var message = reader.GetString(1);
                var timestamp = reader.GetInt64(2);
                var cheep = new CheepViewModel(author, message, UnixTimeStampToDateTimeString(timestamp));
                cheeps.Add(cheep);
            }
        }
        return cheeps;
    }
    
    public List<CheepViewModel> GetAllCheeps()
    {
        var cheeps = new List<CheepViewModel>();
        
        var sqlQuery = @"SELECT u.username, m.text, m.pub_date FROM 
                                          ""message"" m JOIN ""user"" u ON m.author_id = u.user_id";
        
        using (var connection = new SqliteConnection(_connectionString))
        {
            connection.Open();

            var command = connection.CreateCommand();
            command.CommandText = sqlQuery;

            using var reader = command.ExecuteReader();
            while (reader.Read())
            {
                var author = reader.GetString(0);
                var message = reader.GetString(1);
                var timestamp = reader.GetInt64(2);
                var cheep = new CheepViewModel(author, message, UnixTimeStampToDateTimeString(timestamp));
                cheeps.Add(cheep);
            }
        }
        return cheeps;
    }
    
    public List<CheepViewModel> GetCheepsByAuthor(string author)
    {
        string sqlQuery = "SELECT u.username, m.text, m.pub_date\nFROM message m\nJOIN user u ON m.author_id = u.user_id\n " +
                  "WHERE u.username = @author";
        var cheeps = new List<CheepViewModel>();
        
        using (var connection = new SqliteConnection(_connectionString))
        {
            connection.Open();

            var command = connection.CreateCommand();
            command.CommandText = sqlQuery;
            command.Parameters.AddWithValue("@author", author);

            using var reader = command.ExecuteReader();
            while (reader.Read())
            {
                var dbauthor = reader.GetString(0);
                var message = reader.GetString(1);
                var timestamp = reader.GetInt64(2);
                var cheep = new CheepViewModel(dbauthor, message, UnixTimeStampToDateTimeString(timestamp));
                cheeps.Add(cheep);
            }
        }
        return cheeps;
    }
    
    public List<CheepViewModel> GetPagedCheepsByAuthor(string author, int page, int pageSize = 32)
    {
        string sqlQuery = @"SELECT u.username, m.text, m.pub_date 
                     FROM ""message"" m 
                     JOIN ""user"" u ON m.author_id = u.user_id 
                     WHERE u.username = @author
                     ORDER BY m.pub_date DESC
                     LIMIT @pageSize OFFSET @offset";
        
        var cheeps = new List<CheepViewModel>();
        int offset = (page - 1) * pageSize;
        
        using (var connection = new SqliteConnection(_connectionString))
        {
            connection.Open();

            var command = connection.CreateCommand();
            command.CommandText = sqlQuery;
            command.Parameters.AddWithValue("@author", author);
            command.Parameters.AddWithValue("@pageSize", pageSize);
            command.Parameters.AddWithValue("@offset", offset);

            using var reader = command.ExecuteReader();
            while (reader.Read())
            {
                var dbauthor = reader.GetString(0);
                var message = reader.GetString(1);
                var timestamp = reader.GetInt64(2);
                var cheep = new CheepViewModel(dbauthor, message, UnixTimeStampToDateTimeString(timestamp));
                cheeps.Add(cheep);
            }
        }
        return cheeps;
    }
    
    private static string UnixTimeStampToDateTimeString(double unixTimeStamp)
    {
        // Unix timestamp is seconds past epoch
        DateTime dateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
        dateTime = dateTime.AddSeconds(unixTimeStamp);
        return dateTime.ToString("MM/dd/yy H:mm:ss");
    }
}