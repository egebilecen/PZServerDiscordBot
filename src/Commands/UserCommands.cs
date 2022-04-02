using Discord;
using Discord.Commands;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

public class UserCommands : ModuleBase<SocketCommandContext>
{
    [Command("help")]
    [Summary("Gets the available commands list. (!help)")]
    public async Task Help()
    {
        List<KeyValuePair<string, string>> commandModule = BotUtility.Discord.GetCommandModule("UserCommands");

        await Context.Message.AddReactionAsync(EmojiList.GreenCheck);
        await ReplyAsync("Here is the command list:");
        await BotUtility.Discord.SendEmbeddedMessage(Context.Message, commandModule);
    }

    [Command("server_status")]
    [Summary("Gets the server status. (!server_status)")]
    public async Task ServerStatus()
    {
        await Context.Message.AddReactionAsync(EmojiList.GreenCheck);
        await ReplyAsync(ServerUtility.IsServerRunning() ? "Server is **running** :hamster:" : "Server is **dead** :skull:");
    }

    [Command("debug")]
    [Summary("Command enabled for debug purposes. (!debug)")]
    [Remarks("skip")]
    public async Task Debug()
    {
        Logger.WriteLog("["+Context.Message.Timestamp.UtcDateTime.ToString()+"]"+string.Format("[BotCommands - debug] Caller: {0}", Context.User.ToString()));
        await Context.Message.AddReactionAsync(EmojiList.GreenCheck);
    }
}
