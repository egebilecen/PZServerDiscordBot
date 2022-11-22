using System;
using System.Collections.Generic;
using System.Linq;

using AnnouncementIntervalList = System.Collections.Generic.List<System.Collections.Generic.KeyValuePair<int, bool>>;

public static partial class Schedules
{

    public static void ServerRestartAnnouncer(List<object> args)
    {
        if(!ServerUtility.IsServerRunning()) return;

        ScheduleItem serverRestartSchedule = Scheduler.GetItem("ServerRestart");
        ScheduleItem self = Scheduler.GetItem("ServerRestartAnnouncer");

        if(args == null)
        {
            // If bool value is true then announcement is done
            args = new List<object>{
                new AnnouncementIntervalList
                {
                    new KeyValuePair<int, bool>(60, false),
                    new KeyValuePair<int, bool>(30, false),
                    new KeyValuePair<int, bool>(15, false),
                    new KeyValuePair<int, bool>(5, false),
                    new KeyValuePair<int, bool>(1, false)
                }
            };

            self.Args = args;
        }

        if(serverRestartSchedule == null)
        {
            Logger.WriteLog("[Server Restart Announcer Schedule] serverRebootSchedule is null.");
            return;
        }

        DateTime now = DateTime.Now;
        double timeDiffMinutes = serverRestartSchedule.NextExecuteTime.Subtract(now).TotalMinutes;

        AnnouncementIntervalList intervalList = args[0] as AnnouncementIntervalList;
        int index = intervalList.FindIndex(x => (timeDiffMinutes <= x.Key && timeDiffMinutes >= Convert.ToDouble(x.Key) - 0.5) 
                                                 && !x.Value);
        if(index == -1) return;

        var announcementPair = intervalList[index];
        var publicChannel = DiscordUtility.GetTextChannelById(Application.BotSettings.PublicChannelId);

        string message = string.Format("Server will be restarted in {0} {1}.", 
                                        announcementPair.Key >= 60 ? announcementPair.Key / 60 : announcementPair.Key,
                                        announcementPair.Key >= 60 ? "hour(s)" : "minute(s)");

        intervalList[index] = new KeyValuePair<int, bool>(announcementPair.Key, true);

        publicChannel?.SendMessageAsync(message);
        ServerUtility.Commands.ServerMsg(message);
    }
}
