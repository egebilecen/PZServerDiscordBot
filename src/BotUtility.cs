using Discord;
using Discord.Commands;
using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public static class BotUtility
{
    static Dictionary<string, List<KeyValuePair<string, string>>> commandList = new Dictionary<string, List<KeyValuePair<string, string>>>();

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
        public static SocketTextChannel GetTextChannelById(ulong id)
        {
            var guild = Application.client.Guilds.ElementAt(0);
            if(guild == null) return null;

            var textChannel = guild.GetTextChannel(id);

            return textChannel;
        }

        public static async Task DoChannelCheck()
        {
            var guild = Application.client.Guilds.ElementAt(0);

            if(Application.botSettings.GuildId == 0)
            {
                Application.botSettings.GuildId = guild.Id;
                Application.botSettings.Save();
            }

            bool warningMessage = false;

            if(guild.TextChannels.Count < 1) return;

        commandChannelCheck:
            if(Application.botSettings.CommandChannelId == 0)
            {
                warningMessage = true;
                await guild.TextChannels.ElementAt(0).SendMessageAsync("Please set the channel for the bot to work in using **!set_command_channel <channel tag>** command.");
            }
            else
            {
                SocketTextChannel textChannel = guild.GetTextChannel(Application.botSettings.CommandChannelId);

                if(textChannel == null)
                {
                    Application.botSettings.CommandChannelId = 0;
                    Application.botSettings.Save();
                    goto commandChannelCheck;
                }
            }

        logChannelCheck:
            if(Application.botSettings.LogChannelId == 0)
            {
                warningMessage = true;
                await guild.TextChannels.ElementAt(0).SendMessageAsync("Please set the channel for the bot to write logs using **!set_log_channel <channel tag>** command.");
            }
            else
            {
                SocketTextChannel textChannel = guild.GetTextChannel(Application.botSettings.LogChannelId);

                if(textChannel == null)
                {
                    Application.botSettings.LogChannelId = 0;
                    Application.botSettings.Save();
                    goto logChannelCheck;
                }

                await textChannel.SendMessageAsync(string.Format("Bot (**{0}**) is started. :zombie:", Application.botVersion));
            }

        publicChannelCheck:
            if(Application.botSettings.PublicChannelId == 0)
            {
                warningMessage = true;
                await guild.TextChannels.ElementAt(0).SendMessageAsync("Please set the channel for the bot to accept commands in a public channel using **!set_public_channel <channel tag>** command.");
            }
            else
            {
                SocketTextChannel textChannel = guild.GetTextChannel(Application.botSettings.PublicChannelId);

                if(textChannel == null)
                {
                    Application.botSettings.PublicChannelId = 0;
                    Application.botSettings.Save();
                    goto publicChannelCheck;
                }
            }

            if(warningMessage)
                await guild.TextChannels.ElementAt(0).SendMessageAsync("Bot won't accept any other commands until the steps above step(s) are completed. @everyone");
        }

        public static void OrganizeCommands()
        {
            List<CommandInfo> commands = Application.commands.Commands.ToList();

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
                case "AdminCommands":    return Application.botSettings.CommandChannelId;
                case "PZServerCommands": return Application.botSettings.CommandChannelId;
                case "BotCommands":      return Application.botSettings.CommandChannelId;
                case "UserCommands":     return Application.botSettings.PublicChannelId;
            }

            return 0;
        }

        public static string GetCommandModuleOfChannelId(ulong channelId)
        {
            if(channelId == Application.botSettings.CommandChannelId) return "AdminCommands";
            if(channelId == Application.botSettings.PublicChannelId)  return "UserCommands";

            return string.Empty;
        }

        public static async Task SendEmbeddedMessage(SocketUserMessage context, List<KeyValuePair<string, string>> dataList)
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
                    await context.Channel.SendMessageAsync("", false, embedBuilder.Build());

                    if(counter == 25)
                        embedBuilder = new EmbedBuilder();

                    counter = 0;
                    fullCycleCount++;
                }
            }
        }
    }
}
