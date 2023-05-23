using Discord.WebSocket;

namespace DiscordDumper
{
    internal class Program
    {
        static async Task Main(string[] args)
        { 

            var Client = new DiscordSocketClient();

            var token = Environment.GetEnvironmentVariable("TOKEN");

            await Client.LoginAsync(Discord.TokenType.Bot, token);

            await Client.StartAsync();

            

            await Task.Delay(-1);

        }
    }
}