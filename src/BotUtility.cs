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
    public const string DiscordBotTokenFile = "./bot_token.txt";

    static string discordBotToken = null;
    static Dictionary<string, List<KeyValuePair<string, string>>> commandList = new Dictionary<string, List<KeyValuePair<string, string>>>();

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

        if(version.Contains("-beta"))
            return null;

        return version;
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

    public static class Discord
    {
        public static string GetToken()
        {
            if(discordBotToken != null) return discordBotToken;

            if(!File.Exists(DiscordBotTokenFile))
            {
                try
                {
                    string envVar = Environment.GetEnvironmentVariable("EB_DISCORD_BOT_TOKEN");

                    if(!string.IsNullOrEmpty(envVar))
                    {
                        File.WriteAllText(DiscordBotTokenFile, envVar);
                        discordBotToken = envVar;
                        return envVar;
                    }
                }
                catch { }

                return null;
            }

            discordBotToken = File.ReadAllText(DiscordBotTokenFile);
            return discordBotToken;
        }

        public static SocketTextChannel GetTextChannelById(ulong id)
        {
            var guild = Application.Client.Guilds.ElementAt(0);
            if(guild == null) return null;

            var textChannel = guild.GetTextChannel(id);

            return textChannel;
        }

        public static async Task DoChannelCheck()
        {
            var guild = Application.Client.Guilds.ElementAt(0);

            if(Application.BotSettings.GuildId == 0)
            {
                Application.BotSettings.GuildId = guild.Id;
                Application.BotSettings.Save();
            }

            bool warningMessage = false;

            if(guild.TextChannels.Count < 1) return;

        commandChannelCheck:
            if(Application.BotSettings.CommandChannelId == 0)
            {
                warningMessage = true;
                await guild.TextChannels.ElementAt(0).SendMessageAsync("Please set the channel for the bot to work in using **!set_command_channel <channel tag>** command.");
            }
            else
            {
                SocketTextChannel textChannel = guild.GetTextChannel(Application.BotSettings.CommandChannelId);

                if(textChannel == null)
                {
                    Application.BotSettings.CommandChannelId = 0;
                    Application.BotSettings.Save();
                    goto commandChannelCheck;
                }
            }

        logChannelCheck:
            if(Application.BotSettings.LogChannelId == 0)
            {
                warningMessage = true;
                await guild.TextChannels.ElementAt(0).SendMessageAsync("Please set the channel for the bot to write logs using **!set_log_channel <channel tag>** command.");
            }
            else
            {
                SocketTextChannel textChannel = guild.GetTextChannel(Application.BotSettings.LogChannelId);

                if(textChannel == null)
                {
                    Application.BotSettings.LogChannelId = 0;
                    Application.BotSettings.Save();
                    goto logChannelCheck;
                }

                await textChannel.SendMessageAsync(string.Format("Bot (**{0}**) is started. :zombie:", Application.BotVersion));
            }

        publicChannelCheck:
            if(Application.BotSettings.PublicChannelId == 0)
            {
                warningMessage = true;
                await guild.TextChannels.ElementAt(0).SendMessageAsync("Please set the channel for the bot to accept commands in a public channel using **!set_public_channel <channel tag>** command.");
            }
            else
            {
                SocketTextChannel textChannel = guild.GetTextChannel(Application.BotSettings.PublicChannelId);

                if(textChannel == null)
                {
                    Application.BotSettings.PublicChannelId = 0;
                    Application.BotSettings.Save();
                    goto publicChannelCheck;
                }
            }

            if(warningMessage)
                await guild.TextChannels.ElementAt(0).SendMessageAsync("Bot won't accept any other commands until the steps above step(s) are completed. @everyone");
        }

        public static void OrganizeCommands()
        {
            List<CommandInfo> commands = Application.Commands.Commands.ToList();

            foreach(CommandInfo command in commands)
            {
                if(command.Remarks == "skip")
                    continue;

                string moduleName = command.Module.Name;

                if(!commandList.ContainsKey(moduleName))
                    commandList.Add(moduleName, new List<KeyValuePair<string, string>>());

                commandList[moduleName].Add(new KeyValuePair<string, string>("!"+command.Name, command.Summary ?? "No description available\n"));
            }
        }

        public static string GetCommandModuleName(string command)
        {
            if(commandList == null) return string.Empty;

            foreach(KeyValuePair<string, List<KeyValuePair<string, string>>> commandModule in commandList)
            {
                foreach(KeyValuePair<string, string> commandPair in commandModule.Value)
                {
                    if(commandPair.Key == command)
                        return commandModule.Key;
                }
            }

            return string.Empty;
        }

        public static Dictionary<string, List<KeyValuePair<string, string>>> GetCommandModuleList(string command)
        {
            if(commandList == null) return null;

            var foundCommandList = new Dictionary<string, List<KeyValuePair<string, string>>>();

            foreach(KeyValuePair<string, List<KeyValuePair<string, string>>> commandModule in commandList)
            {
                foreach(KeyValuePair<string, string> commandPair in commandModule.Value)
                {
                    if(commandPair.Key == command)
                        foundCommandList.Add(commandModule.Key, commandModule.Value);
                }
            }

            return foundCommandList.Count > 0 ? foundCommandList : null;
        }

        public static List<KeyValuePair<string, string>> GetCommandModule(string moduleName)
        {
            if(!commandList.ContainsKey(moduleName))
                return null;

            return commandList[moduleName];
        }

        public static ulong GetChannelIdOfCommandModule(string moduleName)
        {
            switch(moduleName)
            {
                case "AdminCommands":    return Application.BotSettings.CommandChannelId;
                case "PZServerCommands": return Application.BotSettings.CommandChannelId;
                case "BotCommands":      return Application.BotSettings.CommandChannelId;
                case "UserCommands":     return Application.BotSettings.PublicChannelId;
            }

            return 0;
        }

        public static string GetCommandModuleOfChannelId(ulong channelId)
        {
            if(channelId == Application.BotSettings.CommandChannelId) return "AdminCommands";
            if(channelId == Application.BotSettings.PublicChannelId)  return "UserCommands";

            return string.Empty;
        }

        public static async Task SendEmbeddedMessage(ISocketMessageChannel channel, List<KeyValuePair<string, string>> dataList)
        {
            if(dataList == null) return;

            EmbedBuilder embedBuilder  = new EmbedBuilder();
            int totalData      = dataList.Count;
            int fullCycleCount = 0;
            int counter        = 0;

            foreach(KeyValuePair<string, string> dataPair in dataList)
            {
                embedBuilder.AddField(dataPair.Key, dataPair.Value);
                counter++;

                if(counter == 25
                || counter + (25 * fullCycleCount) == totalData)
                {
                    await channel.SendMessageAsync("", false, embedBuilder.Build());

                    if(counter == 25)
                        embedBuilder = new EmbedBuilder();

                    counter = 0;
                    fullCycleCount++;
                }
            }
        }
    }
}
