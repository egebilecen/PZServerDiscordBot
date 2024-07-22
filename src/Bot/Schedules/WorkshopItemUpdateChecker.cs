using System;
using System.Collections.Generic;
using System.Threading.Tasks;

public static partial class Schedules
{
    public static void WorkshopItemUpdateChecker(List<object> args)
    {
    #if !DEBUG
        if(!ServerUtility.IsServerRunning())
        {
            Logger.WriteLog(string.Format("[{0}][Workshop Item Update Checker Schedule] Server is not running. Skipping...", Logger.GetLoggingDate()));
            return;
        }
    #endif

        ScheduleItem serverRestartSchedule = Scheduler.GetItem("ServerRestart");
        ScheduleItem serverRestartAnnouncer = Scheduler.GetItem("ServerRestartAnnouncer");

        if(serverRestartSchedule == null)
        {
            Logger.WriteLog("[Workshop Item Update Checker Schedule] serverRebootSchedule is null.");
            return;
        }
        else if(serverRestartSchedule.NextExecuteTime.Subtract(DateTime.Now).TotalMilliseconds <= Application.BotSettings.ServerScheduleSettings.WorkshopItemUpdateRestartTimer)
        {
            Logger.WriteLog("[Workshop Item Update Checker Schedule] Upcoming restart detected. Skipping...");
            return;
        }

        if(serverRestartAnnouncer == null)
        {
            Logger.WriteLog("[Workshop Item Update Checker Schedule] serverRestartAnnouncer is null.");
            return;
        }

        string configFilePath = ServerUtility.GetServerConfigIniFilePath();
        if(string.IsNullOrEmpty(configFilePath))
        {
            Logger.WriteLog("[Workshop Item Update Checker Schedule] configFilePath is null or empty.");
            return;
        }

        IniParser.IniData iniData = IniParser.Parse(configFilePath);
        if(iniData == null)
        {
            Logger.WriteLog("[Workshop Item Update Checker Schedule] iniData is null.");
            return;
        }

        string[] workshopIdList = iniData.GetValue("WorkshopItems").Split(';');
        if(workshopIdList.Length < 1)
        {
            Logger.WriteLog($"[Workshop Item Update Checker Schedule] Couldn't find any workshop items in iniData. configFilePath: {configFilePath}");
            return;
        }

        var fetchDetails = Task.Run(async () => await SteamWebAPI.GetWorkshopItemDetails(workshopIdList));
        var itemDetails  = fetchDetails.Result;

        var logChannel   = DiscordUtility.GetTextChannelById(Application.BotSettings.LogChannelId);

        foreach(var item in itemDetails)
        {
            if(Application.BotSettings.BotFeatureSettings.NonPublicModLogging
            && item.Result != 1
            && logChannel != null)
            {
                logChannel.SendMessageAsync(Localization.Get("sch_workshopitemupdatechecker_details_fail").KeyFormat(("id", item.PublishedFileId), ("code", item.Result), ("link", "https://steamcommunity.com/sharedfiles/filedetails/?id="+item.PublishedFileId)));
            }

            var updateDate = DateTimeOffset.FromUnixTimeSeconds(item.TimeUpdated);

            if(updateDate > Application.StartTime)
            {
                var  publicChannel    = DiscordUtility.GetTextChannelById(Application.BotSettings.PublicChannelId);
                uint restartInMinutes = ServerUtility.InitiateServerRestart(Application.BotSettings.ServerScheduleSettings.WorkshopItemUpdateRestartTimer);

                if(logChannel != null)
                {
                    logChannel.SendMessageAsync(Localization.Get("sch_workshopitemupdatechecker_log_chan_text").KeyFormat(("minutes", restartInMinutes)));
                }
                else
                {
                    Logger.WriteLog("[Workshop Item Update Checker Schedule] logChannel is null.");
                    return;
                }

                if(publicChannel != null)
                {
                    publicChannel.SendMessageAsync(Localization.Get("sch_workshopitemupdatechecker_pub_chan_text").KeyFormat(("minutes", restartInMinutes)));
                }
                else
                {
                    Logger.WriteLog("[Workshop Item Update Checker Schedule] publicChannel is null.");
                    return;
                }

                ServerUtility.Commands.ServerMsg(Localization.Get("sch_workshopitemupdatechecker_server_announcement_text").KeyFormat(("minutes", restartInMinutes)));
                break;
            }
        }
    }
}
