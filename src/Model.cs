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
        public uint ServerRestartSchedule      = Convert.ToUInt32(TimeSpan.FromHours(6).TotalMilliseconds);
        public uint WorkshopItemUpdateSchedule = Convert.ToUInt32(TimeSpan.FromMinutes(10).TotalMilliseconds);
    }

    public class BotSettings
    {
        public const string settingsFile = ".\\pzdiscordbot.conf";
        public ulong        GuildId;
        public ulong        CommandChannelId;
        public ulong        LogChannelId;
        public ulong        PublicChannelId;
        public float        VersionNumber;

        public ServerLogParserSettings ServerLogParserSettings = new ServerLogParserSettings();
        public ServerScheduleSettings  ServerScheduleSettings  = new ServerScheduleSettings();

        public void Save()
        {
            File.WriteAllText(settingsFile, JsonConvert.SerializeObject(this, Formatting.None));
        }
    }
}
