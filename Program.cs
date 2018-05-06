using System;
using System.Linq;
using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;

namespace vanguard_bot
{
    class Program
    {
        private DiscordSocketClient _client;

        static void Main(string[] args) => new Program().MainAsync().GetAwaiter().GetResult();

        public async Task MainAsync()
        {
            _client = new DiscordSocketClient();
            _client.Log += LogAsync;
            _client.Ready += ReadyAsync;
            _client.MessageReceived += MessageReceivedAsync;

            await _client.LoginAsync(TokenType.Bot, Environment.GetEnvironmentVariable("token"));
            await _client.StartAsync();
            await Task.Delay(-1);
        }

        private Task LogAsync(LogMessage log)
        {
            Console.WriteLine(log.ToString());
            
            return Task.CompletedTask;
        }

        private Task ReadyAsync()
        {
            Console.WriteLine($"{_client.CurrentUser} is connected!");

            return Task.CompletedTask;
        }

        private async Task MessageReceivedAsync(SocketMessage message)
        {
            if (message.Author.Id == _client.CurrentUser.Id)
                return;
            
            switch (message.Content)
            {
                case "!Operations":
                    AssignRole(message, "Operations");
                    break;
                case "!Exile":
                    AssignRole(message, "Exile");
                    break;
                case "!Antistasi":
                    AssignRole(message, "Antistasi");
                    break;
                
                default:
                    //message.DeleteAsync();
                    break;
            }
        }

        private async void AssignRole(SocketMessage message, string roleName)
        {
            SocketGuildUser guildUser = message.Author as SocketGuildUser;
            SocketRole guildRole = guildUser.Guild.Roles.First(role => role.Name == roleName);

            if (guildRole == null || guildUser == null) return;

            await guildUser.AddRoleAsync(guildRole);

            guildUser.SendMessageAsync($"Role {guildRole.Name} added succesfully");

            message.DeleteAsync();
        }
    }
}