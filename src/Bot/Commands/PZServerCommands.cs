using Discord.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

public class PZServerCommands : ModuleBase<SocketCommandContext>
{
    [Command("server_cmd")]
    [Summary("Allows you to send inputs to the server console. (!server_cmd <text>)")]
    public async Task ServerCommand(params string[] strList)
    {
        try
        {
            ServerUtility.ServerProcess.StandardInput.WriteLine(string.Format("{0}", string.Join(" ", strList)));
            ServerUtility.ServerProcess.StandardInput.Flush();
        }
        catch(Exception ex)
        {
            await Context.Message.AddReactionAsync(EmojiList.RedCross);
            await Context.Channel.SendMessageAsync(ex.Message);
            return;
        }
        
        await Context.Message.AddReactionAsync(EmojiList.GreenCheck);
    }

    [Command("server_msg")]
    [Summary("Broadcasts a message to all players in the server. (!server_msg \"<message>\")")]
    public async Task ServerMessage(string message)
    {
        ServerUtility.Commands.ServerMsg(message);
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
            await Context.Channel.SendMessageAsync(Localization.Get("disc_cmd_start_server_warn_running"));
        }
        else if(ServerBackupCreator.IsRunning)
        {
            await Context.Message.AddReactionAsync(EmojiList.RedCross);
            await Context.Channel.SendMessageAsync(Localization.Get("disc_cmd_start_server_warn_backup"));
        }
        else
        {
            ServerUtility.ServerProcess = ServerUtility.Commands.StartServer();
            
            Logger.WriteLog(string.Format("[PZServerCommand - start_server] Caller: {0}", Context.User.ToString()));
            await Context.Message.AddReactionAsync(EmojiList.GreenCheck);
            await Context.Channel.SendMessageAsync(Localization.Get("disc_cmd_start_server_ok"));
        }
    }

    [Command("stop_server")]
    [Summary("Saves and stops the server. (!stop_server)")]
    public async Task StopServer()
    {
        if(!ServerUtility.IsServerRunning())
        {
            await Context.Message.AddReactionAsync(EmojiList.RedCross);
            await Context.Channel.SendMessageAsync(Localization.Get("disc_cmd_stop_server_warn"));
            return;
        }

        ServerUtility.Commands.StopServer();
        Logger.WriteLog(string.Format("[PZServerCommand - stop_server] Caller: {0}", Context.User.ToString()));

        await Context.Message.AddReactionAsync(EmojiList.GreenCheck);
    }

    [Command("restart_server")]
    [Summary("Restarts the server. (!restart_server)")]
    public async Task RestartServer()
    {
        if(ServerUtility.IsServerRunning())
        {
            Logger.WriteLog(string.Format("[PZServerCommand - restart_server] Caller: {0}", Context.User.ToString()));
            
            await Context.Message.AddReactionAsync(EmojiList.GreenCheck);
            await Context.Channel.SendMessageAsync(Localization.Get("disc_cmd_restart_server_ok"));

            await StopServer();

            while(ServerUtility.IsServerRunning())
                await Task.Delay(250);

            await StartServer();
        }
        else
        {
            await Context.Message.AddReactionAsync(EmojiList.RedCross);
            await Context.Channel.SendMessageAsync(Localization.Get("warn_server_not_running"));
            return;
        }
    }

    [Command("initiate_restart")]
    [Summary("Initiates a restart. (!initiate_restart <minutes until restart>)")]
    public async Task InitiateRestart(uint minutes)
    {
        if(minutes == 0)
        {
            await Context.Message.AddReactionAsync(EmojiList.RedCross);
            await Context.Channel.SendMessageAsync(Localization.Get("disc_cmd_initiate_restart_warn_min"));
            return;
        }

        if(ServerUtility.IsServerRunning())
        {
            Logger.WriteLog(string.Format("[PZServerCommand - initiate_restart] Caller: {0}, Params: {1}", Context.User.ToString(), minutes));
            
            uint restartInMinutes = ServerUtility.InitiateServerRestart(minutes * (60 * 1000));
            ServerUtility.Commands.ServerMsg(Localization.Get("disc_cmd_initiate_restart_info_server_msg").KeyFormat(("minutes", restartInMinutes)));

            await Context.Message.AddReactionAsync(EmojiList.GreenCheck);
            await Context.Channel.SendMessageAsync(Localization.Get("disc_cmd_initiate_restart_info_disc_msg").KeyFormat(("minutes", restartInMinutes)));
        }
        else
        {
            await Context.Message.AddReactionAsync(EmojiList.RedCross);
            await Context.Channel.SendMessageAsync(Localization.Get("warn_server_not_running"));
            return;
        }
    }

    [Command("abort_restart")]
    [Summary("Aborts an upcoming restart. Works both with restart schedule and manual initiated restart. (!abort_restart)")]
    public async Task AbortRestart()
    {
        if(ServerUtility.IsServerRunning())
        {
            Logger.WriteLog(string.Format("[PZServerCommand - abort_restart] Caller: {0}", Context.User.ToString()));
            
            if(Application.BotSettings.ServerScheduleSettings.ServerRestartScheduleType.ToLower() == "time")
                ServerUtility.AbortNextTimedServerRestart = true;
            else
                ServerUtility.ResetServerRestartInterval();
            
            // ServerUtility.Commands.ServerMsg(Localization.Get("disc_cmd_abort_restart_ok_server").KeyFormat(("minutes", Scheduler.GetItem("ServerRestart").NextExecuteTime.Subtract(DateTime.Now).TotalMinutes)));
            ServerUtility.Commands.ServerMsg(Localization.Get("disc_cmd_abort_restart_ok_disc"));

            await Context.Message.AddReactionAsync(EmojiList.GreenCheck);
            await Context.Channel.SendMessageAsync(Localization.Get("disc_cmd_abort_restart_ok_disc"));
        }
        else
        {
            await Context.Message.AddReactionAsync(EmojiList.RedCross);
            await Context.Channel.SendMessageAsync(Localization.Get("warn_server_not_running"));
            return;
        }
    }

    [Command("save_server")]
    [Summary("Saves the current world. (!save_server)")]
    public async Task SaveServer()
    {
        ServerUtility.Commands.SaveServer();
        Logger.WriteLog(string.Format("[PZServerCommand - save_server] Caller: {0}", Context.User.ToString()));

        await Context.Message.AddReactionAsync(EmojiList.GreenCheck);
    }

    [Command("perk_info")]
    [Summary("Displays the perk information of player. (!perk_info \"<username>\")")]
    public async Task PerkInfo(string username)
    {
        Logger.WriteLog(string.Format("[BotCommands - perk_info] Caller: {0}, Params: {1}", Context.User.ToString(), username));
        
        var userPerkDataList = ServerLogParsers.PerkLog.Get();

        if(userPerkDataList == null
        || !userPerkDataList.ContainsKey(username))
        {
            await Context.Message.AddReactionAsync(EmojiList.RedCross);
            await Context.Channel.SendMessageAsync(Localization.Get("disc_cmd_perk_info_no_result").KeyFormat(("username", username)));
        }
        else
        {
            await Context.Message.AddReactionAsync(EmojiList.GreenCheck);

            List<KeyValuePair<string, string>> perkList = new List<KeyValuePair<string, string>>();

            var userPerkData = userPerkDataList[username];

            foreach(KeyValuePair<string, int> perk in userPerkData.Perks)
                perkList.Add(new KeyValuePair<string, string>(perk.Key, perk.Value.ToString()));

            await Context.Channel.SendMessageAsync(Localization.Get("disc_cmd_perk_info_result_title").KeyFormat(("username", username)));
            await DiscordUtility.SendEmbeddedMessage(Context.Message.Channel, perkList);
        }

        var lastCacheTime = ServerLogParsers.PerkLog.LastCacheTime;

        if(lastCacheTime != null)
        {
            await Context.Channel.SendMessageAsync(
                Localization.Get("gen_last_cache_text").KeyFormat(("relative_time", BotUtility.GetPastRelativeTimeStr(DateTime.Now, (DateTime)lastCacheTime)))
            );
        }
    }

    [Command("add_user")]
    [Summary("Adds a new user to the whitelist. (!add_user \"<username>\" \"<password>\")")]
    public async Task AddUser(string username, string password)
    {
        ServerUtility.Commands.AddUser(username, password);
        Logger.WriteLog(string.Format("[PZServerCommand - add_user] Caller: {0}, Params: {1}", Context.User.ToString(), username+","+password));

        await Context.Message.AddReactionAsync(EmojiList.GreenCheck);
    }

    [Command("add_user_to_whitelist")]
    [Summary("Adds a single user connected with a password to the whitelist. (!add_user_to_whitelist \"<username>\")")]
    public async Task AddUserToWhiteList(string username)
    {
        ServerUtility.Commands.AddUserToWhiteList(username);
        Logger.WriteLog(string.Format("[PZServerCommand - addusertowhitelist] Caller: {0}, Params: {1}", Context.User.ToString(), username));

        await Context.Message.AddReactionAsync(EmojiList.GreenCheck);
    }

    [Command("remove_user_from_white_list")]
    [Summary("Removes a single user connected with a password to the whitelist. (!remove_user_from_whitelist \"<username>\")")]
    public async Task RemoveUserFromWhiteList(string username)
    {
        ServerUtility.Commands.RemoveUserFromWhiteList(username);
        Logger.WriteLog(string.Format("[PZServerCommand - remove_user_from_whitelist] Caller: {0}, Params: {1}", Context.User.ToString(), username));

        await Context.Message.AddReactionAsync(EmojiList.GreenCheck);
    }

    [Command("ban_steamid")]
    [Summary("Bans a Steam ID. (!ban_steamid <steam id>)")]
    public async Task BanId(ulong id)
    {
        ServerUtility.Commands.BanId(id);
        Logger.WriteLog(string.Format("[PZServerCommand - ban_steamid] Caller: {0}, Params: {1}", Context.User.ToString(), id.ToString()));

        await Context.Message.AddReactionAsync(EmojiList.GreenCheck);
    }

    [Command("unban_steamid")]
    [Summary("Unbans a Steam ID. (!unban_steamid <steam id>)")]
    public async Task UnbanId(ulong id)
    {
        ServerUtility.Commands.UnbanId(id);
        Logger.WriteLog(string.Format("[PZServerCommand - unban_steamid] Caller: {0}, Params: {1}", Context.User.ToString(), id.ToString()));

        await Context.Message.AddReactionAsync(EmojiList.GreenCheck);
    }

    [Command("ban_user")]
    [Summary("Bans a user. (!ban_user \"<username>\")")]
    public async Task BanUser(string username)
    {
        ServerUtility.Commands.BanUser(username);
        Logger.WriteLog(string.Format("[PZServerCommand - ban_user] Caller: {0}, Params: {1}", Context.User.ToString(), username));

        await Context.Message.AddReactionAsync(EmojiList.GreenCheck);
    }

    [Command("unban_user")]
    [Summary("Unbans a user. (!unbanuser \"<username>\")")]
    public async Task UnbanUser(string username)
    {
        ServerUtility.Commands.UnbanUser(username);
        Logger.WriteLog(string.Format("[PZServerCommand - unban_user] Caller: {0}, Params: {1}", Context.User.ToString(), username));

        await Context.Message.AddReactionAsync(EmojiList.GreenCheck);
    }

    [Command("make_admin")]
    [Summary("Gives admin rights to a user. (!make_admin \"<username>\")")]
    public async Task GrantAdmin(string username)
    {
        ServerUtility.Commands.GrantAdmin(username);
        Logger.WriteLog(string.Format("[PZServerCommand - make_admin] Caller: {0}, Params: {1}", Context.User.ToString(), username));

        await Context.Message.AddReactionAsync(EmojiList.GreenCheck);
    }

    [Command("remove_admin")]
    [Summary("Removes admin rights to a user. (!remove_admin \"<username>\")")]
    public async Task RemoveAdmin(string username)
    {
        ServerUtility.Commands.RemoveAdmin(username);
        Logger.WriteLog(string.Format("[PZServerCommand - remove_admin] Caller: {0}, Params: {1}", Context.User.ToString(), username));

        await Context.Message.AddReactionAsync(EmojiList.GreenCheck);
    }

    [Command("kick_user")]
    [Summary("Kicks a user from the server. (!kick_user \"<username>\")")]
    public async Task KickUser(string username)
    {
        ServerUtility.Commands.KickUser(username);
        Logger.WriteLog(string.Format("[PZServerCommand - kick_user] Caller: {0}, Params: {1}", Context.User.ToString(), username));

        await Context.Message.AddReactionAsync(EmojiList.GreenCheck);
    }

    [Command("start_rain")]
    [Summary("Starts rain on the server. (!startrain)")]
    public async Task StartRain()
    {
        ServerUtility.Commands.StartRain();
        Logger.WriteLog(string.Format("[PZServerCommand - start_rain] Caller: {0}", Context.User.ToString()));

        await Context.Message.AddReactionAsync(EmojiList.GreenCheck);
    }

    [Command("stop_rain")]
    [Summary("Stops rain on the server. (!stoprain)")]
    public async Task StopRain()
    {
        ServerUtility.Commands.StopRain();
        Logger.WriteLog(string.Format("[PZServerCommand - stop_rain] Caller: {0}", Context.User.ToString()));

        await Context.Message.AddReactionAsync(EmojiList.GreenCheck);
    }

    [Command("teleport")]
    [Summary("Teleports a player. (!teleport \"<username1>\" \"<username2>\") | Username 1 will be teleported to Username 2.")]
    public async Task Teleport(string username1, string username2)
    {
        ServerUtility.Commands.Teleport(username1, username2);
        Logger.WriteLog(string.Format("[PZServerCommand - teleport] Caller: {0}", Context.User.ToString()));

        await Context.Message.AddReactionAsync(EmojiList.GreenCheck);
    }

    [Command("add_item")]
    [Summary("Gives an item to the player. (!add_item \"<username>\" \"<module.item>\")")]
    public async Task AddItem(string username, string item)
    {
        ServerUtility.Commands.AddItem(username, item);
        Logger.WriteLog(string.Format("[PZServerCommand - add_item] User: {0}, Params: {1}", Context.User.ToString(), username+","+item));

        await Context.Message.AddReactionAsync(EmojiList.GreenCheck);
    }

    [Command("add_xp")]
    [Summary("Gives XP to a player. (!addxp \"<username>\" \"<perk>\" <xp>)")]
    public async Task AddXP(string username, string perk, uint xp)
    {
        ServerUtility.Commands.AddXP(username, perk, xp);
        Logger.WriteLog(string.Format("[PZServerCommand - add_xp] Caller: {0}, Params: {1}", Context.User.ToString(), username+","+perk+","+xp));

        await Context.Message.AddReactionAsync(EmojiList.GreenCheck);
    }

    [Command("chopper")]
    [Summary("Places a helicopter event on a random player. (!chopper)")]
    public async Task Chopper()
    {
        ServerUtility.Commands.Chopper();
        Logger.WriteLog(string.Format("[PZServerCommand - chopper] Caller: {0}", Context.User.ToString()));

        await Context.Message.AddReactionAsync(EmojiList.GreenCheck);
    }

    [Command("godmode")]
    [Summary("Makes a player invincible. (!godmode \"<username>\")")]
    public async Task GodMode(string username)
    {
        ServerUtility.Commands.GodMode(username);
        Logger.WriteLog(string.Format("[PZServerCommand - godmode] Caller: {0}, Params: {1}", Context.User.ToString(), username));

        await Context.Message.AddReactionAsync(EmojiList.GreenCheck);
    }

    [Command("invisible")]
    [Summary("Makes a player invisible to zombies. (!invisible \"<username>\")")]
    public async Task Invisible(string username)
    {
        ServerUtility.Commands.Invisible(username);
        Logger.WriteLog(string.Format("[PZServerCommand - invisible] Caller: {0}, Params: {1}", Context.User.ToString(), username));

        await Context.Message.AddReactionAsync(EmojiList.GreenCheck);
    }

    [Command("noclip")]
    [Summary("Allows a player to pass through solid objects. (!noclip \"<username>\")")]
    public async Task NoClip(string username)
    {
        ServerUtility.Commands.NoClip(username);
        Logger.WriteLog(string.Format("[PZServerCommand - noclip] Caller: {0}, Params: {1}", Context.User.ToString(), username));

        await Context.Message.AddReactionAsync(EmojiList.GreenCheck);
    }

    [Command("show_options")]
    [Summary("Shows a list of current server options and values. (Prints to the server console) (!show_options)")]
    public async Task ShowOptions()
    {
        ServerUtility.Commands.ShowOptions();
        Logger.WriteLog(string.Format("[PZServerCommand - show_options] Caller: {0}", Context.User.ToString()));

        await Context.Message.AddReactionAsync(EmojiList.GreenCheck);
    }

    [Command("reload_options")]
    [Summary("Reloads server options. (!reload_options)")]
    public async Task ReloadOptions()
    {
        ServerUtility.Commands.ReloadOptions();
        Logger.WriteLog(string.Format("[PZServerCommand - reload_options] Caller: {0}", Context.User.ToString()));

        await Context.Message.AddReactionAsync(EmojiList.GreenCheck);
    }

    [Command("change_option")]
    [Summary("Changes a server option. (!change_option \"<option>\" \"<newOption>\")")]
    public async Task ChangeOption(string option, string newOption)
    {
        ServerUtility.Commands.ChangeOption(option, newOption);
        Logger.WriteLog(string.Format("[PZServerCommand - change_option] Caller: {0}, Params: {1}", Context.User.ToString(), option + "," + newOption));

        await Context.Message.AddReactionAsync(EmojiList.GreenCheck);
    }

    [Command("workshop_mod")]
    [Summary("Adds or removes a workshop mod from a workshop url. (!workshop_mod <add|remove> <links to workshop mods>)")]
    [Remarks("skip")]
    public async Task WorkshopMod(string type, params string[] workshopModUrls)
    {
        if (type.ToLower() != "add" && type.ToLower() != "remove")
        {
            await Context.Message.AddReactionAsync(EmojiList.RedCross);
            await Context.Channel.SendMessageAsync(Localization.Get("disc_cmd_workshop_mod_change_type"));
            return;
        }

        string configFilePath = ServerUtility.GetServerConfigIniFilePath();
        if (string.IsNullOrEmpty(configFilePath))
        {
            await Context.Message.AddReactionAsync(EmojiList.RedCross);
            await Context.Channel.SendMessageAsync(Localization.Get("disc_cmd_workshop_mod_config_err"));
            return;
        }

        IniParser.IniData iniData = IniParser.Parse(configFilePath);
        if (iniData == null)
        {
            await Context.Message.AddReactionAsync(EmojiList.RedCross);
            await Context.Channel.SendMessageAsync(Localization.Get("disc_cmd_workshop_mod_ini_err"));
            return;
        }

        // Store the current mod Ids in lists
        List<string> workshopIdList = new List<string>(iniData.GetValue("WorkshopItems").Split(';'));
        List<string> modIdList = new List<string>(iniData.GetValue("Mods").Split(';'));

        List<string> workshopModIdList = new List<string>();

        string pattern = @"id=\d*";
        Regex rg = new Regex(pattern);

        foreach (string workshopModUrl in workshopModUrls)
        {
            MatchCollection matchedId = rg.Matches(workshopModUrl);
            workshopModIdList.Add(matchedId[0].Value.Replace("id=",""));
        }

        string[] workshopModIds = workshopModIdList.ToArray();

        var fetchDetails = Task.Run(async () => await SteamWebAPI.GetWorkshopItemDetails(workshopModIds));
        var itemDetails = fetchDetails.Result;

        pattern = @"Mod ID: .*(?!Mod ID: )";
        rg = new Regex(pattern);

        foreach (var item in itemDetails)
        {
            if (type.ToLower()=="add") { workshopIdList.Add(item.PublishedFileId); }
            else if (type.ToLower() == "remove") { workshopIdList.Remove(item.PublishedFileId); }
            MatchCollection matchedId = rg.Matches(item.Description);
            foreach (Match match in matchedId)
            {
                string matchStr = match.Value.Replace("Mod ID: ", "").TrimEnd(new char[] { '\r', '\n' });
                if (type.ToLower() == "add") { modIdList.Add(matchStr); }
                else if (type.ToLower() == "remove") { modIdList.Remove(matchStr); }
            }
        }

        ServerUtility.Commands.ChangeOption("Mods", string.Join(";", modIdList.Distinct()).TrimStart(';'));
        ServerUtility.Commands.ChangeOption("WorkshopItems", string.Join(";", workshopIdList.Distinct()).TrimStart(';'));

        Logger.WriteLog(string.Format("[PZServerCommand - add_workshop_mod] Caller: {0}, Params: {1}", Context.User.ToString(), string.Join(", ", workshopModUrls)));

        if (type.ToLower()=="add")
        {
            await Context.Channel.SendMessageAsync(Localization.Get("disc_cmd_workshop_mod_add_ok"));
        }
        else if (type.ToLower() == "remove")
        {
            await Context.Channel.SendMessageAsync(Localization.Get("disc_cmd_workshop_mod_remove_ok"));
        }


        await Context.Message.AddReactionAsync(EmojiList.GreenCheck);
    }

    [Command("add_workshop_mod")]
    [Summary("Adds a workshop mod from the workshop mod url. (!add_workshop_mod <workshop mod urls with spaces in-between>)")]
    public async Task AddWorkshopMod(params string[] workshopModUrls)
    {
        await WorkshopMod("add", workshopModUrls);
    }

    [Command("remove_workshop_mod")]
    [Summary("Removes a workshop mod from the workshop mod url. (!remove_workshop_mod <workshop mod urls with spaces in-between>)")]
    public async Task RemoveWorkshopMod(params string[] workshopModUrls)
    {
        await WorkshopMod("remove", workshopModUrls);
    }
}
