using Discord;
using Discord.Commands;
using System;
using System.Diagnostics;
using System.Threading.Tasks;

public class PZServerCommands : ModuleBase<SocketCommandContext>
{
    [Command("server_msg")]
    [Summary("Broadcasts a message to all players in the server. (!server_msg \"<message>\")")]
    public async Task ServerMessage(string message)
    {
        ServerUtility.serverProcess.StandardInput.WriteLine(string.Format("servermsg \"{0}\"", message));
        ServerUtility.serverProcess.StandardInput.Flush();
        Logger.WriteLog(string.Format("[PZServerCommand - server_msg] Caller: {0}, Params: {1}", Context.User.ToString(), message));

        await Context.Message.AddReactionAsync(EmojiList.GreenCheck);
    }

    [Command("start_server")]
    [Summary("Starts the server. (!start_server)")]
    public async Task StartServer()
    {
        if(ServerUtility.IsServerRunning())
        {
            await Context.Message.AddReactionAsync(EmojiList.RedCross);
            await Context.Channel.SendMessageAsync("Server is already running.");
        }
        else
        {
            ServerUtility.serverProcess = ServerUtility.StartServer();

            await Context.Message.AddReactionAsync(EmojiList.GreenCheck);
            await Context.Channel.SendMessageAsync("Server should be on it's way to get started. This process may take a while. Please check the server status in 1 or 2 minute.");
        }
    }

    [Command("stop_server")]
    [Summary("Saves and stops the server. (!stop_server)")]
    public async Task StopServer()
    {
        if(!ServerUtility.IsServerRunning())
        {
            await Context.Message.AddReactionAsync(EmojiList.RedCross);
            await Context.Channel.SendMessageAsync("Server is already stopped.");
        }

        ServerUtility.serverProcess.StandardInput.WriteLine("quit");
        ServerUtility.serverProcess.StandardInput.Flush();
        Logger.WriteLog(string.Format("[PZServerCommand - stopserver] Caller: {0}", Context.User.ToString()));

        await Context.Message.AddReactionAsync(EmojiList.GreenCheck);
    }

    [Command("restart_server")]
    [Summary("Restarts the server. (!restart_server)")]
    public async Task RestartServer()
    {
        if(ServerUtility.IsServerRunning())
        {
            await Context.Message.AddReactionAsync(EmojiList.GreenCheck);
            await ReplyAsync("Restarting server.");

            while(ServerUtility.IsServerRunning())
                await Task.Delay(25);

            await StartServer();
        }
    }

    [Command("save")]
    [Summary("Saves the current world. (!save)")]
    public async Task Save()
    {
        ServerUtility.serverProcess.StandardInput.WriteLine("save");
        ServerUtility.serverProcess.StandardInput.Flush();
        Logger.WriteLog(string.Format("[PZServerCommand - save] Caller: {0}", Context.User.ToString()));

        await Context.Message.AddReactionAsync(EmojiList.GreenCheck);
    }

    [Command("add_user")]
    [Summary("Adds a new user to the whitelist. (!add_user \"<username>\" \"<password>\")")]
    public async Task AddUser(string username, string password)
    {
        ServerUtility.serverProcess.StandardInput.WriteLine(string.Format("adduser \"{0}\" \"{1}\"", username, password));
        ServerUtility.serverProcess.StandardInput.Flush();
        Logger.WriteLog(string.Format("[PZServerCommand - add_user] Caller: {0}, Params: {1}", Context.User.ToString(), username+","+password));

        await Context.Message.AddReactionAsync(EmojiList.GreenCheck);
    }

    /* Not working.
    [Command("addusertowhitelist")]
    [Summary("Adds a single user connected with a password to the whitelist. (!addusertowhitelist \"<username>\")")]
    public async Task AddUserToWhiteList(string username)
    {
        ServerUtility.serverProcess.StandardInput.WriteLine(string.Format("addusertowhitelist \"{0}\"", username));
        Logger.WriteLog(string.Format("[PZServerCommand - addusertowhitelist] Caller: {0}, Params: {1}", Context.User.ToString(), username));

        await Context.Message.AddReactionAsync(EmojiList.GreenCheck);
    }
    */

    [Command("remove_user_from_white_list")]
    [Summary("Removes a single user connected with a password to the whitelist. (!remove_user_from_whitelist \"<username>\")")]
    public async Task RemoveUserFromWhiteList(string username)
    {
        ServerUtility.serverProcess.StandardInput.WriteLine(string.Format("removeuserfromwhitelist \"{0}\"", username));
        ServerUtility.serverProcess.StandardInput.Flush();
        Logger.WriteLog(string.Format("[PZServerCommand - remove_user_from_whitelist] Caller: {0}, Params: {1}", Context.User.ToString(), username));

        await Context.Message.AddReactionAsync(EmojiList.GreenCheck);
    }

