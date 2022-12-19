using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

using EB_Utility;
using Newtonsoft.Json.Linq;
using System.Linq;

public static class Localization
{
    public class LocalizationInfo
    {
        public string Name { get; private set; }
        public SemanticVersion Version { get; private set; }
        public string Description { get; private set; }
        public string File { get; private set; }

        public LocalizationInfo(string name, string version, string description, string file)
        {
            Name = name;
            Version = SemanticVersion.TryParse(version, out SemanticVersion versionResult) ? versionResult : new SemanticVersion(0, 0, 0);
            Description = description;
            File = file;
        }

        [JsonConstructor]
        public LocalizationInfo(string name, SemanticVersion version, string description, string file)
        {
            Name = name;
            Version = version;
            Description = description;
            File = file;
        }
    }

    public const string ScheduleName = "LocalizationVersionChecker";
    public const string LocalizationPath = "./localization/";
    private const string exportPath = "../../../localization/";
    
    private const string localizationDirURL  = "https://raw.githubusercontent.com/egebilecen/PZServerDiscordBot/main/localization/";
    private const string localizationListURL = localizationDirURL + "list.json";
    
    public static DateTime? LastCacheTime { get; private set; } = null;
    private const int cacheDurationMin = 15;
    private static List<LocalizationInfo> lastLocalizationInfoCache = null;

