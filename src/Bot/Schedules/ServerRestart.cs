using System.Collections.Generic;

public static partial class Schedules
{
    public static void ServerRestart(List<object> args)
    {
        if(Application.BotSettings.ServerScheduleSettings.ServerRestartScheduleType.ToLower() == "time"
        && ServerUtility.AbortNextTimedServerRestart)
        {
            ServerUtility.AbortNextTimedServerRestart = false;
            ServerUtility.ResetServerRestartInterval();

            return;
        }

        bool isServerRunning = ServerUtility.IsServerRunning();
        var  publicChannel   = DiscordUtility.GetTextChannelById(Application.BotSettings.PublicChannelId);
        var  logChannel      = DiscordUtility.GetTextChannelById(Application.BotSettings.LogChannelId);

        if(logChannel != null)
        {
            if(isServerRunning)
                logChannel.SendMessageAsync(Localization.Get("sch_serverrestart_restart_text"));
            else
                logChannel.SendMessageAsync(Localization.Get("sch_serverrestart_server_not_running"));
        }

        if(publicChannel != null)
        {
            if(isServerRunning)
                publicChannel.SendMessageAsync(Localization.Get("sch_serverrestart_restart_text"));
        }
        
        Logger.WriteLog(string.Format("[Server Restart Schedule] Restarting server if it is running. (Is server running: {0})", isServerRunning.ToString()));
        
        // Prevent this schedule to run continously until it's interval reset in the call to StartServer().
        Scheduler.GetItem("ServerRestart")?.UpdateInterval(999999999);
        if(isServerRunning) ServerUtility.Commands.RestartServer();
    }
}
