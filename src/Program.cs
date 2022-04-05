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
    public const string                botVersion = "v0.1.4";
    public static Settings.BotSettings botSettings;
    public static DiscordSocketClient  client;
    public static CommandService       commands;
    public static IServiceProvider     services;
    public static CommandHandler       commandHandler;
    public static DateTime             startTime = DateTime.Now;

    private static void Main(string[] args) => MainAsync().GetAwaiter().GetResult();

    private static async Task MainAsync()
    {
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
        await client.LoginAsync(TokenType.Bot, Environment.GetEnvironmentVariable("EB_DISCORD_BOT_TOKEN"));
        await client.StartAsync();

        BotUtility.Discord.OrganizeCommands();

        client.Ready += async () =>
        {
            await BotUtility.Discord.DoChannelCheck();
        };

        await Task.Delay(-1);
    }
}