    private static Dictionary<string, string> localization = null;
    private static readonly Dictionary<string, string> defaultLocalization = new Dictionary<string, string>
    {
        // General
        { "gen_enab_up", "Enabled" },
        { "gen_disa_up", "Disabled" },
        { "gen_hours_text", "hour(s)" },
        { "gen_minutes_text", "minute(s)" },
        { "gen_past_rel_time_one_sec", "one second ago" },
        { "gen_past_rel_time_secs", "{seconds} seconds ago" },
        { "gen_past_rel_time_one_min", "a minute ago" },
        { "gen_past_rel_time_mins", "{minutes} minutes ago" },
        { "gen_past_rel_time_one_hour", "an hour ago" },
        { "gen_past_rel_time_hours", "{hours} hours ago" },
        { "gen_past_rel_time_yest", "yesterday" },
        { "gen_past_rel_time_days", "{days} days ago" },
        { "gen_past_rel_time_one_month", "one month ago" },
        { "gen_past_rel_time_months", "{months} months ago" },
        { "gen_past_rel_time_one_year", "one year ago" },
        { "gen_past_rel_time_years", "{years} years ago" },
        { "gen_no_desc", "No description available" },
        { "gen_last_cache_text", "Last cache was at **{relative_time}**." },

        // Success Messages
        { "bot_disc_chan_set_ok", "Channel <#{channel_id}> successfully configured for the bot to work in." },

        // Warning Messages
        { "warn_debug_mode", "WARNING: Bot is running in DEBUG configuration." },
        { "warn_console_missing_conf", "Bot is missing configuration. Please refer to {repo_url}#bot-configuration." },
        { "warn_server_not_running", "Server is not running." },
        { "warn_bot_conf_not_done", "Bot configuration haven't done yet." },
        { "warn_bot_conf_cmd_chan", "Please set a channel for bot to work in using **!set_command_channel <channel tag>** command." },
        { "warn_bot_conf_log_chan", "Please set a channel for bot to write logs using **!set_log_channel <channel tag>** command." },
        { "warn_bot_conf_pub_chan", "Please set a channel for bot to accept commands in a public channel using **!set_public_channel <channel tag>** command." },
        { "warn_bot_wont_accept_cmd", "Bot won't accept any other commands until the steps above step(s) are completed. @everyone" },
        { "warn_unknown_cmd", "Unknown command." },

        // Error Messages
        { "err_bot_token", "Couldn't retrieve bot token from \"bot_token.txt\" file.\nPlease refer to \"{repo_url}#writing-the-discord-bot-token-into-file\"." },
        { "err_retv_bot_token", "An error occured while retrieving bot token. Error details are saved into {log_file} file.\nPlease refer to \"{repo_url}/issues\" and create an issue about this with the log file." },
        { "err_serv_bat", "Couldn't find \"server.bat\" file in the folder. Please rename the bat file you were using to start the server as \"server.bat\". For example, if you were using \"StartServer64.bat\", rename it as \"server.bat\" without quotes." },
        { "err_disc_auth_fail", "Authentication failed! Be sure your discord bot token is valid." },
        { "err_disc_disconn", "An error occured and discord bot has been disconnected! Error details are saved into {log_file} file.\nPlease refer to \"{repo_url}/issues\" and create an issue about this with the log file." },
        { "err_export_localization", "ERROR: Couldn't export default localization file!" },

        // Info Messages
        { "info_disc_act_bot_ver", "Bot Version: {version}" },
        { "info_bot_started", "Bot (**{version}**) is started. :zombie:" },
        { "info_bot_new_version", "There is a new version (**{new_version}**) of bot! Current version: **{current_version}**. Please consider to update from {repo_url}. If you enjoy the bot, please leave a :star: to repo if you haven't :relaxed:." },
        { "info_bot_new_early_version", "There is a new **early access** version (**{new_version}**) of bot! Current version: **{current_version}**. This early access version can be downloaded from **Releases** section of the repo. Repo link: {repo_url}. This version may not be stable as it is not extensively tested (which I also have no means to test it as I don't own a server so any help is appreciated) but it offers early access to the new features. If any problem occurs, you can always switch back to old version from the **Releases** section. If you observe any problem, please report it in **Issues** section." },
        { "info_export_localization", "INFO: Default localization successfully exported!" },

        // Discord Commands
        // ---- !help
        { "disc_cmd_help_user_cmds_title", "Here is the command list:" },
        { "disc_cmd_help_admin_cmds_title", "Admin command list:" },
        { "disc_cmd_help_bot_cmds_title", "Bot command list:" },
        { "disc_cmd_help_pzs_cmds_title", "Project Zomboid server command list:" },

        // ---- Admin Commands
        // -------- !set_command_channel
        { "disc_cmd_set_command_channel_warn", "Channel <#{channel_id}> is configured to be log channel. Please select a different channel." },
        
        // -------- !set_log_channel
        { "disc_cmd_set_log_channel_warn", "Channel <#{channel_id}> is configured to be command channel. Please select a different channel." },

        // -------- !get_settings
        { "disc_cmd_get_settings_serv_id", "**Server ID:** `{server_id}`" },
        { "disc_cmd_get_settings_cmd_chan_id", "**Command Channel ID:** `{channel_id}` (<#{channel_id}>)" },
        { "disc_cmd_get_settings_log_chan_id", "**Log Channel ID:** `{channel_id}` (<#{channel_id}>)" },
        { "disc_cmd_get_settings_pub_chan_id", "**Public Channel ID:** `{channel_id}` (<#{channel_id}>)" },
        { "disc_cmd_get_settings_perk_cac_dur", "**Perk Parser Cache Duration:** {minutes} minute(s)" },
        { "disc_cmd_get_settings_res_sch_int", "**Restart Schedule Interval:** {minutes} minute(s)" },
        { "disc_cmd_get_settings_mod_sch_int", "**Workshop Mod Update Checker Interval:** {minutes} minute(s)" },
        { "disc_cmd_get_settings_mod_rst_timer", "**Workshop Mod Update Restart Timer:** {minutes} minute(s)" },
        { "disc_cmd_get_settings_serv_aut_strt", "**Server Auto Start:** {state}" },
        { "disc_cmd_get_settings_mod_logging", "**Non-public Mod Logging:** {state}" },

        // -------- !get_schedules
        { "disc_cmd_get_schedules_run", "**{name}** schedule will run <t:{timestamp}:R>." },
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
        { "disc_cmd_toggle_non_public_mod_logging_ok", "Non-public mod logging feature has been {state}." },

        // -------- !set_perk_cache_duration
        { "disc_cmd_set_perk_cache_duration_warn", "Duration cannot be smaller than 0. But it can be 0 which means there won't be any caching." },
        { "disc_cmd_set_perk_cache_duration_ok", "Perk cache duration is updated." },
        
        // -------- !reset_perk_cache
        { "disc_cmd_reset_perk_cache_ok", "Perk cache has been reset." },

        // -------- !toggle_server_auto_start
        { "disc_cmd_toggle_server_auto_start_ok", "Server auto start feature has been {state}." },
        
        // -------- !backup_server
        { "disc_cmd_backup_server_warn", "Cannot create a backup while server is running." },
        { "disc_cmd_backup_server_ok", "Starting server backup. You can check the backup progress in log channel (<#{channel_id}>)." },
        { "disc_cmd_backup_server_start", "Server backup started. Total of **{folder_count} folder(s)** will be backed up." },
        { "disc_cmd_backup_server_item_done", "Backup of `{folder_name}` is completed. **({remaining_folder_count} folder left)**" },
        { "disc_cmd_backup_server_finish", "Server backup is completed!" },

        // -------- !localization
        { "disc_cmd_localization_embed_title", "Current Localization Information" },
        { "disc_cmd_localization_embed_language", "Language" },
        { "disc_cmd_localization_embed_version", "Version" },
        { "disc_cmd_localization_embed_desc", "Description" },
        { "disc_cmd_localization_upd_ok", "Localization successfully changed to **{localization}** ({version})." },
        { "disc_cmd_localization_upd_exception", "An unknown error occured while changing localization!" },
        { "disc_cmd_localization_not_found", "Couldn't find **{localization}** localization!" },
        { "disc_cmd_localization_download_fail", "Couldn't download localization! Please try again later..." },
        { "disc_cmd_localization_update_text", "There is a new version for **{localization}** ({version})! Please use `!localization {localization}` command to update." },
        { "disc_cmd_localization_avaib_list", "Available localization list:" },
        { "disc_cmd_localization_usage", "Please use `!localization <localization name>` command to update current localization. You can set localization back to default by using `!localization default` command." },
        { "disc_cmd_localization_no_localization", "There are no other available localizations at the moment." },

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
        { "disc_cmd_initiate_restart_info_server_msg", "A manual server restart has been initiated. Server will be restarted in {minutes} minute(s)." },
        { "disc_cmd_initiate_restart_info_disc_msg", "Manual restart has been initiated. Server will be restarted in **{minutes} minute(s)**." },

        // -------- !abort_restart
        { "disc_cmd_abort_restart_ok_server", "Upcoming restart has been aborted. Next restart will happen in {minutes} minutes." },
        { "disc_cmd_abort_restart_ok_disc", "Upcoming restart has been aborted." },

        // -------- !perk_info
        { "disc_cmd_perk_info_no_result", "Couldn't find any perk log related to username **{username}**." },
        { "disc_cmd_perk_info_result_title", "Perk Information of **{username}**:" },

        // ---- User Commands
        // -------- !bot_info
        { "disc_cmd_bot_info_text", "This bot is written for people to easily manage their server using Discord. Source code and bot files can be reached from {repo_url}. If you enjoy the bot, please leave a :star: to repo if you haven't :relaxed:." },

        // -------- !server_status
        { "disc_cmd_server_status_running", "Server is **running** :hamster:" },
        { "disc_cmd_server_status_backup", "Currently **server backup** is in progress. :wrench:" },
        { "disc_cmd_server_status_dead", "Server is **dead** :skull:" },

        // -------- !restart_time
        { "disc_cmd_restart_time_text", "Server will be restarted <t:{timestamp}:R>." },

        // -------- !game_date
        { "disc_cmd_game_date_warn_file", "Couldn't find the time file." },
        { "disc_cmd_game_date_response", "```Current in-game date: {day}/{month}/{year}```*(Date is in DD-MM-YYYY aka European format)*" },

        // Schedules
        // ---- Display Names
        { "sch_name_serverrestart", "Server Restart" },
        { "sch_name_serverrestartannouncer", "Server Restart Announcer" },
        { "sch_name_workshopitemupdatechecker", "Workshop Mod Update Checker" },
        { "sch_name_autoserverstarter", "Auto Server Starter" },
        { "sch_name_botnewversioncchecker", "Bot New Version Checker" },
        { "sch_name_localizationnewversioncchecker", "Localization New Version Checker" },

        // ---- AutoServerStart
        { "sch_autoserverstart_text", "**[Auto Server Starter]** Server is not running. Attempting to start the server." },

        // ---- ServerRestart
        { "sch_serverrestart_restart_text", "**[Server Restart Schedule]** Restarting server." },
        { "sch_serverrestart_server_not_running", "**[Server Restart Schedule]** Server is not running. Skipping..." },

        // ---- ServerRestartAnnouncer
        { "sch_serverrestartannouncer_text", "Server will be restarted in {time_value} {time_text}." },

        // ---- WorkshopItemUpdateChecker
        { "sch_workshopitemupdatechecker_details_fail", "**[Workshop Mod Update Checker]** Cannot get the details of mod with the ID of `{id}`. It is either set as unlisted or private in Steam Workshop. Steam doesn't allow getting details of unlisted/private workshop items so if it is updated, bot won't detect it. `(Result code: {code})`\n**Mod Link:** {link}" },
        { "sch_workshopitemupdatechecker_log_chan_text", "**[Workshop Mod Update Checker]** A workshop mod update has been detected. Preparing to restart server in {minutes} minute(s)." },
        { "sch_workshopitemupdatechecker_pub_chan_text", "**[Workshop Mod Update Checker]** A workshop mod update has been detected. Server will be restarted in {minutes} minute(s)." },
        { "sch_workshopitemupdatechecker_server_announcement_text", "Workshop mod update has been detected. Server will be restarted in {minutes} minute(s)." },
    };

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

