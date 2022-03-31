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

        Logger.WriteLog(string.Format("[BotCommands - pzbot_set_channel] Caller: {0}, Params: <#{1}>", Context.User.ToString(), Context.Channel.Id));
        await Context.Message.AddReactionAsync(EmojiList.GreenCheck);
        await Context.Channel.SendMessageAsync(string.Format("Channel <#{0}> successfully configured for the bot to work in.", Context.Channel.Id.ToString()));
    }
}
