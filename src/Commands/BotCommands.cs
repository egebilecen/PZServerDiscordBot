using Discord.Commands;
using Discord.WebSocket;
using System.Collections.Generic;
using System.Threading.Tasks;

public class BotCommands : ModuleBase<SocketCommandContext>
{
    [Command("set_command_channel")]
    [Summary("Sets the channel for bot to work in. (!set_command_channel <channel tag>)")]
    public async Task SetCommandChannel(ISocketMessageChannel channel)
    {
        Application.botSettings.CommandChannelId = channel.Id;
        Application.botSettings.Save();

        Logger.WriteLog("["+Context.Message.Timestamp.UtcDateTime.ToString()+"]"+string.Format("[BotCommands - set_command_channel] Caller: {0}, Params: <#{1}>", Context.User.ToString(), channel.Id));
        await Context.Message.AddReactionAsync(EmojiList.GreenCheck);
        await Context.Channel.SendMessageAsync(string.Format("Channel <#{0}> successfully configured for the bot to work in.", channel.Id.ToString()));
    }

    [Command("set_log_channel")]
    [Summary("Sets the channel for bot to work in. (!set_log_channel <channel tag>)")]
    public async Task SetLogChannel(ISocketMessageChannel channel)
    {
        Application.botSettings.LogChannelId = channel.Id;
        Application.botSettings.Save();

        Logger.WriteLog("["+Context.Message.Timestamp.UtcDateTime.ToString()+"]"+string.Format("[BotCommands - set_log_channel] Caller: {0}, Params: <#{1}>", Context.User.ToString(), channel.Id));
        await Context.Message.AddReactionAsync(EmojiList.GreenCheck);
        await Context.Channel.SendMessageAsync(string.Format("Channel <#{0}> successfully configured for the bot to work in.", channel.Id.ToString()));
    }

    [Command("set_public_channel")]
    [Summary("Sets the channel for bot to work in. (!set_public_channel <channel tag>)")]
    public async Task SetPublicChannel(ISocketMessageChannel channel)
    {
        Application.botSettings.PublicChannelId = channel.Id;
        Application.botSettings.Save();

        Logger.WriteLog("["+Context.Message.Timestamp.UtcDateTime.ToString()+"]"+string.Format("[BotCommands - set_public_channel] Caller: {0}, Params: <#{1}>", Context.User.ToString(), channel.Id));
        await Context.Message.AddReactionAsync(EmojiList.GreenCheck);
        await Context.Channel.SendMessageAsync(string.Format("Channel <#{0}> successfully configured for the bot to work in.", channel.Id.ToString()));
    }

    [Command("get_settings")]
    [Summary("Gets the bot settings. (!get_settings)")]
    public async Task GetSettings()
    {
        Logger.WriteLog("["+Context.Message.Timestamp.UtcDateTime.ToString()+"]"+string.Format("[BotCommands - get_settings] Caller: {0}", Context.User.ToString()));
        await Context.Message.AddReactionAsync(EmojiList.GreenCheck);
        
        string botSettings = "";
        botSettings += "**Server ID:** `"+Application.botSettings.GuildId.ToString()+"`";
        botSettings += "\n";
        botSettings += "**Command Channel ID:** `"+Application.botSettings.CommandChannelId.ToString()+"` (<#"+Application.botSettings.CommandChannelId.ToString()+">)";
        botSettings += "\n";
        botSettings += "**Log Channel ID:** `"+Application.botSettings.LogChannelId.ToString()+"` (<#"+Application.botSettings.LogChannelId.ToString()+">)";
        botSettings += "\n";
        botSettings += "**Public Channel ID:** `"+Application.botSettings.PublicChannelId.ToString()+"` (<#"+Application.botSettings.PublicChannelId.ToString()+">)";
        botSettings += "\n";
        botSettings += "**Perk Parser Cache Duration:** "+Application.botSettings.ServerLogParserSettings.PerkParserCacheDuration.ToString()+" minute(s)";
        botSettings += "\n";
        botSettings += "**Restart Schedule Interval:** "+(Application.botSettings.ServerScheduleSettings.ServerRestartSchedule / (60 * 1000)).ToString()+" minute(s)";
        botSettings += "\n";
        botSettings += "**Workshop Mod Update Checker Interval:** "+(Application.botSettings.ServerScheduleSettings.WorkshopItemUpdateSchedule / (60 * 1000)).ToString()+" minute(s)";

        await Context.Channel.SendMessageAsync(botSettings);
    }

