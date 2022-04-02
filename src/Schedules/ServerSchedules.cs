using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public static class Schedules
{
    public static void ServerRestart(List<object> args)
    {
        bool isServerRunning = ServerUtility.IsServerRunning();
        var textChannel      = BotUtility.Discord.GetTextChannelById(Application.botSettings.LogChannelId);

        if(textChannel != null)
        {
            if(isServerRunning)
                textChannel.SendMessageAsync("**[Server Restart Schedule]** Restarting server.");
            else
                textChannel.SendMessageAsync("**[Server Restart Schedule]** Server is not running. Skipping...");
        }

        if(isServerRunning) ServerUtility.Commands.RestartServer();
        Scheduler.GetItem("ServerRestartAnnouncer").Args.Clear();

        Logger.WriteLog(string.Format("[{0}][Server Restart Schedule] Restarting server. (Is server running: {1})", DateTime.Now.ToLocalTime(), isServerRunning.ToString()));
    }
    public static void ServerRestartAnnouncer(List<object> args)
    {
        string message = "";

        ScheduleItem serverRebootSchedule = Scheduler.GetItem("ServerRestart");
        ScheduleItem self   = Scheduler.GetItem("ServerRestartAnnouncer");

        DateTime now        = DateTime.Now;
        var timeDiffMinutes = serverRebootSchedule.NextExecuteTime.Subtract(now).TotalMinutes;
        var publicChannel   = BotUtility.Discord.GetTextChannelById(Application.botSettings.PublicChannelId);

        if(timeDiffMinutes <= 5)
        {
            if((int)args[0] == 2)
            {
                message = "Server will be restarted in 5 minutes. Escape combat soon.";

                ServerUtility.Commands.ServerMsg(message);

                if(publicChannel != null)
                    publicChannel.SendMessageAsync(message);

                self.Args[0] = 1;
            }
        }
        else if(timeDiffMinutes <= 15)
        {
            if((int)args[0] == 3)
            {
                message = "Server will be restarted in 15 minutes. Prepare to find shelter.";

                ServerUtility.Commands.ServerMsg(message);
                
                if(publicChannel != null)
                    publicChannel.SendMessageAsync(message);

                self.Args[0] = 2;
            }
        }
        else if(timeDiffMinutes <= 30)
        {
            if((int)args[0] == 4)
            {
                message = "Server will be restarted in 30 minutes.";

                ServerUtility.Commands.ServerMsg(message);
                
                if(publicChannel != null)
                    publicChannel.SendMessageAsync(message);

                self.Args[0] = 3;
            }
        }
        else if(timeDiffMinutes <= 60)
        {
            if(args == null)
                self.Args = new List<object>();

            if(self.Args.Count < 1)
            {
                message = "Server will be restarted in 1 hour.";

                ServerUtility.Commands.ServerMsg(message);
                
                if(publicChannel != null)
                    publicChannel.SendMessageAsync(message);

                self.Args.Add(4);
            }
        }
    }
}
