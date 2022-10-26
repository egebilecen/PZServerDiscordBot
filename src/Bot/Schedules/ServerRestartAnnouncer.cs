using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public static partial class Schedules
{
    public static void ServerRestartAnnouncer(List<object> args)
    {
        if(!ServerUtility.IsServerRunning()) return;

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
            Logger.WriteLog(string.Format("[{0}][Server Restart Announcer Schedule] serverRebootSchedule is null.", Logger.GetLoggingDate()));
            return;
        }

        // To prevent sending messages every interval, we store the last sent message minute in Args.
        if(args == null)
        {
            self.Args = new List<object>
            {
                minuteList.First()
            };
        }
        else if(args.Count < 1)
            self.Args.Add(minuteList.First());

        DateTime now        = DateTime.Now;
        var timeDiffMinutes = serverRestartSchedule.NextExecuteTime.Subtract(now).TotalMinutes;
        var publicChannel   = DiscordUtility.GetTextChannelById(Application.BotSettings.PublicChannelId);

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
}
