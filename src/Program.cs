using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

public static class Application
{
    public const string                botRepoURL      = "https://github.com/egebilecen/PZServerDiscordBot";
    public const string                botVersion      = "v1.2.5";
    public const float                 botVersionMajor = 1.2f;

    public static Settings.BotSettings botSettings;
    public static DiscordSocketClient  client;
    public static CommandService       commands;
    public static IServiceProvider     services;
    public static CommandHandler       commandHandler;
    public static DateTime             startTime = DateTime.UtcNow;

    private static void Main(string[] _) => MainAsync().GetAwaiter().GetResult();

    private static async Task MainAsync()
    {
        try
        {
            if(string.IsNullOrEmpty(BotUtility.Discord.GetToken()))
            {
                Console.WriteLine("Couldn't retrieve bot token from \"bot_token.txt\" file.\nPlease refer to "+botRepoURL+" and see README.md file about setting up bot token.");
                await Task.Delay(-1);
            }
        }
        catch(Exception ex)
        {
            Logger.LogException(ex);
            Console.WriteLine("An error occured while retrieving bot token. Error details are saved into "+Logger.LogFile+" file.\nPlease refer to "+botRepoURL+" and create an issue about this with the log file.");
            await Task.Delay(-1);
        }

    #if !DEBUG
        string serverFile = "./server.bat";

        if(!File.Exists(serverFile))
        {
            Console.WriteLine("Couldn't find \"server.bat\" file in the folder. Please rename the bat file you were using to start the server as \"server.bat\". For example, if you were using \"StartServer64.bat\", rename it as \"server.bat\" without quotes.");
            await Task.Delay(-1);
        }
        else
        {
            string[] lines = File.ReadAllLines(serverFile);

            for(int i=0; i < lines.Length; i++)
            {
                string line = lines[i];

                if(line.Contains(@".\jre64\bin\java.exe"))
                {
                    string[] args = line.Split(new string[] { " -" }, StringSplitOptions.None);

                    foreach(string arg in args)
                    {
                        if(arg.Contains("user.home"))
                        {
                            ServerPath.BasePath = arg.Split('=').Last() + "\\";
                        }
                    }
                }
                else if(line.Trim().ToLower() == "pause")
                {
                    List<string> newLines = new List<string>(lines);
                    newLines.RemoveAt(i);
                    File.WriteAllLines(serverFile, newLines);
                    break;
                }
            }
        }
    #endif

        if(!File.Exists(Settings.BotSettings.settingsFile))
        {
            botSettings = new Settings.BotSettings
            {
                VersionNumber = botVersionMajor
            };
            botSettings.Save();
        }
        else
        {
            botSettings = JsonConvert.DeserializeObject<Settings.BotSettings>(File.ReadAllText(Settings.BotSettings.settingsFile));
        }

        if(botSettings.VersionNumber == 0)
        {
            File.Delete(Settings.BotSettings.settingsFile);

            Console.WriteLine("Please restart the bot.");
            await Task.Delay(-1);
        }

        Scheduler.AddItem(new ScheduleItem("ServerRestart",
                                           botSettings.ServerScheduleSettings.ServerRestartSchedule,
                                           Schedules.ServerRestart,
                                           null));
        Scheduler.AddItem(new ScheduleItem("ServerRestartAnnouncer",
                                           30 * 1000,
                                           Schedules.ServerRestartAnnouncer,
                                           null));
        Scheduler.AddItem(new ScheduleItem("WorkshopItemUpdateChecker",
                                           botSettings.ServerScheduleSettings.WorkshopItemUpdateSchedule,
                                           Schedules.WorkshopItemUpdateChecker,
                                           null));
        Scheduler.AddItem(new ScheduleItem("BotVersionChecker",
                                           Convert.ToUInt64(TimeSpan.FromHours(1).TotalMilliseconds),
                                           Schedules.BotVersionChecker,
                                           null));
        Scheduler.AddItem(new ScheduleItem("AutoServerStart",
                                           Convert.ToUInt64(TimeSpan.FromSeconds(30).TotalMilliseconds),
                                           Schedules.AutoServerStart,
                                           null));
        Scheduler.Start(
            #if !DEBUG
                30 * 1000
            #else
                1000
            #endif
        );
        
    #if !DEBUG
        ServerUtility.serverProcess = ServerUtility.Commands.StartServer();
    #endif

        client   = new DiscordSocketClient();
        commands = new CommandService();
        services = null;
        commandHandler = new CommandHandler(client, commands, services);

        await commandHandler.SetupAsync();
        await client.LoginAsync(TokenType.Bot, BotUtility.Discord.GetToken());
        await client.StartAsync();
        await client.SetGameAsync("Bot Version: "+botVersion);

        BotUtility.Discord.OrganizeCommands();

        client.Ready += async () =>
        {
            await BotUtility.Discord.DoChannelCheck();

            string latestBotVersion = await BotUtility.GetLatestBotVersion();
            if(!string.IsNullOrEmpty(latestBotVersion)
            && latestBotVersion != botVersion)
                await BotUtility.Discord.GetTextChannelById(botSettings.LogChannelId).SendMessageAsync(string.Format("There is a new version (**{0}**) of bot! Current version: **{1}**. Please consider to update from {2}.", latestBotVersion, botVersion, botRepoURL));
        };

        client.Disconnected += async (ex) =>
        {
            Logger.LogException(ex);
            Logger.LogException(ex.InnerException);

            if(ex.InnerException.Message.Contains("Authentication failed"))
            {
                Console.WriteLine("Authentication failed! Be sure your discord bot token is valid.");
                await Task.Delay(-1);
            }
            else Console.WriteLine("An error occured and discord bot has been disconnected! Error details are saved into "+Logger.LogFile+" file.\nPlease refer to "+botRepoURL+" and create an issue about this with the log file.");
        };

        await Task.Delay(-1);
    }
}
