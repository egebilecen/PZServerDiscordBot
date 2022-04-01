using Discord;
using Discord.Commands;
using Discord.Interactions;
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
                ServerUtility.javaProcessCount++;

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

        //client.Log += LogAsync;
        client.Ready += () =>
        {
            var guild = client.Guilds.ElementAt(0);

            if(botSettings.GuildId == 0)
            {
                botSettings.GuildId = guild.Id;
                botSettings.Save();
            }

        channelCheck:
            if(botSettings.BotChannelId == 0)
                guild.TextChannels.ElementAt(0).SendMessageAsync("Please set the channel for the bot to work in using **!pzbot_set_channel** command.");
            else
            {
                SocketTextChannel textChannel = guild.GetTextChannel(botSettings.BotChannelId);

                if(textChannel == null)
                {
                    botSettings.BotChannelId = 0;
                    botSettings.Save();
                    goto channelCheck;
                }

                textChannel.SendMessageAsync("Bot is started. :zombie:");
            }

            return Task.CompletedTask;
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
