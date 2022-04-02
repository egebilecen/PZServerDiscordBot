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

                await textChannel.SendMessageAsync("Bot is started. :zombie:");
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

        public static string GetCommandModuleName(string command, ulong channelId=0)
        {
            if(commandList == null) return string.Empty;

            foreach(KeyValuePair<string, List<KeyValuePair<string, string>>> commandModule in commandList)
            {
                if(channelId != 0
                && channelId == GetModuleChannelId(commandModule.Key))
                    return commandModule.Key;

                if(channelId == 0)
                {
                    foreach(KeyValuePair<string, string> commandPair in commandModule.Value)
                    {
                        if(commandPair.Key == command)
                            return commandModule.Key;
                    }
                }
            }

            return string.Empty;
        }

        public static List<KeyValuePair<string, string>> GetCommandModule(string commandModule)
        {
            if(!commandList.ContainsKey(commandModule))
                return null;

            return commandList[commandModule];
        }

        public static ulong GetModuleChannelId(string moduleName)
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

        public static async Task SendEmbeddedMessage(SocketUserMessage context, List<KeyValuePair<string, string>> dataList)
        {
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
