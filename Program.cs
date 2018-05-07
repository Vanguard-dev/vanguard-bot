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

            await _client.LoginAsync(TokenType.Bot, Environment.GetEnvironmentVariable("VANGUARD_BOT_DISCORD_TOKEN"));
            await _client.StartAsync();
            await Task.Delay(-1);
        }

        private static Task LogAsync(LogMessage log)
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

            switch (message.Content.ToLower())
            {
                case "!vanguard":
                    AssignRole(message, "Vanguard");
                    break;
                case "!operations":
                    AssignRole(message, "Operations");
                    break;
                case "!exile":
                    AssignRole(message, "Exile");
                    break;
                case "!antistasi":
                    AssignRole(message, "Antistasi");
                    break;
                default:
                    if (message.Channel.Name == "tags")
                    {
                        await message.DeleteAsync();
                    }

                    break;
            }
        }

        private static async void AssignRole(SocketMessage message, string roleName)
        {
            var guildUser = message.Author as SocketGuildUser;
            var guildRole = guildUser?.Guild.Roles.First(role => role.Name == roleName);

            if (guildRole == null)
            {
                return;
            }

            //Remove role
            if (guildUser.Roles.Any(role => role.Name == guildRole.Name))
            {
                await guildUser.RemoveRoleAsync(guildRole);
                guildUser.SendMessageAsync($"Role {guildRole.Name} removed!");
            }
            //Add role
            else
            {
                await guildUser.AddRoleAsync(guildRole);
                guildUser.SendMessageAsync($"Role {guildRole.Name} added!");
            }

            message.DeleteAsync();
        }
    }
}