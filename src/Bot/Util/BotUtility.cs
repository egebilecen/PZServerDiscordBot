using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public static class BotUtility
{
    public static async Task<string> GetLatestBotVersion()
    {
        const string apiURL = "https://api.github.com/repos/egebilecen/PZServerDiscordBot/releases/latest";
     
        string version = null;
        string result  = await EB_Utility.WebRequest.GetAsync(SteamWebAPI.HttpClient, apiURL);

        if(string.IsNullOrEmpty(result))
            return null;

        try
        {
            JObject jsonObj = JObject.Parse(result);
            version = jsonObj["tag_name"].Value<string>();
        }
        catch(Exception ex)
        {
            Logger.LogException(ex, "Error occured during GetLatestBotVersion().");
        }

        return version;
    }

    public static async Task CheckLatestBotVersion()
    {
        string latestBotVersionStr = await GetLatestBotVersion();
        bool parseResult = SemanticVersion.TryParse(latestBotVersionStr, out SemanticVersion latestBotVersion);

        if(parseResult)
        {
            if(latestBotVersion.Stage == DevelopmentStage.None
            && Application.BotVersion < latestBotVersion)
            {
                var commandChannel = DiscordUtility.GetTextChannelById(Application.BotSettings.CommandChannelId);
                
                if(commandChannel != null)
                {
                    string warningText = string.Format("There is a new version (**{0}**) of bot! Current version: **{1}**. Please consider to update from {2}. If you enjoy the bot, please leave a :star: to repo if you haven't :relaxed:.", latestBotVersion, Application.BotVersion, Application.BotRepoURL);
                    var lastMessages = await commandChannel.GetMessagesAsync(1).FlattenAsync();

                    if(!lastMessages.First().Content.Equals(warningText))
                        await commandChannel.SendMessageAsync(warningText);
                }
            }
        }
        else Logger.WriteLog(string.Format("[{0}][CheckLatestBotVersion()] Couldn't parse the version string. String: {1}", Logger.GetLoggingDate(), latestBotVersionStr));
    }

    // Credits: https://www.c-sharpcorner.com/code/2562/c-sharp-code-to-calculate-relative-time.aspx
    public static string GetRelativeTime(DateTime currentTime, DateTime passedTime)
    {
        const int SECOND = 1;  
        const int MINUTE = 60 * SECOND;  
        const int HOUR   = 60 * MINUTE;  
        const int DAY    = 24 * HOUR;  
        const int MONTH  = 30 * DAY;  
  
        var ts = new TimeSpan(currentTime.Ticks - passedTime.Ticks);  
        double delta = Math.Abs(ts.TotalSeconds);  
  
        if (delta < 1 * MINUTE)  
          return ts.Seconds == 1 ? "one second ago" : ts.Seconds + " seconds ago";  
  
        if (delta < 2 * MINUTE)  
          return "a minute ago";  
  
        if (delta < 45 * MINUTE)  
          return ts.Minutes + " minutes ago";  
  
        if (delta < 90 * MINUTE)  
          return "an hour ago";  
  
        if (delta < 24 * HOUR)  
          return ts.Hours + " hours ago";  
  
        if (delta < 48 * HOUR)  
          return "yesterday";  
  
        if (delta < 30 * DAY)  
          return ts.Days + " days ago";  
  
        if (delta < 12 * MONTH)  
        {  
          int months = Convert.ToInt32(Math.Floor((double)ts.Days / 30));  
          return months <= 1 ? "one month ago" : months + " months ago";  
        }  
        else  
        {  
          int years = Convert.ToInt32(Math.Floor((double)ts.Days / 365));  
          return years <= 1 ? "one year ago" : years + " years ago";  
        }  
    }
}
