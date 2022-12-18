using Discord;
using Discord.Commands;
using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

public static class DiscordUtility
{
    public const string DiscordBotTokenFile = "./bot_token.txt";

    static string discordBotToken = null;
    static Dictionary<string, List<KeyValuePair<string, string>>> commandList = new Dictionary<string, List<KeyValuePair<string, string>>>();

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
            await guild.TextChannels.ElementAt(0).SendMessageAsync(Localization.Get("warn_bot_conf_cmd_chan"));
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
            await guild.TextChannels.ElementAt(0).SendMessageAsync(Localization.Get("warn_bot_conf_log_chan"));
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

            await textChannel.SendMessageAsync(Localization.Get("info_bot_started").KeyFormat(("version", Application.BotVersion)));
        }

    publicChannelCheck:
        if(Application.BotSettings.PublicChannelId == 0)
        {
            warningMessage = true;
            await guild.TextChannels.ElementAt(0).SendMessageAsync(Localization.Get("warn_bot_conf_pub_chan"));
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
        {
            Console.WriteLine(Localization.Get("warn_console_missing_conf").KeyFormat(("repo_url", Application.BotRepoURL)));
            await guild.TextChannels.ElementAt(0).SendMessageAsync(Localization.Get("warn_bot_wont_accept_cmd"));
        }
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

            commandList[moduleName].Add(new KeyValuePair<string, string>("!"+command.Name, command.Summary ?? Localization.Get("gen_no_desc") + "\n"));
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
