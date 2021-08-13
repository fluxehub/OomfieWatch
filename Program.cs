using System;
using System.Threading.Tasks;
using DSharpPlus;
using DSharpPlus.Entities;
using Microsoft.Extensions.Logging;
using StormWatch;

var config = Config.FromYaml("./config.yml");

var discord = new DiscordClient(new DiscordConfiguration()
{
    Token = config.Token,
    TokenType = TokenType.Bot,
    Intents = DiscordIntents.All
});

await discord.ConnectAsync();

discord.Ready += (sender, _) =>
{
    sender.Logger.LogInformation($"Connected to Discord as {discord.CurrentUser.Username}#{discord.CurrentUser.Discriminator}");
    return Task.CompletedTask;
};

static bool IsOnline(UserStatus status) =>
    status is not (UserStatus.Offline or UserStatus.Invisible);

discord.PresenceUpdated += async (sender, args) =>
{
    var wasOnline = IsOnline(args.PresenceBefore.Status);
    var isOnline = IsOnline(args.PresenceAfter.Status);

    if (args.User.Id != config.User || wasOnline == isOnline)
        return;

    var channel = await sender.GetChannelAsync(config.Channel);
    
    if (isOnline)
    {
        await channel.SendMessageAsync(config.Gifs.Online);
    }
    else
    {
        await channel.SendMessageAsync(config.Gifs.Offline);
    }
};

await Task.Delay(-1);