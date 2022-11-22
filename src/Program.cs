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
    public const string                    BotRepoURL = "https://github.com/egebilecen/PZServerDiscordBot";
    public static readonly SemanticVersion BotVersion = new SemanticVersion(1, 7, 0, DevelopmentStage.None);
    public static Settings.BotSettings     BotSettings;

    public static DiscordSocketClient  Client;
    public static CommandService       Commands;
    public static IServiceProvider     Services;
    public static CommandHandler       CommandHandler;
    public static DateTime             StartTime = DateTime.UtcNow;

    private static void Main(string[] _) => MainAsync().GetAwaiter().GetResult();

    private static async Task MainAsync()
    {
        try
        {
            if(string.IsNullOrEmpty(DiscordUtility.GetToken()))
            {
                Console.WriteLine("Couldn't retrieve bot token from \"bot_token.txt\" file.\nPlease refer to "+BotRepoURL+" and see README.md file about setting up bot token.");
                await Task.Delay(-1);
            }
        }
        catch(Exception ex)
        {
            Logger.LogException(ex);
            Console.WriteLine("An error occured while retrieving bot token. Error details are saved into "+Logger.LogFile+" file.\nPlease refer to "+BotRepoURL+" and create an issue about this with the log file.");
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

                            if(Directory.Exists(ServerPath.BasePath + "Zomboid\\"))
                                ServerPath.BasePath += "Zomboid\\";
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

        if(!File.Exists(Settings.BotSettings.SettingsFile))
        {
            BotSettings = new Settings.BotSettings
            {
                Version = BotVersion
            };
            BotSettings.Save();
        }
        else
        {
            BotSettings = JsonConvert.DeserializeObject<Settings.BotSettings>(File.ReadAllText(Settings.BotSettings.SettingsFile));
        }

        if(BotSettings.Version == null)
        {
            BotSettings.Version = BotVersion;
            BotSettings.Save();
        }

        Scheduler.AddItem(new ScheduleItem("ServerRestart",
                                           "Server Restart",
                                           BotSettings.ServerScheduleSettings.ServerRestartSchedule,
                                           Schedules.ServerRestart,
                                           null));
        Scheduler.AddItem(new ScheduleItem("ServerRestartAnnouncer",
                                           "Server Restart Announcer",
                                           30 * 1000,
                                           Schedules.ServerRestartAnnouncer,
                                           null));
        Scheduler.AddItem(new ScheduleItem("WorkshopItemUpdateChecker",
                                           "Workshop Mod Update Checker",
                                           BotSettings.ServerScheduleSettings.WorkshopItemUpdateSchedule,
                                           Schedules.WorkshopItemUpdateChecker,
                                           null));
        Scheduler.AddItem(new ScheduleItem("BotVersionChecker",
                                           "Bot New Version Checker",
                                           Convert.ToUInt64(TimeSpan.FromHours(1).TotalMilliseconds),
                                           Schedules.BotVersionChecker,
                                           null));
        Scheduler.AddItem(new ScheduleItem("AutoServerStart",
                                           "Auto Server Starter",
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
        ServerUtility.ServerProcess = ServerUtility.Commands.StartServer();
    #endif

        Client   = new DiscordSocketClient();
        Commands = new CommandService();
        Services = null;
        CommandHandler = new CommandHandler(Client, Commands, Services);

        await CommandHandler.SetupAsync();
        await Client.LoginAsync(TokenType.Bot, DiscordUtility.GetToken());
        await Client.StartAsync();
        await Client.SetGameAsync("Bot Version: "+BotVersion);

        DiscordUtility.OrganizeCommands();

        Client.Ready += async () =>
        {
            await DiscordUtility.DoChannelCheck();
            await BotUtility.CheckLatestBotVersion();
        };

        Client.Disconnected += async (ex) =>
        {
            Logger.LogException(ex);
            Logger.LogException(ex.InnerException);

            if(ex.InnerException.Message.Contains("Authentication failed"))
            {
                Console.WriteLine("Authentication failed! Be sure your discord bot token is valid.");
                await Task.Delay(-1);
            }
            else Console.WriteLine("An error occured and discord bot has been disconnected! Error details are saved into "+Logger.LogFile+" file.\nPlease refer to "+BotRepoURL+" and create an issue about this with the log file.");
        };

        await Task.Delay(-1);
    }
}
