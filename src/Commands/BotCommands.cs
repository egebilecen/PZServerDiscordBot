using Discord.Commands;
using System.Threading.Tasks;

public class BotCommands : ModuleBase<SocketCommandContext>
{
    public Model.BotSettings botSettings { get; set; }

    [Command("pzbot_set_channel")]
    [Summary("Sets the channel for bot to work in. (!pzbot_set_channel)")]
    public async Task PZBotSetChannel()
    {
        botSettings.BotChannelId = Context.Channel.Id;
        botSettings.Save();

        Logger.WriteLog("["+Context.Message.Timestamp.UtcDateTime.ToString()+"]"+string.Format("[BotCommands - pzbot_set_channel] Caller: {0}, Params: <#{1}>", Context.User.ToString(), Context.Channel.Id));
        await Context.Message.AddReactionAsync(EmojiList.GreenCheck);
        await ReplyAsync(string.Format("Channel <#{0}> successfully configured for the bot to work in.", Context.Channel.Id.ToString()));
    }

    [Command("pzbot_reset_channel")]
    [Summary("Resets the channel for being able to set new channel for bot to work in. (!pzbot_reset_channel)")]
    public async Task PZBotResetChannel()
    {
        botSettings.BotChannelId = 0;
        botSettings.Save();

        Logger.WriteLog("["+Context.Message.Timestamp.UtcDateTime.ToString()+"]"+string.Format("[BotCommands - pzbot_reset_channel] Caller: {0}", Context.User.ToString()));
        await Context.Message.AddReactionAsync(EmojiList.GreenCheck);
        await ReplyAsync(string.Format("Current working channel has been reset. Please set the new channel using **!pzbot_set_channel** command.", Context.Channel.Id.ToString()));
    }
}