    public static void AddSchedule()
    {
        if(Scheduler.GetItem(ScheduleName) == null
        && GetCurrentLocalizationInfo().Name.ToLower() != "default")
        {
            Scheduler.AddItem(new ScheduleItem(ScheduleName,
                                               Get("sch_name_localizationnewversioncchecker"),
                                               Convert.ToUInt64(TimeSpan.FromMinutes(cacheDurationMin).TotalMilliseconds),
                                               (args) =>
                                               {
                                                   _ = Task.Run(async () => await CheckUpdate());
                                               },
                                               null));
        }
    }

    public static void ExportDefault()
    {
        if(Directory.Exists(exportPath))
        {
            File.WriteAllText($"{exportPath}/default.json", JsonConvert.SerializeObject(defaultLocalization, Formatting.Indented));
            
        #if DEBUG
            Console.WriteLine(Get("info_export_localization"));
        #endif
            return;
        }

    #if DEBUG
        Console.WriteLine(Get("err_export_localization"));
    #endif
    }
    
    public static void Load()
    {
        if(Application.BotSettings.LocalizationInfo != null)
        {
            try
            {
                string localizationFile = $"{LocalizationPath}{Application.BotSettings.LocalizationInfo.File}";
                localization = JObject.Parse(File.ReadAllText(localizationFile)).ToObject<Dictionary<string, string>>();
            }
            catch(Exception ex) 
            {
                Logger.LogException(ex, "Localization.Load()");    
            }
        }
    }

