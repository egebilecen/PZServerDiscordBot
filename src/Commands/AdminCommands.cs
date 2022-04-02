using Discord.Commands;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

public class AdminCommands : ModuleBase<SocketCommandContext>
{
    [Command("help")]
    [Summary("Gets the available commands list. (!help)")]
    public async Task Help()
    {
        List<KeyValuePair<string, string>> adminCommandModule    = BotUtility.Discord.GetCommandModule("AdminCommands");
        List<KeyValuePair<string, string>> botCommandModule      = BotUtility.Discord.GetCommandModule("BotCommands");
        List<KeyValuePair<string, string>> pzserverCommandModule = BotUtility.Discord.GetCommandModule("PZServerCommands");

        await Context.Message.AddReactionAsync(EmojiList.GreenCheck);

        await ReplyAsync("Admin command list:");
        await BotUtility.Discord.SendEmbeddedMessage(Context.Message, adminCommandModule);

        await ReplyAsync("Bot command list:");
        await BotUtility.Discord.SendEmbeddedMessage(Context.Message, botCommandModule);

        await ReplyAsync("Project Zomboid server command list:");
        await BotUtility.Discord.SendEmbeddedMessage(Context.Message, pzserverCommandModule);
    }
}
