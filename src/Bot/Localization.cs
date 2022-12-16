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

        // Warning Messages
        { "warn_debug_mode", "WARNING: Bot is running in DEBUG configuration." },

        // Error Messages
        { "err_bot_token", "Couldn't retrieve bot token from \"bot_token.txt\" file.\nPlease refer to \"{0}#writing-the-discord-bot-token-into-file\"." },
        { "err_retv_bot_token", "An error occured while retrieving bot token. Error details are saved into {0} file.\nPlease refer to \"{1}/issues\" and create an issue about this with the log file." },
        { "err_serv_bat", "Couldn't find \"server.bat\" file in the folder. Please rename the bat file you were using to start the server as \"server.bat\". For example, if you were using \"StartServer64.bat\", rename it as \"server.bat\" without quotes." },
        { "err_disc_auth_fail", "Authentication failed! Be sure your discord bot token is valid." },
        { "err_disc_disconn", "An error occured and discord bot has been disconnected! Error details are saved into {0} file.\nPlease refer to \"{1}/issues\" and create an issue about this with the log file." },

        // Info Messages
        { "info_disc_act_bot_ver", "Bot Version: {0}" },

        // Discord Commands
        // ---- Admin Commands
        // -------- !set_command_channel
        { "disc_cmd_set_command_channel_warn", "Channel <#{0}> is configured to be log channel. Please select a different channel." },
        { "disc_cmd_set_command_channel_ok", "Channel <#{0}> successfully configured for the bot to work in." },
        
        // -------- !set_log_channel
        { "disc_cmd_set_log_channel_warn", "Channel <#{0}> is configured to be command channel. Please select a different channel." },
        { "disc_cmd_set_log_channel_ok", "Channel <#{0}> successfully configured for the bot to work in." },
        
        // -------- !set_public_channel
        { "disc_cmd_set_public_channel_ok", "Channel <#{0}> successfully configured for the bot to work in." },

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
        
        // -------- !
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
        return $"LOCALIZATION EROR ({key})";
    }
}
