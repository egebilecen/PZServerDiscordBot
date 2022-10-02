using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public static partial class Schedules
{
    public static void BotVersionChecker(List<object> args)
    {
        var latestBotVersionTask = Task.Run(async () => await BotUtility.GetLatestBotVersion());
        string latestBotVersion  = latestBotVersionTask.Result;
        
        if(!string.IsNullOrEmpty(latestBotVersion)
        && latestBotVersion != Application.botVersion)
            BotUtility.Discord.GetTextChannelById(Application.botSettings.LogChannelId).SendMessageAsync(string.Format("There is a new version (**{0}**) of bot! Current version: **{1}**. Please consider to update from {2}.", latestBotVersion, Application.botVersion, Application.botRepoURL));
    }
}
