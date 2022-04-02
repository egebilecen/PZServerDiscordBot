using Discord.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

public class PZServerCommands : ModuleBase<SocketCommandContext>
{
    [Command("server_msg")]
    [Summary("Broadcasts a message to all players in the server. (!server_msg \"<message>\")")]
    public async Task ServerMessage(string message)
    {
        ServerUtility.Commands.ServerMsg(message);
        Logger.WriteLog("["+Context.Message.Timestamp.UtcDateTime.ToString()+"]"+string.Format("[PZServerCommand - server_msg] Caller: {0}, Params: {1}", Context.User.ToString(), message));

        await Context.Message.AddReactionAsync(EmojiList.GreenCheck);
    }

    [Command("start_server")]
    [Summary("Starts the server. (!start_server)")]
    public async Task StartServer()
    {
        if(ServerUtility.IsServerRunning())
        {
            await Context.Message.AddReactionAsync(EmojiList.RedCross);
            await ReplyAsync("Server is already running.");
        }
        else
        {
            ServerUtility.serverProcess = ServerUtility.Commands.StartServer();
            
            Logger.WriteLog("["+Context.Message.Timestamp.UtcDateTime.ToString()+"]"+string.Format("[PZServerCommand - start_server] Caller: {0}", Context.User.ToString()));
            await Context.Message.AddReactionAsync(EmojiList.GreenCheck);
            await ReplyAsync("Server should be on it's way to get started. This process may take a while. Please check the server status in 1 or 2 minute.");
        }
    }

    [Command("stop_server")]
    [Summary("Saves and stops the server. (!stop_server)")]
    public async Task StopServer()
    {
        if(!ServerUtility.IsServerRunning())
        {
            await Context.Message.AddReactionAsync(EmojiList.RedCross);
            await ReplyAsync("Server is already stopped.");
            return;
        }

        ServerUtility.Commands.StopServer();
        Logger.WriteLog("["+Context.Message.Timestamp.UtcDateTime.ToString()+"]"+string.Format("[PZServerCommand - stop_server] Caller: {0}", Context.User.ToString()));

        await Context.Message.AddReactionAsync(EmojiList.GreenCheck);
    }

    [Command("restart_server")]
    [Summary("Restarts the server. (!restart_server)")]
    public async Task RestartServer()
    {
        if(ServerUtility.IsServerRunning())
        {
            Logger.WriteLog("["+Context.Message.Timestamp.UtcDateTime.ToString()+"]"+string.Format("[PZServerCommand - restart_server] Caller: {0}", Context.User.ToString()));
            
            await Context.Message.AddReactionAsync(EmojiList.GreenCheck);
            await ReplyAsync("Restarting server.");

            await StopServer();

            while(ServerUtility.IsServerRunning())
                await Task.Delay(250);

            await StartServer();
        }
        else
        {
            await Context.Message.AddReactionAsync(EmojiList.RedCross);
            await ReplyAsync("Server is not running. Start the server first.");
            return;
        }
    }

    [Command("save_server")]
    [Summary("Saves the current world. (!save_server)")]
    public async Task SaveServer()
    {
        ServerUtility.Commands.SaveServer();
        Logger.WriteLog("["+Context.Message.Timestamp.UtcDateTime.ToString()+"]"+string.Format("[PZServerCommand - save_server] Caller: {0}", Context.User.ToString()));

        await Context.Message.AddReactionAsync(EmojiList.GreenCheck);
    }

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

            List<KeyValuePair<string, string>> perkList = new List<KeyValuePair<string, string>>();

            var userPerkData = userPerkDataList[username];
            var logTimestamp = new DateTimeOffset(DateTime.Parse(userPerkData.LogDate)).ToUnixTimeSeconds();

            foreach(KeyValuePair<string, int> perk in userPerkData.Perks)
                perkList.Add(new KeyValuePair<string, string>(perk.Key, perk.Value.ToString()));