    public static async Task CheckUpdate()
    {
        LocalizationInfo currentLocalization = GetCurrentLocalizationInfo();

        if(currentLocalization.Name.ToLower() == "default")
            return;

        var commandChannel = DiscordUtility.GetTextChannelById(Application.BotSettings.CommandChannelId);

        if(commandChannel == null)
        {
            Logger.WriteLog("Localization.CheckUpdate() - commandChannel is null.");
            return;
        }

        // Update cache if expired.
        await GetAvailableLocalizationList();

        LocalizationInfo newLocalizationInfo = lastLocalizationInfoCache.FirstOrDefault(x => x.Name == currentLocalization.Name);

        if(newLocalizationInfo != null
        && newLocalizationInfo.Version > currentLocalization.Version)
        {
            await commandChannel.SendMessageAsync(Get("disc_cmd_localization_update_text").KeyFormat(("localization", newLocalizationInfo.Name), ("version", newLocalizationInfo.Version)));
            Scheduler.RemoveItem("LocalizationVersionChecker");
        }
    }

    public static async Task<(bool, string)> Download(string language = null)
    {
        language = language?.ToLower();

        if(language == null
        || language == "default")
        {
            localization = null;
            
            Application.BotSettings.LocalizationInfo = null;
            Application.BotSettings.Save();

            return (true, Get("disc_cmd_localization_upd_ok").KeyFormat(("localization", "default"), ("version", "v0.0.0")));
        }

        // Update cache if expired.
        await GetAvailableLocalizationList();

        LocalizationInfo selectedLocalization = lastLocalizationInfoCache.FirstOrDefault(x => x.Name.ToLower() == language);
        if(selectedLocalization == null)
            return (false, Get("disc_cmd_localization_not_found").KeyFormat(("localization", language)));

        try
        {
            string localizationSaveFileName = GetLocalizationSaveFileName(selectedLocalization);
            string localizationSavePath     = $"{LocalizationPath}{localizationSaveFileName}";

            // Don't download localization file if already exist in local.
            if(!File.Exists(localizationSavePath))
            {
                string localizationFileURL = localizationDirURL + selectedLocalization.File;
                string localizationContent = await WebRequest.GetAsync(SteamWebAPI.HttpClient, localizationFileURL);

                if(string.IsNullOrEmpty(localizationContent))
                    return (false, Get("disc_cmd_localization_download_fail"));

                localization = JObject.Parse(localizationContent).ToObject<Dictionary<string, string>>();

                File.WriteAllText(
                    localizationSavePath, 
                    localizationContent
                );
            }
            else
            {
                localization = JObject.Parse(File.ReadAllText(localizationSavePath)).ToObject<Dictionary<string, string>>();
            }

            Application.BotSettings.LocalizationInfo = new LocalizationInfo(
                selectedLocalization.Name, 
                selectedLocalization.Version, 
                selectedLocalization.Description, 
                localizationSaveFileName
            );
            Application.BotSettings.Save();
        }
        catch (Exception ex)
        {
            Logger.LogException(ex, "Localization.Download()");
            return (false, Get("disc_cmd_localization_upd_exception"));
        }

        AddSchedule();
        return (true, Get("disc_cmd_localization_upd_ok").KeyFormat(("localization", selectedLocalization.Name), ("version", selectedLocalization.Version)));
    }

