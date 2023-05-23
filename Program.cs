using Discord;
using Discord.WebSocket;

namespace DiscordDumper
{
    internal class Program
    {
        static async Task Main()
        {
            Console.WriteLine("SQLite database to write to (Leave blank for messages.sqlite): ");

            var DatabaseName = Console.ReadLine();

            if (string.IsNullOrEmpty(DatabaseName) ) { DatabaseName = "messages.sqlite"; }

            var Connection = new DataAccessor(DatabaseName);

            if (await Connection.DatabaseExists() == false)
            {
                await Connection.Setup();
            }

            Console.WriteLine("Bot token: ");

            string token = "";

            while (string.IsNullOrEmpty(token) )
            {
                token=Console.ReadLine()!;
            }

            var Client = await Login(token);

            Console.Clear();

            Console.WriteLine("Guild id: ");

            ulong guildId;

            while (true)
            {
                if (ulong.TryParse(Console.ReadLine(), out guildId))
                {
                    break;
                }
                Console.WriteLine("Invalid id!");
            }

            var server = await Client.GetGuildAsync(guildId) ?? throw new Exception("Guild not found!");

            Console.WriteLine("Channels to dump:");

            var channels = await server.GetTextChannelsAsync();

            foreach (var channel in channels)
            {
                Console.WriteLine(channel.Name);
            }

            foreach (var channel in channels)
            {
                Console.WriteLine("Dumping " + channel.Name);
                try
                {
                    var messages = channel.GetMessagesAsync(1000000, CacheMode.AllowDownload);
                    await messages.ForEachAsync(async messagesPage =>
                    {
                        foreach(var message in messagesPage)
                        {
                            Message msg = new()
                            {
                                userID = message.Author.Id,
                                username = message.Author.Username,
                                channelName = message.Channel.Name,
                                channelID = message.Channel.Id,
                                content = message.Content,
                                messageID = message.Id,
                                sent = message.Timestamp.UtcDateTime
                            };
                            await Connection.InsertMessage(msg);
                        }
                    });
                }
                catch (Exception e){
                    Console.WriteLine($"Error getting messages in channel {channel.Name} : {e.Message} ");
                }
            }
        }


        private static async Task<IDiscordClient> Login(string Token) {

            var ready = false;

            var socketClient = new DiscordSocketClient(new DiscordSocketConfig());

            socketClient.Ready += () =>
            {
                ready = true;
                return Task.CompletedTask;
            };

            await socketClient.LoginAsync(TokenType.Bot, Token);

            await socketClient.StartAsync();

            while (!ready)
            {
                await Task.Delay(200);
            }
            return socketClient;
        }

    }
}