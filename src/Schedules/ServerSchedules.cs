using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public static class Schedules
{
    public static void ServerRestart(List<object> args)
    {
        ScheduleItem serverRestartAnnouncer = Scheduler.GetItem("ServerRestartAnnouncer");

        if(serverRestartAnnouncer == null)
        {
            Logger.WriteLog(string.Format("[{0}][Server Restart Schedule] serverRestartAnnouncer is null.", DateTime.Now.ToLocalTime()));
            return;
        }

        bool isServerRunning = ServerUtility.IsServerRunning();
        var publicChannel    = BotUtility.Discord.GetTextChannelById(Application.botSettings.PublicChannelId);
        var logChannel       = BotUtility.Discord.GetTextChannelById(Application.botSettings.LogChannelId);

        if(logChannel != null)
        {
            if(isServerRunning)
                logChannel.SendMessageAsync("**[Server Restart Schedule]** Restarting server.");
            else
                logChannel.SendMessageAsync("**[Server Restart Schedule]** Server is not running. Skipping...");
        }

        if(publicChannel != null)
        {
            if(isServerRunning)
                publicChannel.SendMessageAsync("**[Server Restart Schedule]** Restarting server.");
            else
                publicChannel.SendMessageAsync("**[Server Restart Schedule]** Server is not running. Skipping...");
        }
        
        Logger.WriteLog(string.Format("[{0}][Server Restart Schedule] Restarting server. (Is server running: {1})", DateTime.Now.ToLocalTime(), isServerRunning.ToString()));

        serverRestartAnnouncer.Args.Clear();
        if(isServerRunning) ServerUtility.Commands.RestartServer();
        serverRestartAnnouncer.UpdateInterval();
    }

    public static void ServerRestartAnnouncer(List<object> args)
    {
        int[] minuteList = {
            60,
            30,
            15,
            5,
            1
        };

        string[] messageList = { 
            "Server will be restarted in 1 hour.",
            "Server will be restarted in {0} minutes.",
            "Server will be restarted in {0} minutes. Prepare to find shelter!",
            "Server will be restarted in {0} minutes. Escape combat soon!",
            "Server will be restarted in {0} minute. Hold tight!",
        };

        ScheduleItem serverRestartSchedule = Scheduler.GetItem("ServerRestart");
        ScheduleItem self = Scheduler.GetItem("ServerRestartAnnouncer");

        if(serverRestartSchedule == null)
        {
            Logger.WriteLog(string.Format("[{0}][Server Restart Announcer Schedule] serverRebootSchedule is null.", DateTime.Now.ToLocalTime()));
            return;
        }

        if(args == null)
        {
            self.Args = new List<object>();
            self.Args.Add(minuteList.First());
        }
        else if(args.Count < 1)
            self.Args.Add(minuteList.First());

        DateTime now        = DateTime.Now;
        var timeDiffMinutes = serverRestartSchedule.NextExecuteTime.Subtract(now).TotalMinutes;
        var publicChannel   = BotUtility.Discord.GetTextChannelById(Application.botSettings.PublicChannelId);

        int i=0;
        int selectedIndex = -1;
        foreach(int minute in minuteList)
        {
            if(timeDiffMinutes <= minute
            && (int)self.Args.First() == minute)
            {
                selectedIndex = i;

                if(i != minuteList.Length - 1)
                    self.Args[0] = minuteList[i + 1];
            }

            i++;
        }

        if(selectedIndex > -1)
        {
            string serverMsg = string.Format(messageList[selectedIndex], minuteList[selectedIndex].ToString());

            if(publicChannel != null)
                publicChannel.SendMessageAsync(serverMsg);

            ServerUtility.Commands.ServerMsg(serverMsg);
        }
    }

    public static void WorkshopItemUpdateChecker(List<object> args)
    {
        if(!ServerUtility.IsServerRunning()
        && 1 == 0)
        {
            Logger.WriteLog(string.Format("[{0}][Workshop Item Update Checker Schedule] Server is not running. Skipping...", DateTime.Now.ToLocalTime()));
            return;
        }

        ScheduleItem serverRestartSchedule = Scheduler.GetItem("ServerRestart");
        ScheduleItem serverRestartAnnouncer = Scheduler.GetItem("ServerRestartAnnouncer");

        if(serverRestartSchedule == null)
        {
            Logger.WriteLog(string.Format("[{0}][Workshop Item Update Checker Schedule] serverRebootSchedule is null.", DateTime.Now.ToLocalTime()));
            return;
        }
        else if(serverRestartSchedule.NextExecuteTime.Subtract(DateTime.Now).TotalMilliseconds <= Application.botSettings.ServerScheduleSettings.WorkshopItemUpdateRestartTimer)
        {
            Logger.WriteLog(string.Format("[{0}][Workshop Item Update Checker Schedule] Upcoming restart detected. Skipping...", DateTime.Now.ToLocalTime()));
            return;
        }

        if(serverRestartAnnouncer == null)
        {
            Logger.WriteLog(string.Format("[{0}][Workshop Item Update Checker Schedule] serverRestartAnnouncer is null.", DateTime.Now.ToLocalTime()));
            return;
        }

        string configFilePath = ServerUtility.GetServerConfigIniFilePath();
        if(string.IsNullOrEmpty(configFilePath))
        {
            Logger.WriteLog(string.Format("[{0}][Workshop Item Update Checker Schedule] configFilePath is null or empty.", DateTime.Now.ToLocalTime()));
            return;
        }

        IniParser.IniData iniData = IniParser.Parse(configFilePath);
        if(iniData == null)
        {
            Logger.WriteLog(string.Format("[{0}][Workshop Item Update Checker Schedule] iniData is null.", DateTime.Now.ToLocalTime()));
            return;
        }

        string[] workshopIdList = iniData.GetValue("WorkshopItems").Split(';');
        var fetchDetails = Task.Run(async () => await SteamWebAPI.GetWorkshopItemDetails(workshopIdList));
        var itemDetails  = fetchDetails.Result;

        foreach(var item in itemDetails)
        {
            var updateDate = DateTimeOffset.FromUnixTimeSeconds(item.TimeUpdated).LocalDateTime;

            if(updateDate > Application.startTime
            || 1 == 1)
            {
                var  publicChannel    = BotUtility.Discord.GetTextChannelById(Application.botSettings.PublicChannelId);
                var  logChannel       = BotUtility.Discord.GetTextChannelById(Application.botSettings.LogChannelId);
                uint restartInMinutes = Application.botSettings.ServerScheduleSettings.WorkshopItemUpdateRestartTimer / (60 * 1000);

                if(logChannel != null)
                {
                    logChannel.SendMessageAsync("**[Workshop Mod Update Checker]** A workshop mod update has been detected. Preparing to restart server in "+restartInMinutes.ToString()+" minute(s).");
                }
                else
                {
                    Logger.WriteLog(string.Format("[{0}][Workshop Item Update Checker Schedule] logChannel is null.", DateTime.Now.ToLocalTime()));
                    return;
                }

                if(publicChannel != null)
                {
                    publicChannel.SendMessageAsync("**[Workshop Mod Update Checker]** A workshop mod update has been detected. Server will be restarted in "+restartInMinutes.ToString()+" minute(s).");
                }
                else
                {
                    Logger.WriteLog(string.Format("[{0}][Workshop Item Update Checker Schedule] publicChannel is null.", DateTime.Now.ToLocalTime()));
                    return;
                }

                serverRestartSchedule.UpdateInterval(Application.botSettings.ServerScheduleSettings.WorkshopItemUpdateRestartTimer);
                serverRestartAnnouncer.Function(serverRestartAnnouncer.Args);

                ServerUtility.Commands.ServerMsg("Workshop mod update has been detected. Server will be restarted in "+restartInMinutes.ToString()+" minute(s).");

                break;
            }
        }
    }
}