    [Command("ban_steamid")]
    [Summary("Bans a Steam ID. (!ban_steamid <steam id>)")]
    public async Task BanId(ulong id)
    {
        ServerUtility.serverProcess.StandardInput.WriteLine(string.Format("banid \"{0}\"", id.ToString()));
        ServerUtility.serverProcess.StandardInput.Flush();
        Logger.WriteLog(string.Format("[PZServerCommand - ban_steamid] Caller: {0}, Params: {1}", Context.User.ToString(), id.ToString()));

        await Context.Message.AddReactionAsync(EmojiList.GreenCheck);
    }

    [Command("unban_steamid")]
    [Summary("Unbans a Steam ID. (!unban_steamid <steam id>)")]
    public async Task UnbanId(ulong id)
    {
        ServerUtility.serverProcess.StandardInput.WriteLine(string.Format("unbanid \"{0}\"", id.ToString()));
        ServerUtility.serverProcess.StandardInput.Flush();
        Logger.WriteLog(string.Format("[PZServerCommand - unban_steamid] Caller: {0}, Params: {1}", Context.User.ToString(), id.ToString()));

        await Context.Message.AddReactionAsync(EmojiList.GreenCheck);
    }

    [Command("ban_user")]
    [Summary("Bans a user. (!ban_user \"<username>\")")]
    public async Task BanUser(string username)
    {
        ServerUtility.serverProcess.StandardInput.WriteLine(string.Format("banuser \"{0}\"", username));
        ServerUtility.serverProcess.StandardInput.Flush();
        Logger.WriteLog(string.Format("[PZServerCommand - ban_user] Caller: {0}, Params: {1}", Context.User.ToString(), username));

        await Context.Message.AddReactionAsync(EmojiList.GreenCheck);
    }

    [Command("unban_user")]
    [Summary("Unbans a user. (!unbanuser \"<username>\")")]
    public async Task UnbanUser(string username)
    {
        ServerUtility.serverProcess.StandardInput.WriteLine(string.Format("unbanuser \"{0}\"", username));
        ServerUtility.serverProcess.StandardInput.Flush();
        Logger.WriteLog(string.Format("[PZServerCommand - unban_user] Caller: {0}, Params: {1}", Context.User.ToString(), username));

        await Context.Message.AddReactionAsync(EmojiList.GreenCheck);
    }

    [Command("make_admin")]
    [Summary("Gives admin rights to a user. (!make_admin \"<username>\")")]
    public async Task GrantAdmin(string username)
    {
        ServerUtility.serverProcess.StandardInput.WriteLine(string.Format("grantadmin \"{0}\"", username));
        ServerUtility.serverProcess.StandardInput.Flush();
        Logger.WriteLog(string.Format("[PZServerCommand - make_admin] Caller: {0}, Params: {1}", Context.User.ToString(), username));

        await Context.Message.AddReactionAsync(EmojiList.GreenCheck);
    }

    [Command("remove_admin")]
    [Summary("Removes admin rights to a user. (!remove_admin \"<username>\")")]
    public async Task RemoveAdmin(string username)
    {
        ServerUtility.serverProcess.StandardInput.WriteLine(string.Format("removeadmin \"{0}\"", username));
        ServerUtility.serverProcess.StandardInput.Flush();
        Logger.WriteLog(string.Format("[PZServerCommand - remove_admin] Caller: {0}, Params: {1}", Context.User.ToString(), username));

        await Context.Message.AddReactionAsync(EmojiList.GreenCheck);
    }

    [Command("kick_user")]
    [Summary("Kicks a user from the server. (!kick_user \"<username>\")")]
    public async Task KickUser(string username)
    {
        ServerUtility.serverProcess.StandardInput.WriteLine(string.Format("kickuser \"{0}\"", username));
        ServerUtility.serverProcess.StandardInput.Flush();
        Logger.WriteLog(string.Format("[PZServerCommand - kick_user] Caller: {0}, Params: {1}", Context.User.ToString(), username));

        await Context.Message.AddReactionAsync(EmojiList.GreenCheck);
    }

    [Command("start_rain")]
    [Summary("Starts rain on the server. (!startrain)")]
    public async Task StartRain()
    {
        ServerUtility.serverProcess.StandardInput.WriteLine("startrain");
        ServerUtility.serverProcess.StandardInput.Flush();
        Logger.WriteLog(string.Format("[PZServerCommand - start_rain] Caller: {0}", Context.User.ToString()));

        await Context.Message.AddReactionAsync(EmojiList.GreenCheck);
    }

