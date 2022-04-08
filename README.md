# Discord Bot for Managing Project Zomboid Server
This bot is written for people to easily manage their server using Discord.

# Features
- Automated server restart schedule with ingame and discord warning. (Warnings are announced when 1 hour, 30 min, 15 min, 5 min and 1 min left until server restart. Restart interval can be configured with bot commands.)
![Automated Server Restart Example](https://i.ibb.co/SQWfnL1/Screenshot-1.png)
- Executing server commands through bot commands. (For example; saving server, kicking player, teleporting player, starting/stopping rain, making admin and so on. Full list will be at the bottom and will be listed under available commands.)
![Server Commands Example](https://i.ibb.co/FnH50cH/Screenshot-3.png)
- Perk Parser with cache system. (Bot automatically parses the last perk log file that holds the player skills when they login to server. This can be used to aid players that died to a bug but they can't remember their skills. As stated before, server logs the player skills only when they log into server. If player levels up a skill after connecting to server, it won't appear in log unless player logs into server afterwards again.) When command for perk parser is invoked, bot will parse the file and save it contents in memory until cache (in minutes) expires. This is for to increase efficiency as bot will not have to parse the same file each time. In a situation where fresh data needed, **!reset_perk_cache** command can be used to reset the cache.                
![Perk Parser Example](https://i.ibb.co/TmnxYQz/Screenshot-2.png)
