# Discord Bot for Managing Project Zomboid Server
This bot is written for people to easily manage their server using Discord. Please check **bot configuration** section.

# Features
- Automated server restart schedule with ingame and discord warning. (Warnings are announced when 1 hour, 30 min, 15 min, 5 min and 1 min left until server restart. Restart interval can be configured with bot commands.)
![Automated Server Restart Example](https://i.ibb.co/SQWfnL1/Screenshot-1.png)
- Executing server commands through bot commands. (For example; saving server, kicking player, teleporting player, starting/stopping rain, making admin and so on. Full list will be at the bottom and will be listed under available commands.)
![Server Commands Example](https://i.ibb.co/FnH50cH/Screenshot-3.png)
- Perk Parser with cache system. (Bot automatically parses the last perk log file that holds the player skills when they login to server. This can be used to aid players that died to a bug but they can't remember their skills. As stated before, server logs the player skills only when they log into server. If player levels up a skill after connecting to server, it won't appear in log unless player logs into server afterwards again.) When command for perk parser is invoked, bot will parse the file and save it contents in memory until cache (in minutes) expires. This is for to increase efficiency as bot will not have to parse the same file each time. In a situation where fresh data needed, **!reset_perk_cache** command can be used to reset the cache.                
![Perk Parser Example](https://i.ibb.co/TmnxYQz/Screenshot-2.png)

# Bot Configuration
This bot uses 3 different channels to operate. First channel is *Public* channel where users can interact with bot. Second channel is *Command* channel where must set to be only visible to server admins. This channel is used for executing server management and bot configuration commands. Third channel is *Log* channel. There aren't any commands to execute in this channel and it's set for bot to announce stuff. After the bot launched for first time (or not configured), it will ask you to configure the mentioned three channels using **!set_command_channel**, **!set_log_channel** and **!set_public_channel** commands. Those commands are very easy to use. Just reply to any channel with the tag of the channel you want the bot to be configured in. For example: `!set_public_channel #bot-public`

![Bot Configuration Example](https://i.ibb.co/CsgGjkn/Screenshot-1.png)

# Bot Commands
**!help** command can be used in any of configured 3 channels which bot will respond with the available command list for *that channel*.
