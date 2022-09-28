using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Newtonsoft.Json;
using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;

public static class Application
{
    public const string                botVersion      = "v1.2";
    public const float                 botVersionMajor = 1.2f;

    public static Settings.BotSettings botSettings;
    public static DiscordSocketClient  client;
    public static CommandService       commands;
    public static IServiceProvider     services;
    public static CommandHandler       commandHandler;
    public static DateTime             startTime = DateTime.Now;

    private static void Main(string[] _) => MainAsync().GetAwaiter().GetResult();

    private static async Task MainAsync()
    {
        try
        {
            if(string.IsNullOrEmpty(BotUtility.GetDiscordBotToken()))
            {
                Console.WriteLine("Couldn't retrieve bot token from \"bot_token.txt\" file.\nPlease refer to https://github.com/egebilecen/PZServerDiscordBot and see README.md file about setting up environment variable.");
                await Task.Delay(-1);
            }
        }
        catch(Exception ex)
        {
            Logger.LogException(ex);
            Console.WriteLine("An error occured while retrieving bot token. Error details are saved into "+Logger.LogFile+" file.\nPlease refer to https://github.com/egebilecen/PZServerDiscordBot and create an issue about this with the log file.");
            await Task.Delay(-1);
        }

    #if !DEBUG
        if(!File.Exists("./server.bat"))
        {
            Console.WriteLine("Couldn't find \"server.bat\" file in the folder. Please rename the bat file you were using to start the server as \"server.bat\". For example, if you were using \"StartServer64.bat\", rename it as \"server.bat\" without quotes.");
            await Task.Delay(-1);
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

        foreach(Process process in Process.GetProcesses())
            if(process.ProcessName.Contains("java"))
                ServerUtility.initialJavaProcessCount++;

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
        await client.LoginAsync(TokenType.Bot, BotUtility.GetDiscordBotToken());
        await client.StartAsync();

        BotUtility.Discord.OrganizeCommands();

        client.Ready += async () =>
        {
            await BotUtility.Discord.DoChannelCheck();
        };

        client.Disconnected += async (ex) =>
        {
            Logger.LogException(ex);
            Logger.LogException(ex.InnerException);

            if(ex.InnerException.Message.Contains("Authentication failed"))
            {
                Console.WriteLine("Authentication failed! Be sure your discord bot token is valid.");
            }
            else Console.WriteLine("An error occured and discord bot has been disconnected! Error details are saved into "+Logger.LogFile+" file.\nPlease refer to https://github.com/egebilecen/PZServerDiscordBot and create an issue about this with the log file.");
            
            await Task.Delay(-1);
        };

        await Task.Delay(-1);
    }
}
