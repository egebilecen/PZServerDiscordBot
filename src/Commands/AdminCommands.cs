using Discord;
using Discord.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

public class AdminCommands : ModuleBase<SocketCommandContext>
{
    [Command("perk_info")]
    [Summary("Displays the latest logged perk information of player. (!perk_info \"<username>\")")]
    public async Task PerkInfo(string username)
    {
        Logger.WriteLog("["+Context.Message.Timestamp.UtcDateTime.ToString()+"]"+string.Format("[BotCommands - perk_info] Caller: {0}, Params: {1}", Context.User.ToString(), username));
        var userPerkDataList = ServerLogParsers.PerkLog.Get();

        if(!userPerkDataList.ContainsKey(username))
        {
            await Context.Message.AddReactionAsync(EmojiList.RedCross);
            await ReplyAsync(string.Format("Couldn't find any perk log related to username **{0}**.", username));
        }
        else
        {
            await Context.Message.AddReactionAsync(EmojiList.GreenCheck);

            var userPerkData   = userPerkDataList[username];
            int totalPerks     = userPerkData.Perks.Count;
            int fullCycleCount = 0;
            int perkCounter    = 0;
            EmbedBuilder embedBuilder = new EmbedBuilder();

            var logTimestamp = new DateTimeOffset(DateTime.Parse(userPerkData.LogDate)).ToUnixTimeSeconds();

            await ReplyAsync(string.Format("Perk Information of **{0}** in <t:{1}:f>:", username, logTimestamp));
            
            foreach(KeyValuePair<string, object> perkInfo in userPerkData.Perks)
            {
                embedBuilder.AddField(perkInfo.Key, perkInfo.Value.ToString());
                perkCounter++;

                if(perkCounter == 25
                || perkCounter + (25 * fullCycleCount) == totalPerks)
                {
                    await ReplyAsync("", false, embedBuilder.Build());

                    if(perkCounter == 25)
                        embedBuilder = new EmbedBuilder();

                    perkCounter = 0;
                    fullCycleCount++;
                }
            }
        }
    }
}
