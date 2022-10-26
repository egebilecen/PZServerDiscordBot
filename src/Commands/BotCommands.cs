using Discord.Commands;
using Discord.WebSocket;
using System.Collections.Generic;
using System.Threading.Tasks;

public class BotCommands : ModuleBase<SocketCommandContext>
{
    [Command("set_command_channel")]
    [Summary("Sets the channel for bot to work in. (!set_command_channel <channel tag>)")]
    public async Task SetCommandChannel(ISocketMessageChannel channel)
    {
        Application.BotSettings.CommandChannelId = channel.Id;
        Application.BotSettings.Save();

        Logger.WriteLog("["+Logger.GetLoggingDate()+"]"+string.Format("[BotCommands - set_command_channel] Caller: {0}, Params: <#{1}>", Context.User.ToString(), channel.Id));
        await Context.Message.AddReactionAsync(EmojiList.GreenCheck);
        await Context.Channel.SendMessageAsync(string.Format("Channel <#{0}> successfully configured for the bot to work in.", channel.Id.ToString()));
    }

    [Command("set_log_channel")]
    [Summary("Sets the channel for bot to work in. (!set_log_channel <channel tag>)")]
    public async Task SetLogChannel(ISocketMessageChannel channel)
    {
        Application.BotSettings.LogChannelId = channel.Id;
        Application.BotSettings.Save();

        Logger.WriteLog("["+Logger.GetLoggingDate()+"]"+string.Format("[BotCommands - set_log_channel] Caller: {0}, Params: <#{1}>", Context.User.ToString(), channel.Id));
        await Context.Message.AddReactionAsync(EmojiList.GreenCheck);
        await Context.Channel.SendMessageAsync(string.Format("Channel <#{0}> successfully configured for the bot to work in.", channel.Id.ToString()));
    }

    [Command("set_public_channel")]
    [Summary("Sets the channel for bot to work in. (!set_public_channel <channel tag>)")]
    public async Task SetPublicChannel(ISocketMessageChannel channel)
    {
        Application.BotSettings.PublicChannelId = channel.Id;
        Application.BotSettings.Save();

        Logger.WriteLog("["+Logger.GetLoggingDate()+"]"+string.Format("[BotCommands - set_public_channel] Caller: {0}, Params: <#{1}>", Context.User.ToString(), channel.Id));
        await Context.Message.AddReactionAsync(EmojiList.GreenCheck);
        await Context.Channel.SendMessageAsync(string.Format("Channel <#{0}> successfully configured for the bot to work in.", channel.Id.ToString()));
    }

    [Command("get_settings")]
    [Summary("Gets the bot settings. (!get_settings)")]
    public async Task GetSettings()
    {
        Logger.WriteLog("["+Logger.GetLoggingDate()+"]"+string.Format("[BotCommands - get_settings] Caller: {0}", Context.User.ToString()));
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

        Logger.WriteLog("["+Logger.GetLoggingDate()+"]"+string.Format("[BotCommands - set_restart_interval] Caller: {0}, Params: {1}", Context.User.ToString(), intervalMinute));
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

        Logger.WriteLog("["+Logger.GetLoggingDate()+"]"+string.Format("[BotCommands - set_mod_update_check_interval] Caller: {0}, Params: {1}", Context.User.ToString(), intervalMinute));
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

        Logger.WriteLog("["+Logger.GetLoggingDate()+"]"+string.Format("[BotCommands - set_mod_update_restart_timer] Caller: {0}, Params: {1}", Context.User.ToString(), intervalMinute));
        await Context.Message.AddReactionAsync(EmojiList.GreenCheck);
        
        Application.BotSettings.ServerScheduleSettings.WorkshopItemUpdateRestartTimer = intervalMinute * 60 * 1000;
        Application.BotSettings.Save();

        await Context.Channel.SendMessageAsync("Workshop mod update restart timer is updated.");
    }

    [Command("toggle_non_public_bot_logging")]
    [Summary("Bot will print out non-public mods to log channel if enabled. (!toggle_non_public_bot_logging)")]
    public async Task ToggleNonPublicBotLogging()
    {
        Logger.WriteLog("["+Logger.GetLoggingDate()+"]"+string.Format("[BotCommands - ToggleNonPublicBotLogs] Caller: {0}", Context.User.ToString()));
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

        Logger.WriteLog("["+Logger.GetLoggingDate()+"]"+string.Format("[BotCommands - set_perk_cache_duration] Caller: {0}, Params: {1}", Context.User.ToString(), durationMinute));
        await Context.Message.AddReactionAsync(EmojiList.GreenCheck);
        
        Application.BotSettings.ServerLogParserSettings.PerkParserCacheDuration = durationMinute;
        Application.BotSettings.Save();

        await Context.Channel.SendMessageAsync("Perk cache duration is updated.");
    }

    [Command("reset_perk_cache")]
    [Summary("Reset the perk cache. (!reset_perk_cache)")]
    public async Task ResetPerkCache()
    {
        Logger.WriteLog("["+Logger.GetLoggingDate()+"]"+string.Format("[BotCommands - reset_perk_cache] Caller: {0}", Context.User.ToString()));
        await Context.Message.AddReactionAsync(EmojiList.GreenCheck);
        
        ServerLogParsers.PerkLog.PerkCache = null;
        await Context.Channel.SendMessageAsync("Perk cache has been reset.");
    }

    [Command("toggle_server_auto_start")]
    [Summary("Enables/Disables the server auto start feature if server is not running. (!toggle_server_auto_start)")]
    public async Task ToggleServerAutoStart()
    {
        Logger.WriteLog("["+Logger.GetLoggingDate()+"]"+string.Format("[BotCommands - ToggleServerAutoStart] Caller: {0}", Context.User.ToString()));
        await Context.Message.AddReactionAsync(EmojiList.GreenCheck);
        
        Application.BotSettings.BotFeatureSettings.AutoServerStart = !Application.BotSettings.BotFeatureSettings.AutoServerStart;
        Application.BotSettings.Save();

        ScheduleItem autoServerStartSchedule = Scheduler.GetItem("AutoServerStart");

        if(autoServerStartSchedule != null)
            autoServerStartSchedule.UpdateInterval();

        await Context.Channel.SendMessageAsync("Server auto start feature has been " + (Application.BotSettings.BotFeatureSettings.AutoServerStart ? "enabled" : "disabled") + ".");
    }
}
