using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public static class Schedules
{
    public static void ServerReboot(List<object> args)
    {
        bool isServerRunning = ServerUtility.IsServerRunning();

        var guild = Application.client.Guilds.ElementAt(0);
        if(guild != null)
        {
            var textChannel = guild.GetTextChannel(Application.botSettings.LogChannelId);

            if(textChannel != null)
            {
                if(isServerRunning)
                    textChannel.SendMessageAsync("**[Reboot Schedule]** Rebooting server.");
                else
                    textChannel.SendMessageAsync("**[Reboot Schedule]** Server is not running. Skipping...");
            }
        }

        if(isServerRunning) ServerUtility.Commands.RestartServer();
    }
}
