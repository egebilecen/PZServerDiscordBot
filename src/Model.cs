using Newtonsoft.Json;
using System;
using System.IO;

namespace Settings
{
    public class ServerLogParserSettings
    {
        public uint PerkParserCacheDuration = 10; // minute
    }

    public class ServerScheduleSettings
    {
        public ulong ServerRestartSchedule = Convert.ToUInt64(TimeSpan.FromHours(6).TotalMinutes);
    }

    public class BotSettings
    {
        public const string settingsFile = ".\\pzdiscordbot.conf";
        public ulong        GuildId;
        public ulong        CommandChannelId;
        public ulong        LogChannelId;
        public ulong        PublicChannelId;

        public ServerLogParserSettings ServerLogParserSettings = new ServerLogParserSettings();
        public ServerScheduleSettings  ServerScheduleSettings  = new ServerScheduleSettings();

        public void Save()
        {
            File.WriteAllText(settingsFile, JsonConvert.SerializeObject(this, Formatting.None));
        }
    }
}
