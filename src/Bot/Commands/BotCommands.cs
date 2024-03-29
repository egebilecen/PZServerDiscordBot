﻿using Discord;
using Discord.Commands;
using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Globalization;
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
            await Context.Channel.SendMessageAsync(Localization.Get("disc_cmd_set_command_channel_warn").KeyFormat(("channel_id", channel.Id)));
            return;
        }

        Application.BotSettings.CommandChannelId = channel.Id;
        Application.BotSettings.Save();

        Logger.WriteLog(string.Format("[BotCommands - set_command_channel] Caller: {0}, Params: <#{1}>", Context.User.ToString(), channel.Id));
        await Context.Message.AddReactionAsync(EmojiList.GreenCheck);
        await Context.Channel.SendMessageAsync(Localization.Get("bot_disc_chan_set_ok").KeyFormat(("channel_id", channel.Id)));
    }

    [Command("set_log_channel")]
    [Summary("Sets the channel for bot to work in. (!set_log_channel <channel tag>)")]
    public async Task SetLogChannel(ISocketMessageChannel channel)
    {
        if(Application.BotSettings.CommandChannelId == channel.Id)
        {
            await Context.Message.AddReactionAsync(EmojiList.RedCross);
            await Context.Channel.SendMessageAsync(Localization.Get("disc_cmd_set_log_channel_warn").KeyFormat(("channel_id", channel.Id)));
            return;
        }

        Application.BotSettings.LogChannelId = channel.Id;
        Application.BotSettings.Save();

        Logger.WriteLog(string.Format("[BotCommands - set_log_channel] Caller: {0}, Params: <#{1}>", Context.User.ToString(), channel.Id));
        await Context.Message.AddReactionAsync(EmojiList.GreenCheck);
        await Context.Channel.SendMessageAsync(Localization.Get("bot_disc_chan_set_ok").KeyFormat(("channel_id", channel.Id)));
    }

    [Command("set_public_channel")]
    [Summary("Sets the channel for bot to work in. (!set_public_channel <channel tag>)")]
    public async Task SetPublicChannel(ISocketMessageChannel channel)
    {
        Application.BotSettings.PublicChannelId = channel.Id;
        Application.BotSettings.Save();

        Logger.WriteLog(string.Format("[BotCommands - set_public_channel] Caller: {0}, Params: <#{1}>", Context.User.ToString(), channel.Id));
        await Context.Message.AddReactionAsync(EmojiList.GreenCheck);
        await Context.Channel.SendMessageAsync(Localization.Get("bot_disc_chan_set_ok").KeyFormat(("channel_id", channel.Id)));
    }

    [Command("get_settings")]
    [Summary("Gets the bot settings. (!get_settings)")]
    public async Task GetSettings()
    {
        Logger.WriteLog(string.Format("[BotCommands - get_settings] Caller: {0}", Context.User.ToString()));
        await Context.Message.AddReactionAsync(EmojiList.GreenCheck);
        
        string botSettings = "";
        botSettings += Localization.Get("disc_cmd_get_settings_serv_id").KeyFormat(("server_id", Application.BotSettings.GuildId));
        botSettings += "\n";
        botSettings += Localization.Get("disc_cmd_get_settings_cmd_chan_id").KeyFormat(("channel_id", Application.BotSettings.CommandChannelId));
        botSettings += "\n";
        botSettings += Localization.Get("disc_cmd_get_settings_log_chan_id").KeyFormat(("channel_id", Application.BotSettings.LogChannelId));
        botSettings += "\n";
        botSettings += Localization.Get("disc_cmd_get_settings_pub_chan_id").KeyFormat(("channel_id", Application.BotSettings.PublicChannelId));
        botSettings += "\n";
        botSettings += Localization.Get("disc_cmd_get_settings_perk_cac_dur").KeyFormat(("minutes", Application.BotSettings.ServerLogParserSettings.PerkParserCacheDuration));
        botSettings += "\n";
        botSettings += Localization.Get("disc_cmd_get_settings_res_serv_sch_type").KeyFormat(("type", Application.BotSettings.ServerScheduleSettings.ServerRestartScheduleType));
        botSettings += "\n";
        botSettings += Localization.Get("disc_cmd_get_settings_serv_res_times").KeyFormat(("timeList", String.Join(", ", Application.BotSettings.ServerScheduleSettings.ServerRestartTimes)));
        botSettings += "\n";
        botSettings += Localization.Get("disc_cmd_get_settings_res_sch_int").KeyFormat(("minutes", Application.BotSettings.ServerScheduleSettings.ServerRestartSchedule / (60 * 1000)));
        botSettings += "\n";
        botSettings += Localization.Get("disc_cmd_get_settings_mod_sch_int").KeyFormat(("minutes", Application.BotSettings.ServerScheduleSettings.WorkshopItemUpdateSchedule / (60 * 1000)));
        botSettings += "\n";
        botSettings += Localization.Get("disc_cmd_get_settings_mod_rst_timer").KeyFormat(("minutes", Application.BotSettings.ServerScheduleSettings.WorkshopItemUpdateRestartTimer / (60 * 1000)));
        botSettings += "\n";
        botSettings += Localization.Get("disc_cmd_get_settings_serv_aut_strt").KeyFormat(("state", Application.BotSettings.BotFeatureSettings.AutoServerStart ? Localization.Get("gen_enab_up") : Localization.Get("gen_disa_up")));
        botSettings += "\n";
        botSettings += Localization.Get("disc_cmd_get_settings_mod_logging").KeyFormat(("state", Application.BotSettings.BotFeatureSettings.NonPublicModLogging ? Localization.Get("gen_enab_up") : Localization.Get("gen_disa_up")));

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
                schedules += Localization.Get("disc_cmd_get_schedules_run").KeyFormat(("name", scheduleItem.DisplayName), ("timestamp", new DateTimeOffset(scheduleItem.NextExecuteTime).ToUnixTimeSeconds()));
                
                if(i != scheduleItems.Count - 1) schedules += "\n";
            }
        
            await Context.Channel.SendMessageAsync(schedules);
        }
        else await Context.Channel.SendMessageAsync(Localization.Get("disc_cmd_get_schedules_not_fnd"));
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

    [Command("set_restart_schedule_type")]
    [Summary("Set the server's restart schedule type. (\"Interval\" or \"Time\") (!set_restart_schedule_type <\"interval\"|\"time\">)")]
    public async Task SetRestartScheduleType(string scheduleType)
    {
        if(scheduleType.ToLower() != "interval" && scheduleType.ToLower()!="time")
        {
            await Context.Message.AddReactionAsync(EmojiList.RedCross);
            await Context.Channel.SendMessageAsync(Localization.Get("disc_cmd_set_restart_schedule_type_warn"));
            return;
        }

        Logger.WriteLog(string.Format("[BotCommands - set_restart_schedule_type] Caller: {0}, Params: {1}", Context.User.ToString(), scheduleType));
        await Context.Message.AddReactionAsync(EmojiList.GreenCheck);

        Application.BotSettings.ServerScheduleSettings.ServerRestartScheduleType = scheduleType.ToLower();
        Application.BotSettings.Save();

        ServerUtility.ResetServerRestartInterval();

        await Context.Channel.SendMessageAsync(Localization.Get("disc_cmd_set_restart_schedule_type_ok").KeyFormat(("type", scheduleType)));
    }

    [Command("set_restart_interval")]
    [Summary("Set the server's restart schedule interval. (in minutes!) (!set_restart_interval <interval in minutes>)")]
    public async Task SetRestartInterval(uint intervalMinute)
    {
        if(intervalMinute < 60)
        {
            await Context.Message.AddReactionAsync(EmojiList.RedCross);
            await Context.Channel.SendMessageAsync(Localization.Get("disc_cmd_set_restart_interval_int_warn"));
            return;
        }

        Logger.WriteLog(string.Format("[BotCommands - set_restart_interval] Caller: {0}, Params: {1}", Context.User.ToString(), intervalMinute));
        await Context.Message.AddReactionAsync(EmojiList.GreenCheck);
        
        Scheduler.GetItem("ServerRestart")?.UpdateInterval(intervalMinute * 60 * 1000);

        Application.BotSettings.ServerScheduleSettings.ServerRestartSchedule = intervalMinute * 60 * 1000;
        Application.BotSettings.Save();

        await Context.Channel.SendMessageAsync(Localization.Get("disc_cmd_set_restart_interval_int_ok"));
    }

    [Command("set_restart_time")]
    [Summary("Set the server's restart time(s). The time format must be \"HH:mm\" (using 24-hour time). (!set_restart_time <times separated by space>)")]
    public async Task SetRestartTimes(params string[] timeArray)
    {
        if (timeArray.Count() == 0)
        {
            await Context.Message.AddReactionAsync(EmojiList.RedCross);
            await Context.Channel.SendMessageAsync(Localization.Get("disc_cmd_set_restart_time_warn_miss_param"));
            return;
        }

        List<string> timeList = new List<string>(timeArray);
        foreach (string time in timeList)
        {
            DateTime timeDT;
            try 
	        {	        
                timeDT = DateTime.ParseExact(time, "HH:mm", CultureInfo.InvariantCulture);
	        }
	        catch (Exception)
	        {
                await Context.Message.AddReactionAsync(EmojiList.RedCross);
                await Context.Channel.SendMessageAsync(Localization.Get("disc_cmd_set_restart_time_warn_invld_time").KeyFormat(("time", time)));
                return;
	        }
        }

        Logger.WriteLog(string.Format("[BotCommands - set_restart_times] Caller: {0}, Params: {1}", Context.User.ToString(), timeList));
        await Context.Message.AddReactionAsync(EmojiList.GreenCheck);
        
        Scheduler.GetItem("ServerRestart").UpdateInterval(Scheduler.GetIntervalFromTimes(timeList));

        Application.BotSettings.ServerScheduleSettings.ServerRestartTimes = timeList;
        Application.BotSettings.Save();

        await Context.Channel.SendMessageAsync(Localization.Get("disc_cmd_set_restart_time_ok").KeyFormat(("timeList", String.Join(", ",timeList))));
    }

    [Command("set_mod_update_check_interval")]
    [Summary("Set the workshop mod update check schedule interval. (in minutes!) (!set_mod_update_check_interval <interval in minutes>)")]
    public async Task SetWorkshopItemUpdateChecker(uint intervalMinute)
    {
        if(intervalMinute < 0)
        {
            await Context.Message.AddReactionAsync(EmojiList.RedCross);
            await Context.Channel.SendMessageAsync(Localization.Get("disc_cmd_set_mod_update_check_interval_int_warn"));
            return;
        }

        Logger.WriteLog(string.Format("[BotCommands - set_mod_update_check_interval] Caller: {0}, Params: {1}", Context.User.ToString(), intervalMinute));
        await Context.Message.AddReactionAsync(EmojiList.GreenCheck);
        
        Scheduler.GetItem("WorkshopItemUpdateChecker").UpdateInterval(intervalMinute * 60 * 1000);

        Application.BotSettings.ServerScheduleSettings.WorkshopItemUpdateSchedule = intervalMinute * 60 * 1000;
        Application.BotSettings.Save();

        await Context.Channel.SendMessageAsync(Localization.Get("disc_cmd_set_mod_update_check_interval_int_ok"));
    }

    [Command("set_mod_update_restart_timer")]
    [Summary("Sets the restart timer for server when mod update detected. (in minutes!) (!set_mod_update_restart_timer <timer in minutes>)")]
    public async Task SetModUpdateRestartTimer(uint intervalMinute)
    {
        if(intervalMinute < 1)
        {
            await Context.Message.AddReactionAsync(EmojiList.RedCross);
            await Context.Channel.SendMessageAsync(Localization.Get("disc_cmd_set_mod_update_restart_timer_warn"));
            return;
        }

        Logger.WriteLog(string.Format("[BotCommands - set_mod_update_restart_timer] Caller: {0}, Params: {1}", Context.User.ToString(), intervalMinute));
        await Context.Message.AddReactionAsync(EmojiList.GreenCheck);
        
        Application.BotSettings.ServerScheduleSettings.WorkshopItemUpdateRestartTimer = intervalMinute * 60 * 1000;
        Application.BotSettings.Save();

        await Context.Channel.SendMessageAsync(Localization.Get("disc_cmd_set_mod_update_restart_timer_ok"));
    }

    [Command("toggle_non_public_mod_logging")]
    [Summary("Bot will print out non-public mods to log channel if enabled. (!toggle_non_public_mod_logging)")]
    public async Task ToggleNonPublicBotLogging()
    {
        Logger.WriteLog(string.Format("[BotCommands - toggle_non_public_mod_logging] Caller: {0}", Context.User.ToString()));
        await Context.Message.AddReactionAsync(EmojiList.GreenCheck);
        
        Application.BotSettings.BotFeatureSettings.NonPublicModLogging = !Application.BotSettings.BotFeatureSettings.NonPublicModLogging;
        Application.BotSettings.Save();

        await Context.Channel.SendMessageAsync(Localization.Get("disc_cmd_toggle_non_public_mod_logging_ok").KeyFormat(("state", (Application.BotSettings.BotFeatureSettings.NonPublicModLogging ? Localization.Get("gen_enab_up") : Localization.Get("gen_disa_up")).ToLower())));
    }

    [Command("set_perk_cache_duration")]
    [Summary("Set the perk cache duration. (in minutes!) (!set_perk_cache_duration <duration in minutes>)")]
    public async Task SetPerkCacheDuration(uint durationMinute)
    {
        if(durationMinute < 0)
        {
            await Context.Message.AddReactionAsync(EmojiList.RedCross);
            await Context.Channel.SendMessageAsync(Localization.Get("disc_cmd_set_perk_cache_duration_warn"));
            return;
        }

        Logger.WriteLog(string.Format("[BotCommands - set_perk_cache_duration] Caller: {0}, Params: {1}", Context.User.ToString(), durationMinute));
        await Context.Message.AddReactionAsync(EmojiList.GreenCheck);
        
        Application.BotSettings.ServerLogParserSettings.PerkParserCacheDuration = durationMinute;
        Application.BotSettings.Save();

        await Context.Channel.SendMessageAsync(Localization.Get("disc_cmd_set_perk_cache_duration_ok"));
    }

    [Command("reset_perk_cache")]
    [Summary("Reset the perk cache. (!reset_perk_cache)")]
    public async Task ResetPerkCache()
    {
        Logger.WriteLog(string.Format("[BotCommands - reset_perk_cache] Caller: {0}", Context.User.ToString()));
        await Context.Message.AddReactionAsync(EmojiList.GreenCheck);
        
        ServerLogParsers.PerkLog.PerkCache = null;
        await Context.Channel.SendMessageAsync(Localization.Get("disc_cmd_reset_perk_cache_ok"));
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
        autoServerStartSchedule?.UpdateInterval();

        await Context.Channel.SendMessageAsync(Localization.Get("disc_cmd_toggle_server_auto_start_ok").KeyFormat(("state", (Application.BotSettings.BotFeatureSettings.AutoServerStart ? Localization.Get("gen_enab_up") : Localization.Get("gen_disa_up")).ToLower())));
    }

    [Command("backup_server")]
    [Summary("Creates a backup of the server. Backup files can be found in \"server_backup\" folder in the directory where bot has been launched. (!backup_server)")]
    public async Task BackupServer()
    {
        if(ServerUtility.IsServerRunning())
        {
            await Context.Message.AddReactionAsync(EmojiList.RedCross);
            await Context.Channel.SendMessageAsync(Localization.Get("disc_cmd_backup_server_warn"));
            return;
        }

        Logger.WriteLog(string.Format("[BotCommands - backup_server] Caller: {0}", Context.User.ToString()));
        await Context.Message.AddReactionAsync(EmojiList.GreenCheck);
        
        _ = Task.Run(async () => await ServerBackupCreator.Start());

        await Context.Channel.SendMessageAsync(Localization.Get("disc_cmd_backup_server_ok").KeyFormat(("channel_id", Application.BotSettings.LogChannelId)));
    }

    [Command("localization")]
    [Summary("Get/change current localization. (!localization <(optional) new localization name>)")]
    public async Task Localization_(string localizationName = null)
    {
        if(!string.IsNullOrEmpty(localizationName))
        {
            (bool, string) result = await Localization.Download(localizationName);
            
            if(result.Item1)
                await Context.Message.AddReactionAsync(EmojiList.GreenCheck);
            else
                await Context.Message.AddReactionAsync(EmojiList.RedCross);

            await Context.Channel.SendMessageAsync(result.Item2);
            return;
        }

        Localization.LocalizationInfo localizationInfo = Localization.GetCurrentLocalizationInfo();

        var embed = new EmbedBuilder()
        {
            Title = Localization.Get("disc_cmd_localization_embed_title"),
            Color = Color.Green
        };

        embed.AddField(Localization.Get("disc_cmd_localization_embed_language"), localizationInfo.Name);
        embed.AddField(Localization.Get("disc_cmd_localization_embed_version"), localizationInfo.Version);
        embed.AddField(Localization.Get("disc_cmd_localization_embed_desc"), localizationInfo.Description);

        
        await Context.Message.AddReactionAsync(EmojiList.GreenCheck);
        await Context.Channel.SendMessageAsync(" ", embed: embed.Build());

        List<Localization.LocalizationInfo> localizationList = await Localization.GetAvailableLocalizationList();
            
        if(localizationList != null)
        {
            List<KeyValuePair<string, string>> availableLocalizations = localizationList
                .Select(x => new KeyValuePair<string, string>(x.Name, $"{x.Description} ({x.Version})"))
                .ToList();

            await Context.Channel.SendMessageAsync(Localization.Get("disc_cmd_localization_avaib_list"));
            await DiscordUtility.SendEmbeddedMessage(Context.Channel, availableLocalizations);
            await Context.Channel.SendMessageAsync(Localization.Get("disc_cmd_localization_usage"));
        }
        else await Context.Channel.SendMessageAsync(Localization.Get("disc_cmd_localization_no_localization"));

        var lastCacheTime = Localization.LastCacheTime;

        if(lastCacheTime != null)
        {
            await Context.Channel.SendMessageAsync(
                Localization.Get("gen_last_cache_text").KeyFormat(("relative_time", BotUtility.GetPastRelativeTimeStr(DateTime.Now, (DateTime)lastCacheTime)))
            );
        }
    }
}
