using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
            Logger.WriteLog(string.Format("[{0}][Workshop Item Update Checker Schedule] serverRebootSchedule is null.", Logger.GetLoggingDate()));
            return;
        }
        else if(serverRestartSchedule.NextExecuteTime.Subtract(DateTime.Now).TotalMilliseconds <= Application.BotSettings.ServerScheduleSettings.WorkshopItemUpdateRestartTimer)
        {
            Logger.WriteLog(string.Format("[{0}][Workshop Item Update Checker Schedule] Upcoming restart detected. Skipping...", Logger.GetLoggingDate()));
            return;
        }

        if(serverRestartAnnouncer == null)
        {
            Logger.WriteLog(string.Format("[{0}][Workshop Item Update Checker Schedule] serverRestartAnnouncer is null.", Logger.GetLoggingDate()));
            return;
        }

        string configFilePath = ServerUtility.GetServerConfigIniFilePath();
        if(string.IsNullOrEmpty(configFilePath))
        {
            Logger.WriteLog(string.Format("[{0}][Workshop Item Update Checker Schedule] configFilePath is null or empty.", Logger.GetLoggingDate()));
            return;
        }

        IniParser.IniData iniData = IniParser.Parse(configFilePath);
        if(iniData == null)
        {
            Logger.WriteLog(string.Format("[{0}][Workshop Item Update Checker Schedule] iniData is null.", Logger.GetLoggingDate()));
            return;
        }

        string[] workshopIdList = iniData.GetValue("WorkshopItems").Split(';');
        var fetchDetails = Task.Run(async () => await SteamWebAPI.GetWorkshopItemDetails(workshopIdList));
        var itemDetails  = fetchDetails.Result;

        var logChannel   = BotUtility.Discord.GetTextChannelById(Application.BotSettings.LogChannelId);

        foreach(var item in itemDetails)
        {
            if(Application.BotSettings.BotFeatureSettings.NonPublicModLogging
            && item.Result != 1
            && logChannel != null)
            {
                logChannel.SendMessageAsync(string.Format("**[Workshop Mod Update Checker]** Cannot get the details of mod with the ID of `{0}`. It is either set as unlisted or private in Steam Workshop. Steam doesn't allow getting details of unlisted/private workshop items so if it is updated, bot won't detect it. `(Result code: {1})`\n**Mod Link:** {2}", item.PublishedFileId, item.Result.ToString(), "https://steamcommunity.com/sharedfiles/filedetails/?id="+item.PublishedFileId));
            }

            var updateDate = DateTimeOffset.FromUnixTimeSeconds(item.TimeUpdated);

            if(updateDate > Application.StartTime)
            {
                var  publicChannel    = BotUtility.Discord.GetTextChannelById(Application.BotSettings.PublicChannelId);
                uint restartInMinutes = Application.BotSettings.ServerScheduleSettings.WorkshopItemUpdateRestartTimer / (60 * 1000);

                if(logChannel != null)
                {
                    logChannel.SendMessageAsync("**[Workshop Mod Update Checker]** A workshop mod update has been detected. Preparing to restart server in "+restartInMinutes.ToString()+" minute(s).");
                }
                else
                {
                    Logger.WriteLog(string.Format("[{0}][Workshop Item Update Checker Schedule] logChannel is null.", Logger.GetLoggingDate()));
                    return;
                }

                if(publicChannel != null)
                {
                    publicChannel.SendMessageAsync("**[Workshop Mod Update Checker]** A workshop mod update has been detected. Server will be restarted in "+restartInMinutes.ToString()+" minute(s).");
                }
                else
                {
                    Logger.WriteLog(string.Format("[{0}][Workshop Item Update Checker Schedule] publicChannel is null.", Logger.GetLoggingDate()));
                    return;
                }

                ServerUtility.Commands.ServerMsg("Workshop mod update has been detected. Server will be restarted in "+restartInMinutes.ToString()+" minute(s).");
                serverRestartSchedule.UpdateInterval(Application.BotSettings.ServerScheduleSettings.WorkshopItemUpdateRestartTimer);
                Application.StartTime = DateTime.UtcNow.AddMinutes(restartInMinutes);
                break;
            }
        }
    }
}
