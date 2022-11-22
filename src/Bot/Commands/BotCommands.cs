using Discord.Commands;
using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

public class BotCommands : ModuleBase<SocketCommandContext>
{
    [Command("set_command_channel")]
    [Summary("Sets the channel for bot to work in. (!set_command_channel <channel tag>)")]
    public async Task SetCommandChannel(ISocketMessageChannel channel)
    {
        if(Application.BotSettings.LogChannelId == channel.Id)
        {
            await Context.Message.AddReactionAsync(EmojiList.RedCross);
            await Context.Channel.SendMessageAsync(string.Format("Channel <#{0}> is configured to be log channel. Please select a different channel.", channel.Id.ToString()));
            return;
        }

        Application.BotSettings.CommandChannelId = channel.Id;
        Application.BotSettings.Save();

        Logger.WriteLog(string.Format("[BotCommands - set_command_channel] Caller: {0}, Params: <#{1}>", Context.User.ToString(), channel.Id));
        await Context.Message.AddReactionAsync(EmojiList.GreenCheck);
        await Context.Channel.SendMessageAsync(string.Format("Channel <#{0}> successfully configured for the bot to work in.", channel.Id.ToString()));
    }

    [Command("set_log_channel")]
    [Summary("Sets the channel for bot to work in. (!set_log_channel <channel tag>)")]
    public async Task SetLogChannel(ISocketMessageChannel channel)
    {
        if(Application.BotSettings.CommandChannelId == channel.Id)
        {
            await Context.Message.AddReactionAsync(EmojiList.RedCross);
            await Context.Channel.SendMessageAsync(string.Format("Channel <#{0}> is configured to be command channel. Please select a different channel.", channel.Id.ToString()));
            return;
        }

        Application.BotSettings.LogChannelId = channel.Id;
        Application.BotSettings.Save();

        Logger.WriteLog(string.Format("[BotCommands - set_log_channel] Caller: {0}, Params: <#{1}>", Context.User.ToString(), channel.Id));
        await Context.Message.AddReactionAsync(EmojiList.GreenCheck);
        await Context.Channel.SendMessageAsync(string.Format("Channel <#{0}> successfully configured for the bot to work in.", channel.Id.ToString()));
    }

    [Command("set_public_channel")]
    [Summary("Sets the channel for bot to work in. (!set_public_channel <channel tag>)")]
    public async Task SetPublicChannel(ISocketMessageChannel channel)
    {
        Application.BotSettings.PublicChannelId = channel.Id;
        Application.BotSettings.Save();

        Logger.WriteLog(string.Format("[BotCommands - set_public_channel] Caller: {0}, Params: <#{1}>", Context.User.ToString(), channel.Id));
        await Context.Message.AddReactionAsync(EmojiList.GreenCheck);
        await Context.Channel.SendMessageAsync(string.Format("Channel <#{0}> successfully configured for the bot to work in.", channel.Id.ToString()));
    }

    [Command("get_settings")]
    [Summary("Gets the bot settings. (!get_settings)")]
    public async Task GetSettings()
    {
        Logger.WriteLog(string.Format("[BotCommands - get_settings] Caller: {0}", Context.User.ToString()));
        await Context.Message.AddReactionAsync(EmojiList.GreenCheck);
        
        string botSettings = "";
        botSettings += "**Server ID:** `"+Application.BotSettings.GuildId.ToString()+"`";
        botSettings += "\n";
        botSettings += "**Command Channel ID:** `"+Application.BotSettings.CommandChannelId.ToString()+"` (<#"+Application.BotSettings.CommandChannelId.ToString()+">)";
        botSettings += "\n";
        botSettings += "**Log Channel ID:** `"+Application.BotSettings.LogChannelId.ToString()+"` (<#"+Application.BotSettings.LogChannelId.ToString()+">)";
        botSettings += "\n";
        botSettings += "**Public Channel ID:** `"+Application.BotSettings.PublicChannelId.ToString()+"` (<#"+Application.BotSettings.PublicChannelId.ToString()+">)";
        botSettings += "\n";
        botSettings += "**Perk Parser Cache Duration:** "+Application.BotSettings.ServerLogParserSettings.PerkParserCacheDuration.ToString()+" minute(s)";
        botSettings += "\n";
        botSettings += "**Restart Schedule Interval:** "+(Application.BotSettings.ServerScheduleSettings.ServerRestartSchedule / (60 * 1000)).ToString()+" minute(s)";
        botSettings += "\n";
        botSettings += "**Workshop Mod Update Checker Interval:** "+(Application.BotSettings.ServerScheduleSettings.WorkshopItemUpdateSchedule / (60 * 1000)).ToString()+" minute(s)";
        botSettings += "\n";
        botSettings += "**Workshop Mod Update Restart Timer:** "+(Application.BotSettings.ServerScheduleSettings.WorkshopItemUpdateRestartTimer / (60 * 1000)).ToString()+" minute(s)";
        botSettings += "\n";
        botSettings += "**Server Auto Start:** "+(Application.BotSettings.BotFeatureSettings.AutoServerStart ? "Enabled" : "Disabled");
        botSettings += "\n";
        botSettings += "**Non-public Mod Logging:** "+(Application.BotSettings.BotFeatureSettings.NonPublicModLogging ? "Enabled" : "Disabled");
        
        await Context.Channel.SendMessageAsync(botSettings);
    }

