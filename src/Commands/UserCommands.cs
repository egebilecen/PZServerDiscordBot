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
        await Context.Channel.SendMessageAsync(ServerUtility.IsServerRunning() ? "Server is **running** :hamster:" : "Server is **dead** :skull:");
    }

    [Command("restart_time")]
    [Summary("Gets the next automated restart time. (!restart_time)")]
    public async Task RebootTime()
    {
        var timestamp = new DateTimeOffset(Scheduler.GetItem("ServerRestart").NextExecuteTime).ToUnixTimeSeconds();

        await Context.Message.AddReactionAsync(EmojiList.GreenCheck);
        await Context.Channel.SendMessageAsync(string.Format("Server will be restarted <t:{0}:R>.", timestamp));
    }
}
