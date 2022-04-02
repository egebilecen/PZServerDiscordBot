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
        ||  message.HasMentionPrefix(_client.CurrentUser, ref argPos)
        ||  message.Author.IsBot)
            return;

        string command       = message.Content.Split(' ')[0];
        string commandModule = BotUtility.Discord.GetCommandModuleName(command);

        var context = new SocketCommandContext(_client, message);

        if(commandModule == string.Empty)
            goto unknownCommand;
        else
        {
            // If command channel is not set, do not handle any other commands but bot commands.
            if((_botSettings.CommandChannelId == 0
            || _botSettings.LogChannelId      == 0
            || _botSettings.PublicChannelId   == 0)
            && commandModule != "BotCommands")
            {
                await context.Message.AddReactionAsync(EmojiList.RedCross);
                await context.Channel.SendMessageAsync("Bot configuration haven't done yet.");
                return;
            }
            // If the channel that the command has been sent doesn't match with the
            // setted channel, do not handle it.
            else if(_botSettings.CommandChannelId != 0
            && _botSettings.LogChannelId          != 0
            && _botSettings.PublicChannelId       != 0
            && BotUtility.Discord.GetModuleChannelId(commandModule) != context.Channel.Id)
                goto unknownCommand;

            await _commands.ExecuteAsync(context : context, 
                                         argPos  : argPos,
                                         services: _services);
            return;
        }

    unknownCommand:
        await context.Message.AddReactionAsync(EmojiList.RedCross);
        await context.Channel.SendMessageAsync("Unknown command.");
        return;
    }
}