    [Command("get_schedules")]
    [Summary("Gets the remaining times until schedules to be executed. (!get_schedules)")]
    public async Task GetSchedules()
    {
        Logger.WriteLog(string.Format("[BotCommands - get_schedules] Caller: {0}", Context.User.ToString()));
        await Context.Message.AddReactionAsync(EmojiList.GreenCheck);
        
        IReadOnlyCollection<ScheduleItem> scheduleItems = Scheduler.GetItems();

        if(scheduleItems.Count > 0)
        {
            string schedules = "";
            
            foreach((int i, ScheduleItem scheduleItem) in scheduleItems.Select((val, i) => (i, val)))
            {
                schedules += string.Format("**{0}** schedule will run <t:{1}:R>.", scheduleItem.DisplayName, new DateTimeOffset(scheduleItem.NextExecuteTime).ToUnixTimeSeconds());
                
                if(i != scheduleItems.Count - 1) schedules += "\n";
            }
        
            await Context.Channel.SendMessageAsync(schedules);
        }
        else await Context.Channel.SendMessageAsync("No schedule found.");
    }

    [Command("get_ram_cpu")]
    [Summary("Gets the total RAM and CPU usage of the machine. (!get_ram_cpu)")]
    public async Task GetRAMCPU()
    {
        Logger.WriteLog(string.Format("[BotCommands - get_ram_cpu] Caller: {0}", Context.User.ToString()));
        await Context.Message.AddReactionAsync(EmojiList.GreenCheck);
        
        string progressBarStr = "```";
        progressBarStr += Statistics.GetPercentageValueProgressBar("RAM", Statistics.GetTotalRAMUsagePercentage());
        progressBarStr += "\n";
        progressBarStr += Statistics.GetPercentageValueProgressBar("CPU", Statistics.GetTotalCPUUsagePercentage());
        progressBarStr += "```";

        await Context.Channel.SendMessageAsync(progressBarStr);
    }

    [Command("set_restart_interval")]
    [Summary("Set the server's restart schedule interval. (in minutes!) (!set_restart_interval <interval in minutes>)")]
    public async Task SetRestartInterval(uint intervalMinute)
    {
        if(intervalMinute < 60)
        {
            await Context.Message.AddReactionAsync(EmojiList.RedCross);
            await Context.Channel.SendMessageAsync("Restart interval must be at least 60 minutes.");
            return;
        }

        Logger.WriteLog(string.Format("[BotCommands - set_restart_interval] Caller: {0}, Params: {1}", Context.User.ToString(), intervalMinute));
        await Context.Message.AddReactionAsync(EmojiList.GreenCheck);
        
        Scheduler.GetItem("ServerRestart").UpdateInterval(intervalMinute * 60 * 1000);

        Application.BotSettings.ServerScheduleSettings.ServerRestartSchedule = intervalMinute * 60 * 1000;
        Application.BotSettings.Save();

        await Context.Channel.SendMessageAsync("Server restart schedule is updated.");
    }

    [Command("set_mod_update_check_interval")]
    [Summary("Set the workshop mod update check schedule interval. (in minutes!) (!set_mod_update_check_interval <interval in minutes>)")]
    public async Task SetWorkshopItemUpdateChecker(uint intervalMinute)
    {
        //if(intervalMinute < 1)
        //{
        //    await Context.Message.AddReactionAsync(EmojiList.RedCross);
        //    await Context.Channel.SendMessageAsync("Interval must be at least 1 minute(s).");
        //    return;
        //}

        if(intervalMinute < 0)
        {
            await Context.Message.AddReactionAsync(EmojiList.RedCross);
            await Context.Channel.SendMessageAsync("Interval minutes cannot be smaller than 0. But it can be 0 which means there won't be any workshop mod update checking.");
            return;
        }

        Logger.WriteLog(string.Format("[BotCommands - set_mod_update_check_interval] Caller: {0}, Params: {1}", Context.User.ToString(), intervalMinute));
        await Context.Message.AddReactionAsync(EmojiList.GreenCheck);
        
        Scheduler.GetItem("WorkshopItemUpdateChecker").UpdateInterval(intervalMinute * 60 * 1000);

        Application.BotSettings.ServerScheduleSettings.WorkshopItemUpdateSchedule = intervalMinute * 60 * 1000;
        Application.BotSettings.Save();

        await Context.Channel.SendMessageAsync("Workshop mod update check schedule is updated.");
    }

