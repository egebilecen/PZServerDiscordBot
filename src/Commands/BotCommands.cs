using Discord.Commands;
using Discord.WebSocket;
using System.Collections.Generic;
using System.Threading.Tasks;

public class BotCommands : ModuleBase<SocketCommandContext>
{
    [Command("set_command_channel")]
    [Summary("Sets the channel for bot to work in. (!set_command_channel <channel tag>)")]
    public async Task PZBotSetCommandChannel(ISocketMessageChannel channel)
    {
        Application.botSettings.CommandChannelId = channel.Id;
        Application.botSettings.Save();

        Logger.WriteLog("["+Context.Message.Timestamp.UtcDateTime.ToString()+"]"+string.Format("[BotCommands - set_command_channel] Caller: {0}, Params: <#{1}>", Context.User.ToString(), channel.Id));
        await Context.Message.AddReactionAsync(EmojiList.GreenCheck);
        await ReplyAsync(string.Format("Channel <#{0}> successfully configured for the bot to work in.", channel.Id.ToString()));
    }

    [Command("set_log_channel")]
    [Summary("Sets the channel for bot to work in. (!set_log_channel <channel tag>)")]
    public async Task PZBotSetLogChannel(ISocketMessageChannel channel)
    {
        Application.botSettings.LogChannelId = channel.Id;
        Application.botSettings.Save();

        Logger.WriteLog("["+Context.Message.Timestamp.UtcDateTime.ToString()+"]"+string.Format("[BotCommands - set_log_channel] Caller: {0}, Params: <#{1}>", Context.User.ToString(), channel.Id));
        await Context.Message.AddReactionAsync(EmojiList.GreenCheck);
        await ReplyAsync(string.Format("Channel <#{0}> successfully configured for the bot to work in.", channel.Id.ToString()));
    }

    [Command("set_public_channel")]
    [Summary("Sets the channel for bot to work in. (!set_public_channel <channel tag>)")]
    public async Task PZBotSetChannel(ISocketMessageChannel channel)
    {
        Application.botSettings.PublicChannelId = channel.Id;
        Application.botSettings.Save();

        Logger.WriteLog("["+Context.Message.Timestamp.UtcDateTime.ToString()+"]"+string.Format("[BotCommands - set_public_channel] Caller: {0}, Params: <#{1}>", Context.User.ToString(), channel.Id));
        await Context.Message.AddReactionAsync(EmojiList.GreenCheck);
        await ReplyAsync(string.Format("Channel <#{0}> successfully configured for the bot to work in.", channel.Id.ToString()));
    }
}
