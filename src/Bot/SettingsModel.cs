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
        public uint ServerRestartSchedule          = Convert.ToUInt32(TimeSpan.FromHours(6).TotalMilliseconds);
        public uint WorkshopItemUpdateSchedule     = Convert.ToUInt32(TimeSpan.FromMinutes(10).TotalMilliseconds);
        public uint WorkshopItemUpdateRestartTimer = Convert.ToUInt32(TimeSpan.FromMinutes(15).TotalMilliseconds);
    }

    public class BotFeatureSettings
    {
        public bool AutoServerStart     = false;
        public bool NonPublicModLogging = false;
    }

    public class BotSettings
    {
        [JsonIgnore]
        public const string SettingsFile = ".\\pzdiscordbot.conf";
        public ulong        GuildId;
        public ulong        CommandChannelId;
        public ulong        LogChannelId;
        public ulong        PublicChannelId;

        public ServerLogParserSettings ServerLogParserSettings = new ServerLogParserSettings();
        public ServerScheduleSettings  ServerScheduleSettings  = new ServerScheduleSettings();
        public BotFeatureSettings      BotFeatureSettings      = new BotFeatureSettings();

        public void Save()
        {
            File.WriteAllText(SettingsFile, JsonConvert.SerializeObject(this, Formatting.Indented));
        }
    }
}
