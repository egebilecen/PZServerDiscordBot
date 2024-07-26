#define EXPORT_DEFAULT_LOCALIZATION

using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Threading.Tasks;

public static class Application
{
    public const string                    BotRepoURL = "https://github.com/egebilecen/PZServerDiscordBot";
    public static readonly SemanticVersion BotVersion = new SemanticVersion(1, 11, 4, DevelopmentStage.Release);
    public static Settings.BotSettings     BotSettings;

    public static DiscordSocketClient  Client;
    public static CommandService       Commands;
    public static IServiceProvider     Services;
    public static CommandHandler       CommandHandler;
    public static DateTime             StartTime = DateTime.UtcNow;

    private static bool botInitialCheck = false;

    private static void Main(string[] _) => MainAsync().GetAwaiter().GetResult();

    private static async Task MainAsync()
    {
        if(!File.Exists(Settings.BotSettings.SettingsFile))
        {
            BotSettings = new Settings.BotSettings();
            BotSettings.Save();
        }
        else
        {
            BotSettings = JsonConvert.DeserializeObject<Settings.BotSettings>(File.ReadAllText(Settings.BotSettings.SettingsFile), 
                new JsonSerializerSettings{ObjectCreationHandling = ObjectCreationHandling.Replace});
        }

        Localization.Load();
    #if EXPORT_DEFAULT_LOCALIZATION
        Localization.ExportDefault();
    #endif

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
        ServerPath.CheckCustomBasePath();
    #endif

        if(!Directory.Exists(Localization.LocalizationPath))
            Directory.CreateDirectory(Localization.LocalizationPath);

        Scheduler.AddItem(new ScheduleItem("ServerRestart",
                                           Localization.Get("sch_name_serverrestart"),
                                           BotSettings.ServerScheduleSettings.GetServerRestartSchedule(),
                                           Schedules.ServerRestart,
                                           null));
        Scheduler.AddItem(new ScheduleItem("ServerRestartAnnouncer",
                                           Localization.Get("sch_name_serverrestartannouncer"),
                                           Convert.ToUInt64(TimeSpan.FromSeconds(30).TotalMilliseconds),
                                           Schedules.ServerRestartAnnouncer,
                                           null));
        Scheduler.AddItem(new ScheduleItem("WorkshopItemUpdateChecker",
                                           Localization.Get("sch_name_workshopitemupdatechecker"),
                                           BotSettings.ServerScheduleSettings.WorkshopItemUpdateSchedule,
                                           Schedules.WorkshopItemUpdateChecker,
                                           null));
        Scheduler.AddItem(new ScheduleItem("AutoServerStart",
                                           Localization.Get("sch_name_autoserverstarter"),
                                           Convert.ToUInt64(TimeSpan.FromSeconds(30).TotalMilliseconds),
                                           Schedules.AutoServerStart,
                                           null));
        Scheduler.AddItem(new ScheduleItem("BotVersionChecker",
                                           Localization.Get("sch_name_botnewversioncchecker"),
                                           Convert.ToUInt64(TimeSpan.FromMinutes(5).TotalMilliseconds),
                                           Schedules.BotVersionChecker,
                                           null));
        Localization.AddSchedule();
        Scheduler.Start(1000);
        
    #if !DEBUG
        ServerUtility.ServerProcess = ServerUtility.Commands.StartServer();
    #endif

        Client   = new DiscordSocketClient(new DiscordSocketConfig() { GatewayIntents = GatewayIntents.All });
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
            if(!botInitialCheck)
            {
                botInitialCheck = true;

                await DiscordUtility.DoChannelCheck();
                await BotUtility.NotifyLatestBotVersion();
                await Localization.CheckUpdate();
            }
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
            //else Console.WriteLine(Localization.Get("err_disc_disconn").KeyFormat(("log_file", Logger.LogFile), ("repo_url", BotRepoURL)));
        };

        await Task.Delay(-1);
    }
}
