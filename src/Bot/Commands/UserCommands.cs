using Discord;
using Discord.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

public class UserCommands : ModuleBase<SocketCommandContext>
{
    [Command("bot_info")]
    [Summary("Displays information about this bot. (!bot_info)")]
    public async Task BotInfo()
    {
        await Context.Message.AddReactionAsync(EmojiList.GreenCheck);
        await Context.Channel.SendMessageAsync("This bot is written for people to easily manage their server using Discord. Source code and bot files can be reached from "+Application.BotRepoURL+". If you enjoy the bot, please leave a :star: to repo if you haven't :relaxed:.");
    }

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
