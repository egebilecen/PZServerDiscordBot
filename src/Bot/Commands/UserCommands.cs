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
        await Context.Channel.SendMessageAsync(string.Format(Localization.Get("disc_cmd_bot_info_text"), Application.BotRepoURL));
    }

    [Command("server_status")]
    [Summary("Gets the server status. (!server_status)")]
    public async Task ServerStatus()
    {
        await Context.Message.AddReactionAsync(EmojiList.GreenCheck);
        await Context.Channel.SendMessageAsync(ServerUtility.IsServerRunning() 
                                             ? Localization.Get("disc_cmd_server_status_running")
                                             : ServerBackupCreator.IsRunning
                                             ? Localization.Get("disc_cmd_server_status_backup")
                                             : Localization.Get("disc_cmd_server_status_dead"));
    }

    [Command("restart_time")]
    [Summary("Gets the next automated restart time. (!restart_time)")]
    public async Task RebootTime()
    {
        var timestamp = new DateTimeOffset(Scheduler.GetItem("ServerRestart").NextExecuteTime).ToUnixTimeSeconds();

        await Context.Message.AddReactionAsync(EmojiList.GreenCheck);
        await Context.Channel.SendMessageAsync(string.Format(Localization.Get("disc_cmd_restart_time_text"), timestamp));
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
            await Context.Channel.SendMessageAsync(Localization.Get("disc_cmd_game_date_warn_file"));
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
            Localization.Get("disc_cmd_game_date_response"), 
            day.ToString().PadLeft(2, '0'), month.ToString().PadLeft(2, '0'), year.ToString()
        );

        await Context.Message.AddReactionAsync(EmojiList.GreenCheck);
        await Context.Channel.SendMessageAsync(responseText);
    }
}