    public static LocalizationInfo GetCurrentLocalizationInfo()
    {
        if(Application.BotSettings.LocalizationInfo != null)
            return Application.BotSettings.LocalizationInfo;

        return new LocalizationInfo("Default", "0.0.0", "English translation of the bot.", "-");
    }

    public static async Task<List<LocalizationInfo>> GetAvailableLocalizationList()
    {
        if(LastCacheTime != null
        && DateTime.Now.Subtract((DateTime)LastCacheTime).TotalMinutes <= cacheDurationMin)
            return lastLocalizationInfoCache;

        string listContent = await WebRequest.GetAsync(SteamWebAPI.HttpClient, localizationListURL);

        if(listContent != null)
        {
            List<LocalizationInfo> localizationInfoList = new List<LocalizationInfo>();

            JObject json = JObject.Parse(listContent);
            if(json.Count < 1) return null;

            Dictionary<string, Dictionary<string, string>> dict = json.ToObject<Dictionary<string, Dictionary<string, string>>>();
            localizationInfoList.AddRange(dict.Select(x =>
            {
                string name = x.Key;
                string version = x.Value["version"];
                string desc = x.Value["desc"];
                string file = x.Value["file"];

                return new LocalizationInfo(name, version, desc, file);
            }).ToList());

            lastLocalizationInfoCache = localizationInfoList;
            LastCacheTime = DateTime.Now;

            return lastLocalizationInfoCache;
        }
        
        lastLocalizationInfoCache = null;
        return lastLocalizationInfoCache;
    }

    public static string GetLocalizationSaveFileName(LocalizationInfo localizationInfo)
    {
        return $"{Path.GetFileNameWithoutExtension(localizationInfo.File)}_{localizationInfo.Version}{Path.GetExtension(localizationInfo.File)}";
    }
}
