using Discord;
using Discord.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

public class UserCommands : ModuleBase<SocketCommandContext>
{
    [Command("server_status")]
    [Summary("Gets the server status. (!server_status)")]
    public async Task ServerStatus()
    {
        await Context.Message.AddReactionAsync(EmojiList.GreenCheck);
        await ReplyAsync(ServerUtility.IsServerRunning() ? "Server is **running** :hamster:" : "Server is **dead** :skull:");
    }

    [Command("reboot_time")]
    [Summary("Gets the next automated reboot time. (!reboot_time)")]
    public async Task RebootTime()
    {
        var timestamp = new DateTimeOffset(Scheduler.GetItem("ServerReboot").NextExecuteTime).ToUnixTimeSeconds();

        await Context.Message.AddReactionAsync(EmojiList.GreenCheck);
        await ReplyAsync(string.Format("Server will automatically rebooted <t:{0}:R>.", timestamp));
    }
}
