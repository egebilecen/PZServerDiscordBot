using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

public static class Application
{
    public static Settings.BotSettings botSettings;
    public static DiscordSocketClient  client;
    public static CommandService       commands;
    public static IServiceProvider     services;
    public static CommandHandler       commandHandler;

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

        Scheduler.AddItem(new ScheduleItem("ServerReboot",
                                           Convert.ToUInt64(TimeSpan.FromHours(6).TotalMinutes),
                                           Schedules.ServerReboot,
                                           null));
        Scheduler.Start();

        ServerUtility.serverProcess = ServerUtility.Commands.StartServer();

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
            await BotUtility.Discord.DoChannelCheck(client);
        };

        await Task.Delay(-1);
    }
}
