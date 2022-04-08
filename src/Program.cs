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
    private static readonly string     botToken   = Environment.GetEnvironmentVariable("EB_DISCORD_BOT_TOKEN");
    public const string                botVersion = "v1.0";

    public static Settings.BotSettings botSettings;
    public static DiscordSocketClient  client;
    public static CommandService       commands;
    public static IServiceProvider     services;
    public static CommandHandler       commandHandler;
    public static DateTime             startTime = DateTime.Now;

    private static void Main(string[] args) => MainAsync().GetAwaiter().GetResult();

    private static async Task MainAsync()
    {
        if(string.IsNullOrEmpty(botToken))
        {
            Console.WriteLine("Couldn't retrieve bot token from environment variable.\nPlease refer to https://github.com/egebilecen/PZServerDiscordBot and see README.md file about setting up environment variable.");
            await Task.Delay(-1);
        }

        if(!File.Exists(Settings.BotSettings.settingsFile))
        {
            botSettings = new Settings.BotSettings();
            botSettings.Save();
        }
        else botSettings = JsonConvert.DeserializeObject<Settings.BotSettings>(File.ReadAllText(Settings.BotSettings.settingsFile));

        foreach(Process process in Process.GetProcesses())
            if(process.ProcessName.Contains("java"))
                ServerUtility.initialJavaProcessCount++;

        Scheduler.AddItem(new ScheduleItem("ServerRestart",
                                           botSettings.ServerScheduleSettings.ServerRestartSchedule,
                                           Schedules.ServerRestart,
                                           null));
        Scheduler.AddItem(new ScheduleItem("ServerRestartAnnouncer",
                                           1,
                                           Schedules.ServerRestartAnnouncer,
                                           null));
        Scheduler.Start();
        
    #if !DEBUG
        ServerUtility.serverProcess = ServerUtility.Commands.StartServer();
    #endif

        client   = new DiscordSocketClient();
        commands = new CommandService();
        services = null;
        commandHandler = new CommandHandler(client, commands, services);

        await commandHandler.SetupAsync();
        await client.LoginAsync(TokenType.Bot, botToken);
        await client.StartAsync();

        BotUtility.Discord.OrganizeCommands();

        client.Ready += async () =>
        {
            await BotUtility.Discord.DoChannelCheck();
        };

        await Task.Delay(-1);
    }
}
