using Discord;
using Discord.WebSocket;

namespace DiscordDumper
{
    internal class Program
    {
        static async Task Main(string[] args)
        {

            var Client = await Login();

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

            Console.WriteLine("Guilds:");

            foreach (var guild in await Client.GetGuildsAsync())
            {
                Console.WriteLine(guild.Id);
            }
            
            var server = await Client.GetGuildAsync(guildId) ?? throw new Exception("Guild not found!");

            foreach (var channel in await server.GetTextChannelsAsync())
            {
                Console.WriteLine(channel.Name);
                Console.WriteLine("____________________________________________________");
                try
                {
                    var messages = channel.GetMessagesAsync(100000, CacheMode.AllowDownload);
                    await messages.ForEachAsync(async messagesPage =>
                    {
                        foreach(var message in messagesPage)
                        {
                            Console.WriteLine($"{message.Author} : {message.Content}");
                        }
                    });
                    foreach (var message in await channel.GetPinnedMessagesAsync())
                    {
                        Console.WriteLine(message.Content);
                    }
                }
                catch {
                    Console.WriteLine("Error getting messages in channel " + channel.Name);
                }

            }

            await Task.Delay(-1);

        }


        private static async Task<IDiscordClient> Login() {

            var ready = false;

            var socketClient = new DiscordSocketClient(new DiscordSocketConfig());

            socketClient.Ready += () =>
            {
                ready = true;
                return Task.CompletedTask;
            };

            var token = Environment.GetEnvironmentVariable("TOKEN");

            await socketClient.LoginAsync(TokenType.Bot, token);

            await socketClient.StartAsync();

            while (!ready)
            {
                await Task.Delay(200);
            }
            return socketClient;
        }

    }
}