    [Command("stop_rain")]
    [Summary("Stops rain on the server. (!stoprain)")]
    public async Task StopRain()
    {
        ServerUtility.serverProcess.StandardInput.WriteLine("stoprain");
        ServerUtility.serverProcess.StandardInput.Flush();
        Logger.WriteLog(string.Format("[PZServerCommand - stop_rain] Caller: {0}", Context.User.ToString()));

        await Context.Message.AddReactionAsync(EmojiList.GreenCheck);
    }

    [Command("teleport")]
    [Summary("Teleports a player. (!teleport \"<username1>\" \"<username2>\")")]
    public async Task Teleport(string username1, string username2)
    {
        ServerUtility.serverProcess.StandardInput.WriteLine(string.Format("teleport \"{0}\" \"{1}\"", username1, username2));
        ServerUtility.serverProcess.StandardInput.Flush();
        Logger.WriteLog(string.Format("[PZServerCommand - teleport] Caller: {0}", Context.User.ToString()));

        await Context.Message.AddReactionAsync(EmojiList.GreenCheck);
    }

    [Command("add_item")]
    [Summary("Gives an item to the player. (!add_item \"<username>\" \"<module.item>)\"")]
    public async Task AddItem(string username, string item)
    {
        ServerUtility.serverProcess.StandardInput.WriteLine(string.Format("additem \"{0}\" \"{1}\"", username, item));
        ServerUtility.serverProcess.StandardInput.Flush();
        Logger.WriteLog(string.Format("[PZServerCommand - add_item] User: {0}, Params: {1}", Context.User.ToString(), username+","+item));

        await Context.Message.AddReactionAsync(EmojiList.GreenCheck);
    }

    [Command("add_xp")]
    [Summary("Gives XP to a player. (!addxp \"<username>\" \"<perk>\" <xp>)")]
    public async Task AddXP(string username, string perk, uint xp)
    {
        ServerUtility.serverProcess.StandardInput.WriteLine(string.Format("addxp \"{0}\" \"{1}={2}\"", username, perk, xp));
        ServerUtility.serverProcess.StandardInput.Flush();
        Logger.WriteLog(string.Format("[PZServerCommand - add_xp] Caller: {0}, Params: {1}", Context.User.ToString(), username+","+perk+","+xp));

        await Context.Message.AddReactionAsync(EmojiList.GreenCheck);
    }

    [Command("chopper")]
    [Summary("Places a helicopter event on a random player. (!chopper)")]
    public async Task Chopper()
    {
        ServerUtility.serverProcess.StandardInput.WriteLine("chopper");
        ServerUtility.serverProcess.StandardInput.Flush();
        Logger.WriteLog(string.Format("[PZServerCommand - chopper] Caller: {0}", Context.User.ToString()));

        await Context.Message.AddReactionAsync(EmojiList.GreenCheck);
    }

    [Command("godmode")]
    [Summary("Makes a player invincible. (!godmode \"<username>\")")]
    public async Task GodMode(string username)
    {
        ServerUtility.serverProcess.StandardInput.WriteLine(string.Format("godmode \"{0}\"", username));
        ServerUtility.serverProcess.StandardInput.Flush();
        Logger.WriteLog(string.Format("[PZServerCommand - godmode] Caller: {0}, Params: {1}", Context.User.ToString(), username));

        await Context.Message.AddReactionAsync(EmojiList.GreenCheck);
    }

    [Command("invisible")]
    [Summary("Makes a player invisible to zombies. (!invisible \"<username>\")")]
    public async Task Invisible(string username)
    {
        ServerUtility.serverProcess.StandardInput.WriteLine(string.Format("invisible \"{0}\"", username));
        ServerUtility.serverProcess.StandardInput.Flush();
        Logger.WriteLog(string.Format("[PZServerCommand - invisible] Caller: {0}, Params: {1}", Context.User.ToString(), username));

        await Context.Message.AddReactionAsync(EmojiList.GreenCheck);
    }

    [Command("noclip")]
    [Summary("Allows a player to pass through solid objects. (!noclip \"<username>\")")]
    public async Task NoClip(string username)
    {
        ServerUtility.serverProcess.StandardInput.WriteLine(string.Format("noclip \"{0}\"", username));
        ServerUtility.serverProcess.StandardInput.Flush();
        Logger.WriteLog(string.Format("[PZServerCommand - noclip] Caller: {0}, Params: {1}", Context.User.ToString(), username));

        await Context.Message.AddReactionAsync(EmojiList.GreenCheck);
    }
}