    [Command("set_restart_interval")]
    [Summary("Set the server's restart schedule interval. (in minutes!) (!set_restart_interval <interval in minutes>)")]
    public async Task SetRestartInterval(uint intervalMinute)
    {
        if(intervalMinute < 60)
        {
            await Context.Message.AddReactionAsync(EmojiList.RedCross);
            await Context.Channel.SendMessageAsync("Restart interval must be at least 60 minutes.");
            return;
        }

        Logger.WriteLog("["+Context.Message.Timestamp.UtcDateTime.ToString()+"]"+string.Format("[BotCommands - set_restart_interval] Caller: {0}, Params: {1}", Context.User.ToString(), intervalMinute));
        await Context.Message.AddReactionAsync(EmojiList.GreenCheck);
        
        Scheduler.GetItem("ServerRestart").UpdateInterval(intervalMinute * 60 * 1000);

        Application.botSettings.ServerScheduleSettings.ServerRestartSchedule = intervalMinute * 60 * 1000;
        Application.botSettings.Save();

        await Context.Channel.SendMessageAsync("Server restart schedule is updated.");
    }

    [Command("set_mod_update_check_interval")]
    [Summary("Set the workshop mod update check schedule interval. (in minutes!) (!set_mod_update_check_interval <interval in minutes>)")]
    public async Task SetWorkshopItemUpdateChecker(uint intervalMinute)
    {
        if(intervalMinute < 1)
        {
            await Context.Message.AddReactionAsync(EmojiList.RedCross);
            await Context.Channel.SendMessageAsync("Interval must be at least 1 minute(s).");
            return;
        }

        Logger.WriteLog("["+Context.Message.Timestamp.UtcDateTime.ToString()+"]"+string.Format("[BotCommands - set_mod_update_check_interval] Caller: {0}, Params: {1}", Context.User.ToString(), intervalMinute));
        await Context.Message.AddReactionAsync(EmojiList.GreenCheck);
        
        Scheduler.GetItem("WorkshopItemUpdateChecker").UpdateInterval(intervalMinute * 60 * 1000);

        Application.botSettings.ServerScheduleSettings.WorkshopItemUpdateSchedule = intervalMinute * 60 * 1000;
        Application.botSettings.Save();

        await Context.Channel.SendMessageAsync("Workshop mod update check schedule is updated.");
    }

    [Command("set_perk_cache_duration")]
    [Summary("Set the perk cache duration. (in minutes!) (!set_perk_cache_duration <duration in minutes>)")]
    public async Task SetPerkCacheDuration(uint durationMinute)
    {
        //if(durationMinute < 1)
        //{
        //    await Context.Message.AddReactionAsync(EmojiList.RedCross);
        //    await Context.Channel.SendMessageAsync("Duration must be at least 1 minute(s).");
        //    return;
        //}

        if(durationMinute < 0)
        {
            await Context.Message.AddReactionAsync(EmojiList.RedCross);
            await Context.Channel.SendMessageAsync("Duration cannot be smaller than 0. But it can be 0 which means there won't be any caching.");
            return;
        }

        Logger.WriteLog("["+Context.Message.Timestamp.UtcDateTime.ToString()+"]"+string.Format("[BotCommands - set_perk_cache_duration] Caller: {0}, Params: {1}", Context.User.ToString(), durationMinute));
        await Context.Message.AddReactionAsync(EmojiList.GreenCheck);
        
        Application.botSettings.ServerLogParserSettings.PerkParserCacheDuration = durationMinute;
        Application.botSettings.Save();

        await Context.Channel.SendMessageAsync("Perk cache duration is updated.");
    }

    [Command("reset_perk_cache")]
    [Summary("Reset the perk cache. (!reset_perk_cache)")]
    public async Task ResetPerkCache()
    {
        Logger.WriteLog("["+Context.Message.Timestamp.UtcDateTime.ToString()+"]"+string.Format("[BotCommands - reset_perk_cache] Caller: {0}", Context.User.ToString()));
        await Context.Message.AddReactionAsync(EmojiList.GreenCheck);
        
        ServerLogParsers.PerkLog.perkCache = null;
        await Context.Channel.SendMessageAsync("Perk cache has been reset.");
    }
}
