# Project Zomboid Discord Bot for Server Management
This bot is written for people to easily manage their Project Zomboid server using Discord. Please check the **Installation** and **Bot Configuration** section. Also this bot doesn't support multiple discord servers and only works on **Windows** operating system. Be sure to have **.NET Framework 4.7.2** installed on the machine.

<img alt="Passively Maintained" src="https://img.shields.io/badge/maintenance-passively--maintained-yellowgreen.svg" title="There are no plans for new features, but the bot is actively maintained." /><br>
*There are no plans for new features, but the bot is actively maintained.*

## Contents
* [Features](#features)
* [Installation](#installation)
  * [Creating the Discord Bot](#creating-the-discord-bot)
  * [Installing Bot Files](#installing-bot-files)
  * [Writing the Discord Bot Token Into File](#writing-the-discord-bot-token-into-file)
    * [Warning](#warning)
* [Bot Configuration](#bot-configuration)
* [Bot Commands](#bot-commands)
  * [Public Channel](#public-channel)
  * [Command Channel](#command-channel)
* [Localization](#localization)

# Features
- Automated server restart schedule with ingame and discord warning. (Warnings are announced when 1 hour, 30 min, 15 min, 5 min and 1 min left until server restart. Restart interval can be configured with bot commands.)
![Automated Server Restart Example](https://github.com/egebilecen/PZServerDiscordBot/blob/main/.github/images/bot%20features/Screenshot_1.png)
- Automated server restart when a mod (workshop item) update has been detected.
![Automated Workshop Item Update Server Restart](https://github.com/egebilecen/PZServerDiscordBot/blob/main/.github/images/bot%20features/Screenshot_4.png)
- Executing server commands through bot commands. (For example; saving server, kicking player, teleporting player, starting/stopping rain, making admin and so on. Full list will be at the bottom and will be listed under available commands.)
![Server Commands Example](https://github.com/egebilecen/PZServerDiscordBot/blob/main/.github/images/bot%20features/Screenshot_3.png)
- Perk Parser with cache system. (Bot automatically parses the last perk log file that holds the player skills when they login to server. This can be used to aid players that died to a bug but they can't remember their skills. As stated before, server logs the player skills only when they log into server. If player levels up a skill after connecting to server, it won't appear in log unless player logs into server afterwards again.) When command for perk parser is invoked, bot will parse the file and save it contents in memory until cache (in minutes) expires. This is for to increase efficiency as bot will not have to parse the same file each time. In a situation where fresh data needed, **!reset_perk_cache** command can be used to reset the cache.                
![Perk Parser Example](https://github.com/egebilecen/PZServerDiscordBot/blob/main/.github/images/bot%20features/Screenshot_2.png)
- Auto server start if server quits. This feature useful if combined with mods that quits the server for whatever reason. For example, if you are using a mod that checks mod updates and when detects it, quits the server. With enabling this feature, you won't need to manually run the server. <br>
![Auto Server Start Example](https://github.com/egebilecen/PZServerDiscordBot/blob/main/.github/images/bot%20features/Screenshot_5.png)
- `!get_ram_cpu` command for checking current RAM and CPU usage of the machine.
![RAM CPU Command Example](https://github.com/egebilecen/PZServerDiscordBot/blob/main/.github/images/bot%20features/Screenshot_6.png)
- `!backup_server` command for backing up the server easily.
![Backup Command Example 1](https://github.com/egebilecen/PZServerDiscordBot/blob/main/.github/images/bot%20features/Screenshot_7.png)
![Backup Command Example 2](https://github.com/egebilecen/PZServerDiscordBot/blob/main/.github/images/bot%20features/Screenshot_8.png)
- Localization system! You can check available localizations using `!localization` command.  See the [Localization](#localization) section about how to translate the bot.
<i>**Note:** It is not possible to translate commands and their descriptions at the moment. It might change in future, though.</i>

# Installation
#### Creating the Discord Bot
1. Go to [Applications](https://discord.com/developers/applications) section of Discord developer portal. (Be sure to login first.)
2. Click to `New Application` button on the top right corner of screen.
![](https://github.com/egebilecen/PZServerDiscordBot/blob/main/.github/images/bot%20creation/Screenshot_1.png)
3. Enter your Bot's name in pop-up then click to create button. You will be redirected to your application's (bot's) page. In that page, you can update your bot's name, description and even load an image as avatar.
![](https://github.com/egebilecen/PZServerDiscordBot/blob/main/.github/images/bot%20creation/Screenshot_2.png)
4. Navigate to `Bot` section from left menu. Then click to `Add Bot` button. Then confirm the pop-up. You will be redirected to your bot page.
![](https://github.com/egebilecen/PZServerDiscordBot/blob/main/.github/images/bot%20creation/Screenshot_3.png)
5. Click to `Reset Token` button. Then confirm the pop-up. This will create a new token for your bot. Copy the displayed token and save it in a file. You won't able to view your bot token unless you reset it again. Also do not share this token with anyone. It's basically password of your bot. If you share it with someone else, they will have full control on your bot.
![](https://github.com/egebilecen/PZServerDiscordBot/blob/main/.github/images/bot%20creation/Screenshot_4.png)
![](https://github.com/egebilecen/PZServerDiscordBot/blob/main/.github/images/bot%20creation/Screenshot_5.png)
6. Navigate to `OAuth2` section from left menu and select the `URL generator` from dropdown. Check `bot` from `Scopes` section and scroll down to `Bot Permissions`.
![](https://github.com/egebilecen/PZServerDiscordBot/blob/main/.github/images/bot%20creation/Screenshot_6.png)
7. Check the options shown below and copy the generated URL.
![](https://github.com/egebilecen/PZServerDiscordBot/blob/main/.github/images/bot%20creation/Screenshot_7.png)
8. Open the copied link on your browser. In the page, select the server (you must be admin on the server otherwise server won't show up but you can always send the link to an admin which they can authorise the bot) that you want bot to work in. Click to `Continue` button and then to `Authorise` button. Complete the captcha if it pops-up. Now Bot has joined to your server but it's not running yet.
![](https://github.com/egebilecen/PZServerDiscordBot/blob/main/.github/images/bot%20creation/Screenshot_8.png) <br>
![](https://github.com/egebilecen/PZServerDiscordBot/blob/main/.github/images/bot%20creation/Screenshot_9.png)

#### Installing Bot Files
To complete this step and next step, you must have remote access to your Windows machine.
1. Navigate to [releases](https://github.com/egebilecen/PZServerDiscordBot/releases) and pick a binary version. I would suggest picking the latest version as it would consist new features and bug fixes.
2. Download the `zip` archive.
3. Extract the contents in the archive to the `Project Zomboid Dedicated Server` folder. Your directory after extraction will look like the image below.  
![](https://github.com/egebilecen/PZServerDiscordBot/blob/main/.github/images/folder_structure.png)
4. Rename the bat file you were using to start the server as `server.bat`. For example, if you were using `StartServer64.bat`, rename it as `server.bat`.

#### Writing the Discord Bot Token Into File
1. Create an empty text file in the directory and name it as `bot_token.txt` and open it.
![](https://github.com/egebilecen/PZServerDiscordBot/blob/main/.github/images/setting%20up%20bot%20token/Screenshot_1.png)
2. Paste the bot token that you saved while creating the discord bot to the first line. (It will look like the picture below.)
![](https://github.com/egebilecen/PZServerDiscordBot/blob/main/.github/images/setting%20up%20bot%20token/Screenshot_2.png)
3. Save the file and close it.

Now all you need to do is running `PZServerDiscordBot.exe`. If you did setup everything correctly, program will automatically run the Discord bot at background and will show the Project Zomboid Server in the console. (Bot may not send the warning messages about the configuration if your discord server's last created channel is not accessable by the bot. You can just type configuration commands regardless.)
![](https://github.com/egebilecen/PZServerDiscordBot/blob/main/.github/images/bot%20demonstration/Screenshot_2.png)

**Note**<br>
If you never run the project zomboid server before, please run it without using bot. Because when you run the project zomboid server for the first time, it will ask you to setup an admin account. You can't send any key presses to console if you run the server through discord bot's exe file. This also means you can't execute servers commands directly using the console but I did setup all commands in discord bot.

#### Warning
If bot doesn't respond to any commands, that could be mean two things: <br>
* Bot doesn't have permission to see the channel. Be sure that bot has full access on the channel which also includes permission to send messages. After confirming bot has full access but still won't respond, see below.
* Bot has missing *intents* configuration. Please head to [discord developer portal](https://discord.com/developers/applications) (which is the place you did create and set up your bot), select your bot, click to the `Bot` tab on left menu and be sure that all bot intentions are enabled under the `Privileged Gateway Intents` section.
![Bot Intents Example](https://github.com/egebilecen/PZServerDiscordBot/blob/main/.github/images/bot%20intents%20config/Screenshot_1.png)

# Bot Configuration
This bot uses 3 different channels to operate. First channel is *Public* channel where users can interact with bot. Second channel is *Command* channel where must set to be only visible to server admins. This channel is used for executing server management and bot configuration commands. Third channel is *Log* channel. There aren't any commands to execute in this channel and it's set for bot to announce stuff. After the bot launched for first time (or not configured), it will ask you to configure the mentioned three channels using **!set_command_channel**, **!set_log_channel** and **!set_public_channel** commands. Those commands are very easy to use. Just reply to any channel with the tag of the channel you want the bot to be configured in. For example: `!set_public_channel #bot-public`

![Bot Configuration Example](https://github.com/egebilecen/PZServerDiscordBot/blob/main/.github/images/bot%20demonstration/Screenshot_1.png)

# Bot Commands
**!help** command can be used in any of configured 3 channels which bot will respond with the available command list for *that channel*.

#### Public Channel
- ``!bot_info`` Displays information about this bot. (!bot_info)
- ``!server_status`` Gets the server status. (!server_status)
- ``!restart_time`` Gets the next automated restart time. (!restart_time)
- ``!game_date`` Gets the current in-game date. (!game_date)

#### Command Channel
Bot Commands:
- `!set_command_channel` Sets the channel for bot to work in. (!set_command_channel [channel tag>)<br>
- `!set_log_channel` Sets the channel for bot to work in. (!set_log_channel [channel tag])<br>
- `!set_public_channel` Sets the channel for bot to work in. (!set_public_channel [channel tag])<br>
- `!get_settings` Gets the bot settings. (!get_settings)<br>
- `!get_schedules` Gets the remaining times until schedules to be executed. (!get_schedules)<br>
- `!get_ram_cpu` Gets the total RAM and CPU usage of the machine. (!get_ram_cpu)<br>
- `!set_restart_schedule_type` Set the server's restart schedule type. (!set_restart_schedule_type ["interval"|"time"])<br>
- `!set_restart_time` Set the server's restart time(s). The time format is "HH:mm" (using 24-hour time). Server restart schedule type must be "time". (!set_restart_time [times separated by space])<br>
- `!set_restart_interval` Set the server's restart schedule interval. Restart schedule type must be "interval". (in minutes!) (!set_restart_interval [interval in minutes])<br>
- `!set_mod_update_check_interval` Set the workshop mod update check schedule interval. (in minutes!) (!set_mod_update_check_interval [interval in minutes])<br>
- `!set_mod_update_restart_timer` Sets the restart timer for server when mod update detected. (in minutes!) (!set_mod_update_restart_timer [timer in minutes])<br>
- `!toggle_non_public_mod_logging` Bot will print out non-public mods to log channel if enabled. (!toggle_non_public_mod_logging)<br>
- `!set_perk_cache_duration` Set the perk cache duration. (in minutes!) (!set_perk_cache_duration [duration in minutes])<br>
- `!reset_perk_cache` Reset the perk cache. (!reset_perk_cache)<br>
- `!toggle_server_auto_start` Enables/Disables the server auto start feature if server is not running. (!toggle_server_auto_start)<br>
- `!backup_server` Creates a backup of the server. Backup files can be found in "server_backup" folder in the directory where bot has been launched. (!backup_server)<br>
- `!localization` Get/change current localization. (!localization [*(optional)* new localization name])<br>
  
Server Commands:
- `!server_cmd` Allows you to send inputs to the server console. (!server_cmd [text])<br>
- `!server_msg` Broadcasts a message to all players in the server. (!server_msg "[message]")<br>
- `!start_server` Starts the server. (!start_server)<br>
- `!stop_server` Saves and stops the server. (!stop_server)<br>
- `!restart_server` Restarts the server. (!restart_server)<br>
- `!initiate_restart` Initiates a restart. (!initiate_restart [minutes until restart])<br>
- `!abort_restart` Aborts an upcoming restart. Works both with restart schedule and manual initiated restart. (!abort_restart)<br>
- `!save_server` Saves the current world. (!save_server)<br>
- `!perk_info` Displays the perk information of player. (!perk_info "[username]")<br>
- `!add_user` Adds a new user to the whitelist. (!add_user "[username]" "[password]")<br>
- `!add_user_to_whitelist` Adds a single user connected with a password to the whitelist. (!add_user_to_whitelist "[username]")<br>
- `!remove_user_from_white_list` Removes a single user connected with a password to the whitelist. (!remove_user_from_whitelist "[username]")<br>
- `!ban_steamid` Bans a Steam ID. (!ban_steamid [steam id])<br>
- `!unban_steamid` Unbans a Steam ID. (!unban_steamid [steam id])<br>
- `!ban_user` Bans a user. (!ban_user "[username]")<br>
- `!unban_user` Unbans a user. (!unbanuser "[username]")<br>
- `!make_admin` Gives admin rights to a user. (!make_admin "[username]")<br>
- `!remove_admin` Removes admin rights to a user. (!remove_admin "[username]")<br>
- `!kick_user` Kicks a user from the server. (!kick_user "[username]")<br>
- `!start_rain` Starts rain on the server. (!startrain)<br>
- `!stop_rain` Stops rain on the server. (!stoprain)<br>
- `!teleport` Teleports a player. (!teleport "[username1]" "[username2]") | Username 1 will be teleported to Username 2.<br>
- `!add_item` Gives an item to the player. (!add_item "[username]" "[module.item]")<br>
- `!add_xp` Gives XP to a player. (!addxp "[username]" "[perk]" [xp])<br>
- `!chopper` Places a helicopter event on a random player. (!chopper)<br>
- `!godmode` Makes a player invincible. (!godmode "[username]")<br>
- `!invisible` Makes a player invisible to zombies. (!invisible "[username]")<br>
- `!noclip` Allows a player to pass through solid objects. (!noclip "[username]")<br>
- `!show_options` Shows a list of current server options and values. (Prints to the server console) (!show_options)<br>
- `!reload_options` Reloads server options. (!reload_options)<br>
- `!change_option` Changes a server option. (!change_option "[option]" "[newOption]")<br>
- `!add_workshop_mod` Adds a workshop mod from the workshop mod url. (!add_workshop_mod [workshop mod urls with spaces in-between]) **Check [here](https://github.com/egebilecen/PZServerDiscordBot/issues/152#issuecomment-2246117720) before using this command if you have/had multiple servers in the same machine.**<br>
- `!remove_workshop_mod` Removes a workshop mod from the workshop mod url. (!remove_workshop_mod [workshop mod urls with spaces in-between])<br>

# Localization
You can find the default localization file in [here](https://github.com/egebilecen/PZServerDiscordBot/blob/main/localization/default.json). All you need to do is downloading it and translating the all words/sentences in the right side of **":"** between **two quote marks**. Example below will illustrate how it should look like after the translation.

**Before translation:**
```
{
  "gen_enab_up": "Enabled",
  "gen_disa_up": "Disabled",
  "gen_hours_text": "hour(s)",
  ...
  "warn_server_not_running": "Server is not running.",
  "warn_bot_conf_not_done": "Bot configuration haven't done yet.",
  ...
}
```

**After translation:**
```
{
  "gen_enab_up": "Aktif",
  "gen_disa_up": "Pasif",
  "gen_hours_text": "saat",
  ...
  "warn_server_not_running": "Server çalışmıyor.",
  "warn_bot_conf_not_done": "Bot ayarları henüz tamamlanmadı.",
  ...
}
```

**Warning:** 
There might be cases where you can see some weird expressions like `{log_file}`, `{current_version}`, `{state}` etc. These are special expressions that will be replaced with related value before displaying the text. Do **not** translate these. However, you can move them freely in a sentence and match them with your own language's sentence structure.

Some examples of how some of the expressions will look like after the related value replacement:
```
{day}/{month}/{year} -> 19/12/2022
{year}-{month}-{day} -> 2022-12-19

Bot Version: {version}  -> Bot Version: v1.8.0
Bot Version ({version}) -> Bot Version (v1.8.0)

Server auto start feature has been {state}.      -> Server auto start feature has been enabled.
Non-public mod logging feature has been {state}. -> Non-public mod logging feature has been disabled.

{hours} hours ago -> 10 hours ago
hours ago {hours} -> hours ago 10
```

After you have completed translating all words/sentences, please create an [issue](https://github.com/egebilecen/PZServerDiscordBot/issues/new/choose) by selecting the **Localization Submission** template with the title of `Localization of <language here>` and attaching the translated `.json` file in a **zip archive**. I will add it to available localizations. Also, when a new version of the bot released, there might be new added text so it is good to keep an eye on updates. If your current localization is missing the new added text, bot will use the default localization for these.
