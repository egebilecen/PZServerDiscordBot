using System.Collections.Generic;

public static partial class Schedules
{
    public static void ServerRestart(List<object> args)
    {
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
        
        Logger.WriteLog(string.Format("[Server Restart Schedule] Restarting server if it is running. (Is server running: {0})", isServerRunning.ToString()));
        
        // Prevent this schedule to run continously until it's interval reset in the call to StartServer().
        Scheduler.GetItem("ServerRestart")?.UpdateInterval(999999999);
        if(isServerRunning) ServerUtility.Commands.RestartServer();
    }
}