    [Command("set_mod_update_restart_timer")]
    [Summary("Sets the restart timer for server when mod update detected. (in minutes!) (!set_mod_update_restart_timer <timer in minutes>)")]
    public async Task SetModUpdateRestartTimer(uint intervalMinute)
    {
        if(intervalMinute < 1)
        {
            await Context.Message.AddReactionAsync(EmojiList.RedCross);
            await Context.Channel.SendMessageAsync("Interval must be at least 1 minute(s).");
            return;
        }

        Logger.WriteLog(string.Format("[BotCommands - set_mod_update_restart_timer] Caller: {0}, Params: {1}", Context.User.ToString(), intervalMinute));
        await Context.Message.AddReactionAsync(EmojiList.GreenCheck);
        
        Application.BotSettings.ServerScheduleSettings.WorkshopItemUpdateRestartTimer = intervalMinute * 60 * 1000;
        Application.BotSettings.Save();

        await Context.Channel.SendMessageAsync("Workshop mod update restart timer is updated.");
    }

    [Command("toggle_non_public_mod_logging")]
    [Summary("Bot will print out non-public mods to log channel if enabled. (!toggle_non_public_mod_logging)")]
    public async Task ToggleNonPublicBotLogging()
    {
        Logger.WriteLog(string.Format("[BotCommands - toggle_non_public_mod_logging] Caller: {0}", Context.User.ToString()));
        await Context.Message.AddReactionAsync(EmojiList.GreenCheck);
        
        Application.BotSettings.BotFeatureSettings.NonPublicModLogging = !Application.BotSettings.BotFeatureSettings.NonPublicModLogging;
        Application.BotSettings.Save();

        await Context.Channel.SendMessageAsync("Non-public mod logging feature has been " + (Application.BotSettings.BotFeatureSettings.NonPublicModLogging ? "enabled" : "disabled") + ".");
    }

    [Command("set_perk_cache_duration")]
    [Summary("Set the perk cache duration. (in minutes!) (!set_perk_cache_duration <duration in minutes>)")]
    public async Task SetPerkCacheDuration(uint durationMinute)
    {
        //if(durationMinute < 1)
        //{
        //    await Context.Message.AddReactionAsync(EmojiList.RedCross);
        //    await Context.Channel.SendMessageAsync("Duration must be at least 1 minute(s).");
        //    return;
        //}

        if(durationMinute < 0)
        {
            await Context.Message.AddReactionAsync(EmojiList.RedCross);
            await Context.Channel.SendMessageAsync("Duration cannot be smaller than 0. But it can be 0 which means there won't be any caching.");
            return;
        }

        Logger.WriteLog(string.Format("[BotCommands - set_perk_cache_duration] Caller: {0}, Params: {1}", Context.User.ToString(), durationMinute));
        await Context.Message.AddReactionAsync(EmojiList.GreenCheck);
        
        Application.BotSettings.ServerLogParserSettings.PerkParserCacheDuration = durationMinute;
        Application.BotSettings.Save();

        await Context.Channel.SendMessageAsync("Perk cache duration is updated.");
    }

    [Command("reset_perk_cache")]
    [Summary("Reset the perk cache. (!reset_perk_cache)")]
    public async Task ResetPerkCache()
    {
        Logger.WriteLog(string.Format("[BotCommands - reset_perk_cache] Caller: {0}", Context.User.ToString()));
        await Context.Message.AddReactionAsync(EmojiList.GreenCheck);
        
        ServerLogParsers.PerkLog.PerkCache = null;
        await Context.Channel.SendMessageAsync("Perk cache has been reset.");
    }

    [Command("toggle_server_auto_start")]
    [Summary("Enables/Disables the server auto start feature if server is not running. (!toggle_server_auto_start)")]
    public async Task ToggleServerAutoStart()
    {
        Logger.WriteLog(string.Format("[BotCommands - toggle_server_auto_start] Caller: {0}", Context.User.ToString()));
        await Context.Message.AddReactionAsync(EmojiList.GreenCheck);
        
        Application.BotSettings.BotFeatureSettings.AutoServerStart = !Application.BotSettings.BotFeatureSettings.AutoServerStart;
        Application.BotSettings.Save();

        ScheduleItem autoServerStartSchedule = Scheduler.GetItem("AutoServerStart");

        if(autoServerStartSchedule != null)
            autoServerStartSchedule.UpdateInterval();

        await Context.Channel.SendMessageAsync("Server auto start feature has been " + (Application.BotSettings.BotFeatureSettings.AutoServerStart ? "enabled" : "disabled") + ".");
    }

    [Command("backup_server")]
    [Summary("Creates a backup of the server. Backup files can be found in \"server_backup\" folder in the directory where bot has been launched. (!backup_server)")]
    public async Task BackupServer()
    {
        if(ServerUtility.IsServerRunning())
        {
            await Context.Message.AddReactionAsync(EmojiList.RedCross);
            await Context.Channel.SendMessageAsync("Cannot create a backup while server is running.");
            return;
        }

        Logger.WriteLog(string.Format("[BotCommands - backup_server] Caller: {0}", Context.User.ToString()));
        await Context.Message.AddReactionAsync(EmojiList.GreenCheck);
        
        _ = Task.Run(async () => await ServerBackupCreator.Start());

        await Context.Channel.SendMessageAsync("Starting server backup. You can check the backup progress in log channel (<#"+Application.BotSettings.LogChannelId.ToString()+">).");
    }
}
