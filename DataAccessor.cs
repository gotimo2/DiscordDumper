using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using Microsoft.Data.Sqlite;

namespace DiscordDumper
{
    public class DataAccessor
    {
        SqliteConnection connection;

        public DataAccessor(string fileName) {
           connection = new SqliteConnection("Data Source=" + fileName);
        }

        public async Task<bool> DatabaseExists()
        {
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
            connection.Open();
            await connection.ExecuteAsync("CREATE TABLE Message(" +
                "username VARCHAR(100)," +
                "content VARCHAR(2000)," +
                "sent DATETIME," +
                "channelname VARCHAR(200)" +
                "messageid BIGINT," +
                "userid BIGINT," +
                "channelid BIGINT" +
                ";)");
        }

        public async Task InsertMessage(Message message)
        {
            await connection.ExecuteAsync("INSERT INTO Message (username, content, sent, channelname, messageid, userid, channelid) VALUES (@username, @content, @sent, @channelName, @messageID, @userID, @channelID )");
        }
    }
}