            await ReplyAsync(string.Format("Perk Information of **{0}** (Recorded <t:{1}:R>):", username, logTimestamp));
            await BotUtility.Discord.SendEmbeddedMessage(Context.Message, perkList);
        }
    }

    [Command("add_user")]
    [Summary("Adds a new user to the whitelist. (!add_user \"<username>\" \"<password>\")")]
    public async Task AddUser(string username, string password)
    {
        ServerUtility.Commands.AddUser(username, password);
        Logger.WriteLog("["+Context.Message.Timestamp.UtcDateTime.ToString()+"]"+string.Format("[PZServerCommand - add_user] Caller: {0}, Params: {1}", Context.User.ToString(), username+","+password));

        await Context.Message.AddReactionAsync(EmojiList.GreenCheck);
    }

    [Command("addusertowhitelist")]
    [Summary("Adds a single user connected with a password to the whitelist. (!addusertowhitelist \"<username>\")")]
    public async Task AddUserToWhiteList(string username)
    {
        ServerUtility.Commands.AddUserToWhiteList(username);
        Logger.WriteLog("["+Context.Message.Timestamp.UtcDateTime.ToString()+"]"+string.Format("[PZServerCommand - addusertowhitelist] Caller: {0}, Params: {1}", Context.User.ToString(), username));

        await Context.Message.AddReactionAsync(EmojiList.GreenCheck);
    }

    [Command("remove_user_from_white_list")]
    [Summary("Removes a single user connected with a password to the whitelist. (!remove_user_from_whitelist \"<username>\")")]
    public async Task RemoveUserFromWhiteList(string username)
    {
        ServerUtility.Commands.RemoveUserFromWhiteList(username);
        Logger.WriteLog("["+Context.Message.Timestamp.UtcDateTime.ToString()+"]"+string.Format("[PZServerCommand - remove_user_from_whitelist] Caller: {0}, Params: {1}", Context.User.ToString(), username));

        await Context.Message.AddReactionAsync(EmojiList.GreenCheck);
    }

    [Command("ban_steamid")]
    [Summary("Bans a Steam ID. (!ban_steamid <steam id>)")]
    public async Task BanId(ulong id)
    {
        ServerUtility.Commands.BanId(id);
        Logger.WriteLog("["+Context.Message.Timestamp.UtcDateTime.ToString()+"]"+string.Format("[PZServerCommand - ban_steamid] Caller: {0}, Params: {1}", Context.User.ToString(), id.ToString()));

        await Context.Message.AddReactionAsync(EmojiList.GreenCheck);
    }

    [Command("unban_steamid")]
    [Summary("Unbans a Steam ID. (!unban_steamid <steam id>)")]
    public async Task UnbanId(ulong id)
    {
        ServerUtility.Commands.UnbanId(id);
        Logger.WriteLog("["+Context.Message.Timestamp.UtcDateTime.ToString()+"]"+string.Format("[PZServerCommand - unban_steamid] Caller: {0}, Params: {1}", Context.User.ToString(), id.ToString()));

        await Context.Message.AddReactionAsync(EmojiList.GreenCheck);
    }

    [Command("ban_user")]
    [Summary("Bans a user. (!ban_user \"<username>\")")]
    public async Task BanUser(string username)
    {
        ServerUtility.Commands.BanUser(username);
        Logger.WriteLog("["+Context.Message.Timestamp.UtcDateTime.ToString()+"]"+string.Format("[PZServerCommand - ban_user] Caller: {0}, Params: {1}", Context.User.ToString(), username));

        await Context.Message.AddReactionAsync(EmojiList.GreenCheck);
    }

    [Command("unban_user")]
    [Summary("Unbans a user. (!unbanuser \"<username>\")")]
    public async Task UnbanUser(string username)
    {
        ServerUtility.Commands.UnbanUser(username);
        Logger.WriteLog("["+Context.Message.Timestamp.UtcDateTime.ToString()+"]"+string.Format("[PZServerCommand - unban_user] Caller: {0}, Params: {1}", Context.User.ToString(), username));

        await Context.Message.AddReactionAsync(EmojiList.GreenCheck);
    }

    [Command("make_admin")]
    [Summary("Gives admin rights to a user. (!make_admin \"<username>\")")]
    public async Task GrantAdmin(string username)
    {
        ServerUtility.Commands.GrantAdmin(username);
        Logger.WriteLog("["+Context.Message.Timestamp.UtcDateTime.ToString()+"]"+string.Format("[PZServerCommand - make_admin] Caller: {0}, Params: {1}", Context.User.ToString(), username));

        await Context.Message.AddReactionAsync(EmojiList.GreenCheck);
    }

    [Command("remove_admin")]
    [Summary("Removes admin rights to a user. (!remove_admin \"<username>\")")]
    public async Task RemoveAdmin(string username)
    {
        ServerUtility.Commands.RemoveAdmin(username);
        Logger.WriteLog("["+Context.Message.Timestamp.UtcDateTime.ToString()+"]"+string.Format("[PZServerCommand - remove_admin] Caller: {0}, Params: {1}", Context.User.ToString(), username));

        await Context.Message.AddReactionAsync(EmojiList.GreenCheck);
    }

    [Command("kick_user")]
    [Summary("Kicks a user from the server. (!kick_user \"<username>\")")]
    public async Task KickUser(string username)
    {
        ServerUtility.Commands.KickUser(username);
        Logger.WriteLog("["+Context.Message.Timestamp.UtcDateTime.ToString()+"]"+string.Format("[PZServerCommand - kick_user] Caller: {0}, Params: {1}", Context.User.ToString(), username));

        await Context.Message.AddReactionAsync(EmojiList.GreenCheck);
    }

    [Command("start_rain")]
    [Summary("Starts rain on the server. (!startrain)")]
    public async Task StartRain()
    {
        ServerUtility.Commands.StartRain();
        Logger.WriteLog("["+Context.Message.Timestamp.UtcDateTime.ToString()+"]"+string.Format("[PZServerCommand - start_rain] Caller: {0}", Context.User.ToString()));

        await Context.Message.AddReactionAsync(EmojiList.GreenCheck);
    }

    [Command("stop_rain")]
    [Summary("Stops rain on the server. (!stoprain)")]
    public async Task StopRain()
    {
        ServerUtility.Commands.StopRain();
        Logger.WriteLog("["+Context.Message.Timestamp.UtcDateTime.ToString()+"]"+string.Format("[PZServerCommand - stop_rain] Caller: {0}", Context.User.ToString()));

        await Context.Message.AddReactionAsync(EmojiList.GreenCheck);
    }

    [Command("teleport")]
    [Summary("Teleports a player. (!teleport \"<username1>\" \"<username2>\")")]
    public async Task Teleport(string username1, string username2)
    {
        ServerUtility.Commands.Teleport(username1, username2);
        Logger.WriteLog("["+Context.Message.Timestamp.UtcDateTime.ToString()+"]"+string.Format("[PZServerCommand - teleport] Caller: {0}", Context.User.ToString()));

        await Context.Message.AddReactionAsync(EmojiList.GreenCheck);
    }

    [Command("add_item")]
    [Summary("Gives an item to the player. (!add_item \"<username>\" \"<module.item>\")")]
    public async Task AddItem(string username, string item)
    {
        ServerUtility.Commands.AddItem(username, item);
        Logger.WriteLog("["+Context.Message.Timestamp.UtcDateTime.ToString()+"]"+string.Format("[PZServerCommand - add_item] User: {0}, Params: {1}", Context.User.ToString(), username+","+item));

        await Context.Message.AddReactionAsync(EmojiList.GreenCheck);
    }

    [Command("add_xp")]
    [Summary("Gives XP to a player. (!addxp \"<username>\" \"<perk>\" <xp>)")]
    public async Task AddXP(string username, string perk, uint xp)
    {
        ServerUtility.Commands.AddXP(username, perk, xp);
        Logger.WriteLog("["+Context.Message.Timestamp.UtcDateTime.ToString()+"]"+string.Format("[PZServerCommand - add_xp] Caller: {0}, Params: {1}", Context.User.ToString(), username+","+perk+","+xp));

        await Context.Message.AddReactionAsync(EmojiList.GreenCheck);
    }

    [Command("chopper")]
    [Summary("Places a helicopter event on a random player. (!chopper)")]
    public async Task Chopper()
    {
        ServerUtility.Commands.Chopper();
        Logger.WriteLog("["+Context.Message.Timestamp.UtcDateTime.ToString()+"]"+string.Format("[PZServerCommand - chopper] Caller: {0}", Context.User.ToString()));

        await Context.Message.AddReactionAsync(EmojiList.GreenCheck);
    }

    [Command("godmode")]
    [Summary("Makes a player invincible. (!godmode \"<username>\")")]
    public async Task GodMode(string username)
    {
        ServerUtility.Commands.GodMode(username);
        Logger.WriteLog("["+Context.Message.Timestamp.UtcDateTime.ToString()+"]"+string.Format("[PZServerCommand - godmode] Caller: {0}, Params: {1}", Context.User.ToString(), username));

        await Context.Message.AddReactionAsync(EmojiList.GreenCheck);
    }

    [Command("invisible")]
    [Summary("Makes a player invisible to zombies. (!invisible \"<username>\")")]
    public async Task Invisible(string username)
    {
        ServerUtility.Commands.Invisible(username);
        Logger.WriteLog("["+Context.Message.Timestamp.UtcDateTime.ToString()+"]"+string.Format("[PZServerCommand - invisible] Caller: {0}, Params: {1}", Context.User.ToString(), username));

        await Context.Message.AddReactionAsync(EmojiList.GreenCheck);
    }

    [Command("noclip")]
    [Summary("Allows a player to pass through solid objects. (!noclip \"<username>\")")]
    public async Task NoClip(string username)
    {
        ServerUtility.Commands.NoClip(username);
        Logger.WriteLog("["+Context.Message.Timestamp.UtcDateTime.ToString()+"]"+string.Format("[PZServerCommand - noclip] Caller: {0}, Params: {1}", Context.User.ToString(), username));

        await Context.Message.AddReactionAsync(EmojiList.GreenCheck);
    }
}
