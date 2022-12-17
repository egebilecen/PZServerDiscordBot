using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;

public static class Localization
{
    private const string localizationFolder = "./localization/";

    private static Dictionary<string, string> localization = null;
    private static readonly Dictionary<string, string> defaultLocalization = new Dictionary<string, string>
    {
        // General
        { "gen_enab_up", "Enabled" },
        { "gen_disa_up", "Disabled" },
        { "gen_hours_text", "hour(s)" },
        { "gen_minutes_text", "minute(s)" },

        // Success Messages
        { "bot_disc_chan_set_ok", "Channel <#{0}> successfully configured for the bot to work in." },

        // Warning Messages
        { "warn_debug_mode", "WARNING: Bot is running in DEBUG configuration." },
        { "warn_server_not_running", "Server is not running." },

        // Error Messages
        { "err_bot_token", "Couldn't retrieve bot token from \"bot_token.txt\" file.\nPlease refer to \"{0}#writing-the-discord-bot-token-into-file\"." },
        { "err_retv_bot_token", "An error occured while retrieving bot token. Error details are saved into {0} file.\nPlease refer to \"{1}/issues\" and create an issue about this with the log file." },
        { "err_serv_bat", "Couldn't find \"server.bat\" file in the folder. Please rename the bat file you were using to start the server as \"server.bat\". For example, if you were using \"StartServer64.bat\", rename it as \"server.bat\" without quotes." },
        { "err_disc_auth_fail", "Authentication failed! Be sure your discord bot token is valid." },
        { "err_disc_disconn", "An error occured and discord bot has been disconnected! Error details are saved into {0} file.\nPlease refer to \"{1}/issues\" and create an issue about this with the log file." },

        // Info Messages
        { "info_disc_act_bot_ver", "Bot Version: {0}" },
        { "info_bot_new_version", "There is a new version (**{0}**) of bot! Current version: **{1}**. Please consider to update from {2}. If you enjoy the bot, please leave a :star: to repo if you haven't :relaxed:." },
        { "info_bot_new_early_version", "There is a new **early access** version (**{0}**) of bot! Current version: **{1}**. This early access version can be downloaded from **Releases** section of the repo. Repo link: {2}. This version may not be stable as it is not extensively tested (which I also have no means to test it as I don't own a server so any help is appreciated) but it offers early access to the new features. If any problem occurs, you can always switch back to old version from the **Releases** section. If you observe any problem, please report it in **Issues** section." },

        // Discord Commands
        // ---- Admin Commands
        // -------- !set_command_channel
        { "disc_cmd_set_command_channel_warn", "Channel <#{0}> is configured to be log channel. Please select a different channel." },
        
        // -------- !set_log_channel
        { "disc_cmd_set_log_channel_warn", "Channel <#{0}> is configured to be command channel. Please select a different channel." },

        // -------- !get_settings
        { "disc_cmd_get_settings_serv_id", "**Server ID:** `{0}`" },
        { "disc_cmd_get_settings_cmd_chan_id", "**Command Channel ID:** `{0}` (<#{1}>)" },
        { "disc_cmd_get_settings_log_chan_id", "**Log Channel ID:** `{0}` (<#{1}>)" },
        { "disc_cmd_get_settings_pub_chan_id", "**Public Channel ID:** `{0}` (<#{1}>)" },
        { "disc_cmd_get_settings_perk_cac_dur", "**Perk Parser Cache Duration:** {0} minute(s)" },
        { "disc_cmd_get_settings_res_sch_int", "**Restart Schedule Interval:** {0} minute(s)" },
        { "disc_cmd_get_settings_mod_sch_int", "**Workshop Mod Update Checker Interval:** {0} minute(s)" },
        { "disc_cmd_get_settings_mod_rst_timer", "**Workshop Mod Update Restart Timer:** {0} minute(s)" },
        { "disc_cmd_get_settings_serv_aut_strt", "**Server Auto Start:** {0}" },
        { "disc_cmd_get_settings_mod_logging", "**Non-public Mod Logging:** {0}" },

        // -------- !get_schedules
        { "disc_cmd_get_schedules_run", "**{0}** schedule will run <t:{1}:R>." },
        { "disc_cmd_get_schedules_not_fnd", "No schedule found." },

        // -------- !set_restart_interval
        { "disc_cmd_set_restart_interval_int_warn", "Restart interval must be at least 60 minutes." },
        { "disc_cmd_set_restart_interval_int_ok", "Server restart schedule is updated." },

        // -------- !set_mod_update_check_interval
        { "disc_cmd_set_mod_update_check_interval_int_warn", "Interval minutes cannot be smaller than 0. But it can be 0 which means there won't be any workshop mod update checking." },
        { "disc_cmd_set_mod_update_check_interval_int_ok", "Workshop mod update check schedule is updated." },

        // -------- !set_mod_update_restart_timer
        { "disc_cmd_set_mod_update_restart_timer_warn", "Interval must be at least 1 minute(s)." },
        { "disc_cmd_set_mod_update_restart_timer_ok", "Workshop mod update restart timer is updated." },

        // -------- !toggle_non_public_mod_logging
        { "disc_cmd_toggle_non_public_mod_logging_ok", "Non-public mod logging feature has been {0}." },

        // -------- !set_perk_cache_duration
        { "disc_cmd_set_perk_cache_duration_warn", "Duration cannot be smaller than 0. But it can be 0 which means there won't be any caching." },
        { "disc_cmd_set_perk_cache_duration_ok", "Perk cache duration is updated." },
        
        // -------- !reset_perk_cache
        { "disc_cmd_reset_perk_cache_ok", "Perk cache has been reset." },

        // -------- !toggle_server_auto_start
        { "disc_cmd_toggle_server_auto_start_ok", "Server auto start feature has been {0}." },
        
        // -------- !backup_server
        { "disc_cmd_backup_server_warn", "Cannot create a backup while server is running." },
        { "disc_cmd_backup_server_ok", "Starting server backup. You can check the backup progress in log channel (<#{0}>)." },
        { "disc_cmd_backup_server_start", "Server backup started. Total of **{0} folder(s)** will be backed up." },
        { "disc_cmd_backup_server_item_done", "Backup of `{0}` is done. **({1} folder left)**" },
        { "disc_cmd_backup_server_finish", "Server backup is completed!" },
        
        // ---- PZ Server Commands
        // -------- !start_server
        { "disc_cmd_start_server_warn_running", "Server is already running." },
        { "disc_cmd_start_server_warn_backup", "Cannot start the server during backup in progress. Please wait until backup finishes." },
        { "disc_cmd_start_server_ok", "Server should be on it's way to get started. This process may take a while. Please check the server status in 1 or 2 minute(s)." },

        // -------- !stop_server
        { "disc_cmd_stop_server_warn", "Server is already stopped." },
        
        // -------- !restart_server
        { "disc_cmd_restart_server_ok", "Restarting server." },

        // -------- !initiate_restart
        { "disc_cmd_initiate_restart_warn_min", "Minutes cannot be 0. Use `!restart_server` instead." },
        { "disc_cmd_initiate_restart_info_server_msg", "A manual server restart has been initiated. Server will be restarted in {0} minute(s)." },
        { "disc_cmd_initiate_restart_info_disc_msg", "Manual restart has been initiated. Server will be restarted in **{0} minute(s)**." },

        // -------- !abort_restart
        { "disc_cmd_abort_restart_ok_server", "Upcoming restart has been aborted. Next restart will happen in {0} minutes." },
        { "disc_cmd_abort_restart_ok_disc", "Upcoming restart has been aborted." },

        // -------- !perk_info
        { "disc_cmd_perk_info_no_result", "Couldn't find any perk log related to username **{0}**." },
        { "disc_cmd_perk_info_result_title", "Perk Information of **{0}**:" },
        { "disc_cmd_perk_info_last_cache", "Last cache was at **{0}**." },

        // ---- User Commands
        // -------- !bot_info
        { "disc_cmd_bot_info_text", "This bot is written for people to easily manage their server using Discord. Source code and bot files can be reached from {0}. If you enjoy the bot, please leave a :star: to repo if you haven't :relaxed:." },

        // -------- !server_status
        { "disc_cmd_server_status_running", "Server is **running** :hamster:" },
        { "disc_cmd_server_status_backup", "Currently **server backup** is in progress. :wrench:" },
        { "disc_cmd_server_status_dead", "Server is **dead** :skull:" },

        // -------- !restart_time
        { "disc_cmd_restart_time_text", "Server will be restarted <t:{0}:R>." },

        // -------- !game_date
        { "disc_cmd_game_date_warn_file", "Couldn't find the time file." },
        { "disc_cmd_game_date_response", "```Current in-game date: {0}/{1}/{2}```*(Date is in DD-MM-YYYY aka European format)*" },

        // Schedules
        // ---- AutoServerStart
        { "sch_autoserverstart_text", "**[Auto Server Starter]** Server is not running. Attempting to start the server." },

        // ---- ServerRestart
        { "sch_serverrestart_restart_text", "**[Server Restart Schedule]** Restarting server." },
        { "sch_serverrestart_server_not_running", "**[Server Restart Schedule]** Server is not running. Skipping..." },

        // ---- ServerRestartAnnouncer
        { "sch_serverrestartannouncer_text", "Server will be restarted in {0} {1}." },

        // ---- WorkshopItemUpdateChecker
        { "sch_workshopitemupdatechecker_details_fail", "**[Workshop Mod Update Checker]** Cannot get the details of mod with the ID of `{0}`. It is either set as unlisted or private in Steam Workshop. Steam doesn't allow getting details of unlisted/private workshop items so if it is updated, bot won't detect it. `(Result code: {1})`\n**Mod Link:** {2}" },
        { "sch_workshopitemupdatechecker_log_chan_text", "**[Workshop Mod Update Checker]** A workshop mod update has been detected. Preparing to restart server in {0} minute(s)." },
        { "sch_workshopitemupdatechecker_pub_chan_text", "**[Workshop Mod Update Checker]** A workshop mod update has been detected. Server will be restarted in {0} minute(s)." },
        { "sch_workshopitemupdatechecker_server_announcement_text", "Workshop mod update has been detected. Server will be restarted in {0} minute(s)." },

        //
        { "", "" },
    };
    
    // TODO
    private static void Load(string language = null)
    {
        //try
        //{
            
        //}
        //catch(Exception ex)
        //{ 
        //    Logger.LogException(ex, "Localization - Load()"); 
        //}
    }

    public static string Get(string key)
    {
        if(localization != null
        && localization.ContainsKey(key))
            return localization[key];
        else if(defaultLocalization.ContainsKey(key))
            return defaultLocalization[key];

        Logger.WriteLog($"[Localization] No localization found for key \"{key}\"");
        return $"LOCALIZATION ERROR ({key})";
    }
}
