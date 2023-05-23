using Dapper;
using Microsoft.Data.Sqlite;

namespace DiscordDumper
{
    public class DataAccessor
    {
        private readonly string connectionString;

        public DataAccessor(string fileName) {
            connectionString = "Data Source=" + fileName;
        }

        public async Task<bool> DatabaseExists()
        {
            Console.WriteLine("Checking if database exists");
            using var connection = new SqliteConnection(connectionString);
            connection.Open();
            var table = await connection.QueryAsync<string>("SELECT name FROM sqlite_master WHERE type='table' AND name = 'Message';");
            var tableName = table.FirstOrDefault();
            
            if (!string.IsNullOrEmpty(tableName) && tableName == "Message") {
                Console.WriteLine("Found table with name " + tableName);
                return true;
            }
            return false;
        }

        public async Task Setup()
        {
            using var connection = new SqliteConnection(connectionString);
            Console.WriteLine("Setting up database");
            connection.Open();
            await connection.ExecuteAsync("CREATE TABLE Message(" +
                "username VARCHAR(100)," +
                "content VARCHAR(2000)," +
                "sent DATETIME," +
                "channelname VARCHAR(200)," +
                "messageid BIGINT," +
                "userid BIGINT," +
                "channelid BIGINT" +
                ");");
            Console.WriteLine("finished setting up database");
        }

        public async Task InsertMessage(Message message)
        {
            using var connection = new SqliteConnection(connectionString);
            connection.Open();
            await connection.ExecuteAsync(@"INSERT INTO Message (username, content, sent, channelname, messageid, userid, channelid) VALUES (@username, @content, @sent, @channelName, @messageID, @userID, @channelID )", message);
        }
    }
}
