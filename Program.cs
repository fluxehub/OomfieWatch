using System.Threading.Tasks;
using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;
using Microsoft.Extensions.Logging;

namespace StormWatch
{
    public class Program
    {
        private static Config _config;

        private static bool IsOnline(UserStatus status) =>
            status is not (UserStatus.Offline or UserStatus.Invisible);

        private static async Task PostOnlineStatusChange(DiscordClient sender, PresenceUpdateEventArgs args)
        {
            var wasOnline = IsOnline(args.PresenceBefore.Status);
            var isOnline = IsOnline(args.PresenceAfter.Status);

            if (args.User.Id != _config.User || wasOnline == isOnline)
                return;

            var channel = await sender.GetChannelAsync(_config.Channel);

            if (isOnline)
            {
                await channel.SendMessageAsync(_config.Gifs.Online);
            }
            else
            {
                await channel.SendMessageAsync(_config.Gifs.Offline);
            }
        }

        public static async Task Main()
        {
            _config = Config.FromYaml("./config.yml");

            var discord = new DiscordClient(new DiscordConfiguration()
            {
                Token = _config.Token,
                TokenType = TokenType.Bot,
                Intents = DiscordIntents.Guilds 
                          | DiscordIntents.GuildMembers
                          | DiscordIntents.GuildPresences
            });

            discord.Ready += (sender, _) =>
            {
                sender.Logger.LogInformation(
                    $"Connected to Discord as {discord.CurrentUser.Username}#{discord.CurrentUser.Discriminator}");
                return Task.CompletedTask;
            };

            discord.PresenceUpdated += PostOnlineStatusChange;

            await discord.ConnectAsync();
            await Task.Delay(-1);
        }
    }
}