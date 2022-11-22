using System.Collections.Generic;
using Discord;
using Discord.WebSocket;

using SlashCommandsUtil;

public static class Commands
{
    private static readonly List<SlashCommand> list = new List<SlashCommand>{ 
        new SlashCommand("set_command_channel", "Sets the channel for bot to work in.", BotCommands2.SetCommandChannel, 
        new List<SlashCommandOption>()
        {
            new SlashCommandOption(
                "channel", 
                "Which channel bot will work in.", 
                ApplicationCommandOptionType.Channel, 
                (typeof(SocketTextChannel), "You can only assign a text channel."), 
                true
            )
        })
    };

    public static List<SlashCommand> List()
    {
        return list;
    }
}
