#define EXPORT_DEFAULT_LOCALIZATION

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
    public static readonly SemanticVersion BotVersion = new SemanticVersion(1, 8, 0, DevelopmentStage.Release);
    public static Settings.BotSettings     BotSettings;

    public static DiscordSocketClient  Client;
    public static CommandService       Commands;
    public static IServiceProvider     Services;
    public static CommandHandler       CommandHandler;
    public static DateTime             StartTime = DateTime.UtcNow;

    private static void Main(string[] _) => MainAsync().GetAwaiter().GetResult();

    private static async Task MainAsync()
    {
    #if DEBUG
        Console.WriteLine(Localization.Get("warn_debug_mode"));
    #endif

        try
        {
            if(string.IsNullOrEmpty(DiscordUtility.GetToken()))
            {
                Console.WriteLine(Localization.Get("err_bot_token").KeyFormat(("repo_url", BotRepoURL)));
                await Task.Delay(-1);
            }
        }
        catch(Exception ex)
        {
            Logger.LogException(ex);
            Console.WriteLine(Localization.Get("err_retv_bot_token").KeyFormat(("log_file", Logger.LogFile), ("repo_url", BotRepoURL)));
            await Task.Delay(-1);
        }

    #if !DEBUG
        string serverFile = "./server.bat";

        if(!File.Exists(serverFile))
        {
            Console.WriteLine(Localization.Get("err_serv_bat"));
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

        if(!Directory.Exists(Localization.LocalizationPath))
            Directory.CreateDirectory(Localization.LocalizationPath);

    #if EXPORT_DEFAULT_LOCALIZATION
        Localization.ExportDefault();
    #endif

        Scheduler.AddItem(new ScheduleItem("ServerRestart",
                                           "Server Restart",
                                           BotSettings.ServerScheduleSettings.ServerRestartSchedule,
                                           Schedules.ServerRestart,
                                           null));
        Scheduler.AddItem(new ScheduleItem("ServerRestartAnnouncer",
                                           "Server Restart Announcer",
                                           Convert.ToUInt64(TimeSpan.FromSeconds(30).TotalMilliseconds),
                                           Schedules.ServerRestartAnnouncer,
                                           null));
        Scheduler.AddItem(new ScheduleItem("WorkshopItemUpdateChecker",
                                           "Workshop Mod Update Checker",
                                           BotSettings.ServerScheduleSettings.WorkshopItemUpdateSchedule,
                                           Schedules.WorkshopItemUpdateChecker,
                                           null));
        Scheduler.AddItem(new ScheduleItem("AutoServerStart",
                                           "Auto Server Starter",
                                           Convert.ToUInt64(TimeSpan.FromSeconds(30).TotalMilliseconds),
                                           Schedules.AutoServerStart,
                                           null));
        Scheduler.AddItem(new ScheduleItem("BotVersionChecker",
                                           "Bot New Version Checker",
                                           Convert.ToUInt64(TimeSpan.FromMinutes(5).TotalMilliseconds),
                                           Schedules.BotVersionChecker,
                                           null));
        Scheduler.Start(1000);
        
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
        await Client.SetGameAsync(Localization.Get("info_disc_act_bot_ver").KeyFormat(("version", BotVersion)));

        DiscordUtility.OrganizeCommands();

        Client.Ready += async () =>
        {
            await DiscordUtility.DoChannelCheck();
            await BotUtility.NotifyLatestBotVersion();
        };

        Client.Disconnected += async (ex) =>
        {
            Logger.LogException(ex);
            Logger.LogException(ex.InnerException);

            if(ex.InnerException.Message.Contains("Authentication failed"))
            {
                Console.WriteLine(Localization.Get("err_disc_auth_fail"));
                await Task.Delay(-1);
            }
            else Console.WriteLine(Localization.Get("err_disc_disconn").KeyFormat(("log_file", Logger.LogFile), ("repo_url", BotRepoURL)));
        };

        await Task.Delay(-1);
    }
}
