using Discord;
using Discord.Commands;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

public class UserCommands : ModuleBase<SocketCommandContext>
{
    public CommandService _commandService { get; set; }

    [Command("help")]
    [Summary("Gets the available commands list. (!help)")]
    public async Task Help()
    {
        List<CommandInfo> commands = _commandService.Commands.ToList();
        EmbedBuilder embedBuilder  = new EmbedBuilder();

        int totalCommands  = commands.Count;
        int fullCycleCount = 0;
        int commandCounter = 0;

        await Context.Message.AddReactionAsync(EmojiList.GreenCheck);
        await ReplyAsync("Here is the command list:");

        foreach(CommandInfo command in commands)
        {
            if(command.Remarks == "skip")
            {
                totalCommands--;
                continue;
            }

            embedBuilder.AddField(command.Name, command.Summary ?? "No description available\n");

            commandCounter++;

            if(commandCounter == 25
            || commandCounter + (25 * fullCycleCount) == totalCommands)
            {
                await ReplyAsync("", false, embedBuilder.Build());

                if(commandCounter == 25)
                    embedBuilder = new EmbedBuilder();

                commandCounter = 0;
                fullCycleCount++;
            }
        }
    }

    [Command("server_status")]
    [Summary("Gets the server status. (!server_status)")]
    public async Task ServerStatus()
    {
        await Context.Message.AddReactionAsync(EmojiList.GreenCheck);
        await ReplyAsync(ServerUtility.IsServerRunning() ? "Server is **running** :hamster:" : "Server is **dead** :skull:");
    }

    [Command("debug")]
    [Summary("Command enabled for debug purposes. (!debug)")]
    [Remarks("skip")]
    public async Task Debug()
    {
        Logger.WriteLog("["+Context.Message.Timestamp.UtcDateTime.ToString()+"]"+string.Format("[BotCommands - debug] Caller: {0}", Context.User.ToString()));
        await Context.Message.AddReactionAsync(EmojiList.GreenCheck);
    }
}
