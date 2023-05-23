using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using Microsoft.Data.Sqlite;
using SQLitePCL;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace DiscordDumper
{
    public class DataAccessor
    {
        private string connectionString;

        public DataAccessor(string fileName) {
            connectionString = "Data Source=" + fileName;
        }

        public async Task<bool> DatabaseExists()
        {
            using var connection = new SqliteConnection(connectionString);
            connection.Open();
            var table = await connection.QueryAsync<string>("SELECT name FROM sqlite_master WHERE type='table' AND name = 'Message';");
            var tableName = table.FirstOrDefault();
            if (!string.IsNullOrEmpty(tableName) && tableName == "Product") {
                return false;
            }
            return true;
        }

        public async Task Setup()
        {
            using var connection = new SqliteConnection(connectionString);
            Console.WriteLine("Setting up table");
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
