using Discord.Commands;
using System;
using System.IO;
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
        await Context.Channel.SendMessageAsync(ServerUtility.IsServerRunning() 
                                             ? "Server is **running** :hamster:" 
                                             : ServerBackupCreator.IsRunning
                                             ? "Currently **server backup** is in progress. :wrench:"
                                             : "Server is **dead** :skull:");
    }

    [Command("restart_time")]
    [Summary("Gets the next automated restart time. (!restart_time)")]
    public async Task RebootTime()
    {
        var timestamp = new DateTimeOffset(Scheduler.GetItem("ServerRestart").NextExecuteTime).ToUnixTimeSeconds();

        await Context.Message.AddReactionAsync(EmojiList.GreenCheck);
        await Context.Channel.SendMessageAsync(string.Format("Server will be restarted <t:{0}:R>.", timestamp));
    }

    [Command("game_date")]
    [Summary("Gets the current in-game date. (!game_date)")]
    public async Task GameDate()
    {
        string mapTimeFile = ServerPath.MapTimeFilePath();

        if(!File.Exists(mapTimeFile))
        {
            Logger.WriteLog(string.Format("[UserCommand - game_date] Couldn't find path: ", mapTimeFile));
            
            await Context.Message.AddReactionAsync(EmojiList.RedCross);
            await Context.Channel.SendMessageAsync(string.Format("Couldn't find the time file."));
            return;
        }

        // All bytes are in Big-Endian order
        byte[] fileBytes  = File.ReadAllBytes(mapTimeFile);
        byte[] dayBytes   = fileBytes.Skip(0x1C).Take(4).ToArray(); 
        byte[] monthBytes = fileBytes.Skip(0x20).Take(4).ToArray();
        byte[] yearBytes  = fileBytes.Skip(0x24).Take(4).ToArray();

        int day   = (dayBytes[0] << 24
                  |  dayBytes[1] << 16
                  |  dayBytes[2] << 8
                  |  dayBytes[3] << 0)
                  + 1;

        int month = (monthBytes[0] << 24
                  |  monthBytes[1] << 16
                  |  monthBytes[2] << 8
                  |  monthBytes[3] << 0)
                  + 1;
        
        int year  = yearBytes[0] << 24
                  | yearBytes[1] << 16
                  | yearBytes[2] << 8
                  | yearBytes[3] << 0;

        string responseText = string.Format(
            "```" +
            "Current in-game date: {0}/{1}/{2}" +
            "```" + 
            "*(Date is in DD-MM-YYYY aka European format)*", 
            day.ToString().PadLeft(2, '0'), month.ToString().PadLeft(2, '0'), year.ToString());

        await Context.Message.AddReactionAsync(EmojiList.GreenCheck);
        await Context.Channel.SendMessageAsync(responseText);
    }
}
