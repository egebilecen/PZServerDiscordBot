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
    static Model.BotSettings botSettings = null;
    static Dictionary<string, List<KeyValuePair<string, string>>> commandList = new Dictionary<string, List<KeyValuePair<string, string>>>();

    public static void Init(Model.BotSettings _botSettings)
    {
        botSettings = _botSettings;
    }

    public static class Discord
    {
        public static async Task DoChannelCheck(DiscordSocketClient client)
        {
            var guild = client.Guilds.ElementAt(0);

            if(botSettings.GuildId == 0)
            {
                botSettings.GuildId = guild.Id;
                botSettings.Save();
            }

            bool warningMessage = false;

        commandChannelCheck:
            if(botSettings.CommandChannelId == 0)
            {
                warningMessage = true;
                await guild.TextChannels.ElementAt(0).SendMessageAsync("Please set the channel for the bot to work in using **!set_command_channel <channel tag>** command.");
            }
            else
            {
                SocketTextChannel textChannel = guild.GetTextChannel(botSettings.CommandChannelId);

                if(textChannel == null)
                {
                    botSettings.CommandChannelId = 0;
                    botSettings.Save();
                    goto commandChannelCheck;
                }
            }

        logChannelCheck:
            if(botSettings.LogChannelId == 0)
            {
                warningMessage = true;
                await guild.TextChannels.ElementAt(0).SendMessageAsync("Please set the channel for the bot to write logs using **!set_log_channel <channel tag>** command.");
            }
            else
            {
                SocketTextChannel textChannel = guild.GetTextChannel(botSettings.LogChannelId);

                if(textChannel == null)
                {
                    botSettings.LogChannelId = 0;
                    botSettings.Save();
                    goto logChannelCheck;
                }

                await textChannel.SendMessageAsync("Bot is started. :zombie:");
            }

        publicChannelCheck:
            if(botSettings.PublicChannelId == 0)
            {
                warningMessage = true;
                await guild.TextChannels.ElementAt(0).SendMessageAsync("Please set the channel for the bot to accept commands in a public channel using **!set_public_channel <channel tag>** command.");
            }
            else
            {
                SocketTextChannel textChannel = guild.GetTextChannel(botSettings.PublicChannelId);

                if(textChannel == null)
                {
                    botSettings.PublicChannelId = 0;
                    botSettings.Save();
                    goto publicChannelCheck;
                }
            }

            if(warningMessage)
                await guild.TextChannels.ElementAt(0).SendMessageAsync("Bot won't accept any other commands until the steps above step(s) are completed. @everyone");
        }

        public static void OrganizeCommands(CommandService commandService)
        {
            List<CommandInfo> commands = commandService.Commands.ToList();

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
                case "AdminCommands":    return botSettings.CommandChannelId;
                case "PZServerCommands": return botSettings.CommandChannelId;
                case "BotCommands":      return botSettings.CommandChannelId;
                case "UserCommands":     return botSettings.PublicChannelId;
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
