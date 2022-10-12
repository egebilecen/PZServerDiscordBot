using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public static partial class Schedules
{
    public static void AutoServerStart(List<object> args)
    {
        if(Application.botSettings.BotFeatureSettings.AutoServerStart)
        {
            if(!ServerUtility.IsServerRunning())
            {
                var logChannel = BotUtility.Discord.GetTextChannelById(Application.botSettings.LogChannelId);

                Logger.WriteLog(string.Format("[{0}][AutoServerStart Schedule] Server is not running. Attempting to start the server.", Logger.GetLoggingDate()));

                if(logChannel != null)
                    logChannel.SendMessageAsync("**[Auto Server Starter]** Server is not running. Attempting to start the server.");
                
            #if !DEBUG
                ServerUtility.Commands.StartServer();
            #endif
            }
            else Logger.WriteLog(string.Format("[{0}][AutoServerStart Schedule] Server is running. Skipping...", Logger.GetLoggingDate()));
        }
    }
}
