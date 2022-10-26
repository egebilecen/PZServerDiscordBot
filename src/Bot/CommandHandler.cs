using Discord;
using Discord.Commands;
using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

public class CommandHandler
{
    private readonly DiscordSocketClient _client;
    private readonly CommandService      _commands;
    private readonly IServiceProvider    _services;

    private bool IsBotConfigured()
    {
        return !(Application.BotSettings.CommandChannelId == 0
              || Application.BotSettings.LogChannelId     == 0
              || Application.BotSettings.PublicChannelId  == 0);
    }

    public CommandHandler(DiscordSocketClient client, CommandService commands, IServiceProvider services)
    {
        _commands     = commands;
        _client       = client;
        _services     = services;
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

        string command         = message.Content.Split(' ')[0];
        string commandModule   = DiscordUtility.GetCommandModuleName(command);
        bool   isBotConfigured = IsBotConfigured();

        var context = new SocketCommandContext(_client, message);

        if(isBotConfigured
        && (context.Channel.Id != Application.BotSettings.PublicChannelId
            && context.Channel.Id != Application.BotSettings.CommandChannelId))
            return;

        if(command != "!help"
        && commandModule == string.Empty)
            goto unknownCommand;
        else if(command == "!help")
        {
            commandModule = DiscordUtility.GetCommandModuleOfChannelId(context.Channel.Id);
            
            switch(commandModule)
            {
                case "UserCommands":
                {
                    List<KeyValuePair<string, string>> commandList = DiscordUtility.GetCommandModule(commandModule);

                    await context.Message.AddReactionAsync(EmojiList.GreenCheck);
                    await context.Channel.SendMessageAsync("Here is the command list:");
                    await DiscordUtility.SendEmbeddedMessage(context.Message.Channel, commandList);
                    return;
                }

                case "BotCommands":
                case "AdminCommands":
                case "PZServerCommands":
                {
                    List<KeyValuePair<string, string>> adminCommandModule = DiscordUtility.GetCommandModule("AdminCommands");
                    List<KeyValuePair<string, string>> botCommandModule = DiscordUtility.GetCommandModule("BotCommands");
                    List<KeyValuePair<string, string>> pzserverCommandModule = DiscordUtility.GetCommandModule("PZServerCommands");

                    await context.Message.AddReactionAsync(EmojiList.GreenCheck);

                    if(adminCommandModule != null)
                    {
                        await context.Channel.SendMessageAsync("Admin command list:");
                        await DiscordUtility.SendEmbeddedMessage(context.Message.Channel, adminCommandModule);
                    }

                    if(botCommandModule != null)
                    {
                        await context.Channel.SendMessageAsync("Bot command list:");
                        await DiscordUtility.SendEmbeddedMessage(context.Message.Channel, botCommandModule);
                    }

                    if(pzserverCommandModule != null)
                    {
                        await context.Channel.SendMessageAsync("Project Zomboid server command list:");
                        await DiscordUtility.SendEmbeddedMessage(context.Message.Channel, pzserverCommandModule);        
                    }
                    return;
                }
            }
        }
        // If command channel is not set, do not handle any other commands but bot commands.
        else if(!isBotConfigured && commandModule != "BotCommands")
        {
            await context.Message.AddReactionAsync(EmojiList.RedCross);
            await context.Channel.SendMessageAsync("Bot configuration haven't done yet.");
            return;
        }
        // If the channel that the command has been sent doesn't match with the
        // setted channel, do not handle it.
        else if(isBotConfigured && DiscordUtility.GetChannelIdOfCommandModule(commandModule) != context.Channel.Id)
            goto unknownCommand;

        await _commands.ExecuteAsync(context : context, 
                                     argPos  : argPos,
                                     services: _services);
        return;

    unknownCommand:
        await context.Message.AddReactionAsync(EmojiList.RedCross);
        await context.Channel.SendMessageAsync("Unknown command.");
        return;
    }
}
