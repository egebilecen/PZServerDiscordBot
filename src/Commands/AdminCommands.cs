using Discord.Commands;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

public class AdminCommands : ModuleBase<SocketCommandContext>
{
    [Command("debug")]
    [Summary("Command enabled for debug purposes. (!debug ...)")]
    //[Remarks("skip")]
    public async Task Debug(string param1="", string param2="", string param3="")
    {
        // I got some reports about bot missing some workshop updates.
        // This command will print some debug information about that.
        var fetchDetails = Task.Run(async () => await SteamWebAPI.GetWorkshopItemDetails(new string[] { param1 }));
        var itemDetails  = fetchDetails.Result;

        File.WriteAllText("./debug_result.json", JsonConvert.SerializeObject(itemDetails, Formatting.Indented));

        await Context.Message.AddReactionAsync(EmojiList.GreenCheck);
        await BotUtility.Discord.SendEmbeddedMessage(Context.Channel, new List<KeyValuePair<string, string>>
        {
            new KeyValuePair<string, string>("Application.startTime", Application.StartTime.ToString()),
            new KeyValuePair<string, string>("Application.startTime (timestamp)", ((DateTimeOffset)Application.StartTime).ToUnixTimeSeconds().ToString()),
        });
        await Context.Channel.SendFileAsync("./debug_result.json");
    }
}
