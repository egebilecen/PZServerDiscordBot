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

        string command       = message.Content.Split(' ')[0];
        string commandModule = BotUtility.Discord.GetCommandModuleName(command);

        var context = new SocketCommandContext(_client, message);

        if(command != "!help"
        && commandModule == string.Empty)
            goto unknownCommand;
        else if(command == "!help")
        {
            commandModule = BotUtility.Discord.GetCommandModuleOfChannelId(context.Channel.Id);
            
            switch(commandModule)
            {
                case "UserCommands":
                {
                    List<KeyValuePair<string, string>> commandList = BotUtility.Discord.GetCommandModule(commandModule);

                    await context.Message.AddReactionAsync(EmojiList.GreenCheck);
                    await context.Channel.SendMessageAsync("Here is the command list:");
                    await BotUtility.Discord.SendEmbeddedMessage(context.Message, commandList);
                    return;
                }

                case "BotCommands":
                case "AdminCommands":
                case "PZServerCommands":
                {
                    List<KeyValuePair<string, string>> adminCommandModule = BotUtility.Discord.GetCommandModule("AdminCommands");
                    List<KeyValuePair<string, string>> botCommandModule = BotUtility.Discord.GetCommandModule("BotCommands");
                    List<KeyValuePair<string, string>> pzserverCommandModule = BotUtility.Discord.GetCommandModule("PZServerCommands");

                    await context.Message.AddReactionAsync(EmojiList.GreenCheck);

                    if(adminCommandModule != null)
                    {
                        await context.Channel.SendMessageAsync("Admin command list:");
                        await BotUtility.Discord.SendEmbeddedMessage(context.Message, adminCommandModule);
                    }

                    if(botCommandModule != null)
                    {
                        await context.Channel.SendMessageAsync("Bot command list:");
                        await BotUtility.Discord.SendEmbeddedMessage(context.Message, botCommandModule);
                    }

                    if(pzserverCommandModule != null)
                    {
                        await context.Channel.SendMessageAsync("Project Zomboid server command list:");
                        await BotUtility.Discord.SendEmbeddedMessage(context.Message, pzserverCommandModule);        
                    }
                    return;
                }
            }
        }
        // If command channel is not set, do not handle any other commands but bot commands.
        else if((Application.botSettings.CommandChannelId == 0
        || Application.botSettings.LogChannelId      == 0
        || Application.botSettings.PublicChannelId   == 0)
        && commandModule != "BotCommands")
        {
            await context.Message.AddReactionAsync(EmojiList.RedCross);
            await context.Channel.SendMessageAsync("Bot configuration haven't done yet.");
            return;
        }
        // If the channel that the command has been sent doesn't match with the
        // setted channel, do not handle it.
        else if (BotUtility.Discord.GetChannelIdOfCommandModule(commandModule) != context.Channel.Id)
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
