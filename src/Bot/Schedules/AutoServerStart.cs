using System.Collections.Generic;

public static partial class Schedules
{
    public static void AutoServerStart(List<object> args)
    {
        if(Application.BotSettings.BotFeatureSettings.AutoServerStart)
        {
            if(ServerUtility.CanStartServer())
            {
                var logChannel = DiscordUtility.GetTextChannelById(Application.BotSettings.LogChannelId);

                Logger.WriteLog(string.Format("[{0}][AutoServerStart Schedule] Server is not running. Attempting to start the server.", Logger.GetLoggingDate()));

                if(logChannel != null)
                    logChannel.SendMessageAsync("**[Auto Server Starter]** Server is not running. Attempting to start the server.");
                
            #if !DEBUG
                ServerUtility.Commands.StartServer();
            #endif
            }
            else Logger.WriteLog(string.Format("[{0}][AutoServerStart Schedule] Either server is running or backup creator is running. Skipping...", Logger.GetLoggingDate()));
        }
    }
}
