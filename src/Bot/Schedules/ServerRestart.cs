using System.Collections.Generic;

public static partial class Schedules
{
    public static void ServerRestart(List<object> args)
    {
        ScheduleItem serverRestartAnnouncer = Scheduler.GetItem("ServerRestartAnnouncer");

        if(serverRestartAnnouncer == null)
        {
            Logger.WriteLog(string.Format("[{0}][Server Restart Schedule] serverRestartAnnouncer is null.", Logger.GetLoggingDate()));
            return;
        }

        bool isServerRunning = ServerUtility.IsServerRunning();
        var  publicChannel   = DiscordUtility.GetTextChannelById(Application.BotSettings.PublicChannelId);
        var  logChannel      = DiscordUtility.GetTextChannelById(Application.BotSettings.LogChannelId);

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
        }
        
        Logger.WriteLog(string.Format("[{0}][Server Restart Schedule] Restarting server if it is running. (Is server running: {1})", Logger.GetLoggingDate(), isServerRunning.ToString()));
        
        // Set server restart interval value back to the value defined in settings just in case of some function
        // updated the default interval value for earlier restart.
        ServerUtility.ResetServerRestartInterval();

        serverRestartAnnouncer.Args.Clear();
        if(isServerRunning) ServerUtility.Commands.RestartServer();
        serverRestartAnnouncer.UpdateInterval();
    }
}
