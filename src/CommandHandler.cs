using Discord;
using Discord.Commands;
using Discord.WebSocket;
using System;
using System.Reflection;
using System.Threading.Tasks;

public class CommandHandler
{
    private readonly DiscordSocketClient _client;
    private readonly CommandService      _commands;
    private readonly IServiceProvider    _services;
    private readonly Model.BotSettings   _botSettings;

    public CommandHandler(DiscordSocketClient client, CommandService commands, IServiceProvider services, Model.BotSettings botSettings)
    {
        _commands     = commands;
        _client       = client;
        _services     = services;
        _botSettings  = botSettings;
    }
    
    public async Task SetupAsync()
    {
        _client.MessageReceived   += HandleCommandAsync;
        _commands.CommandExecuted += OnCommandExecutedAsync; 

        await _commands.AddModulesAsync(assembly: Assembly.GetEntryAssembly(), 
                                        services: _services);
    }

    private async Task OnCommandExecutedAsync(Optional<CommandInfo> command, ICommandContext context, IResult result)
    {
        if(!string.IsNullOrEmpty(result?.ErrorReason))
        {
            await context.Message.AddReactionAsync(new Emoji("❌"));
            await context.Channel.SendMessageAsync(result.ErrorReason);
        }
    }

    private async Task HandleCommandAsync(SocketMessage messageParam)
    {
        SocketUserMessage message = messageParam as SocketUserMessage;

        if (message == null) return;

        int argPos = 0;

        if(!message.HasCharPrefix('!', ref argPos) 
        || message.HasMentionPrefix(_client.CurrentUser, ref argPos)
        || message.Author.IsBot
        || (_botSettings.BotChannelId != 0
            && message.Channel.Id != _botSettings.BotChannelId))
            return;

        var context = new SocketCommandContext(_client, message);

        if(_botSettings.BotChannelId == 0
        && context.Message.Content != "!pzbot_set_channel")
            return;

        await _commands.ExecuteAsync(context : context, 
                                     argPos  : argPos,
                                     services: _services);
    }
}
