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

public class Program
{
    private Model.BotSettings   botSettings;

    private DiscordSocketClient client;
    private CommandService      commands;
    private IServiceProvider    services;
    private CommandHandler      commandHandler;

    static void Main(string[] args) => new Program().MainAsync().GetAwaiter().GetResult();

    public async Task MainAsync()
    {
        if(!File.Exists(Model.BotSettings.settingsFile))
        {
            botSettings = new Model.BotSettings();
            botSettings.Save();
        }
        else botSettings = JsonConvert.DeserializeObject<Model.BotSettings>(File.ReadAllText(Model.BotSettings.settingsFile));

        foreach(Process process in Process.GetProcesses())
            if(process.ProcessName.Contains("java"))
                ServerUtility.initialJavaProcessCount++;

        ServerLogParsers.PerkLog.Init(botSettings);
        BotUtility.Init(botSettings);
        //ServerUtility.serverProcess = ServerUtility.StartServer();

        client   = new DiscordSocketClient();
        commands = new CommandService();
        services = new ServiceCollection()
                   .AddSingleton(botSettings)
                   .AddSingleton(commands)
                   .BuildServiceProvider();
        commandHandler = new CommandHandler(client, commands, services, botSettings);

        await commandHandler.SetupAsync();
        await client.LoginAsync(TokenType.Bot, Environment.GetEnvironmentVariable("EB_DISCORD_BOT_TOKEN"));
        await client.StartAsync();

        BotUtility.Discord.OrganizeCommands(commands);

        //client.Log += LogAsync;
        client.Ready += async () =>
        {
            await BotUtility.Discord.DoChannelCheck(client);
        };

        await Task.Delay(-1);
    }

    private Task LogAsync(LogMessage log)
    {
        Console.WriteLine(string.Format("[{0}|{1}{2}] {3}", 
                                        log.Source, 
                                        log.Severity, 
                                        log.Exception != null ? "|EXCEPTION" : "",
                                        log.Message));

        return Task.CompletedTask;
    }
}
