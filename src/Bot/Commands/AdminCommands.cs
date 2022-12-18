using Discord.Commands;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

public class AdminCommands : ModuleBase<SocketCommandContext>
{
    [Command("debug")]
    [Summary("Command enabled for debug purposes. (!debug ...)")]
    [Remarks("skip")]
    public async Task Debug(string param1="", string param2="", string param3="")
    {
        await Context.Message.AddReactionAsync(EmojiList.GreenCheck);
    }
}
