using Newtonsoft.Json;
using System;
using System.Collections.Generic;
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
        public string ServerRestartScheduleType    = "Interval";
        public List<string> ServerRestartTimes     = new List<string>{"03:00"};

        public uint GetServerRestartSchedule()
        {
            return this.ServerRestartScheduleType.ToLower() == "interval" ? this.ServerRestartSchedule : Scheduler.GetIntervalFromTimes(this.ServerRestartTimes);
        }

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

        public Localization.LocalizationInfo LocalizationInfo = null;

        public void Save()
        {
            File.WriteAllText(SettingsFile, JsonConvert.SerializeObject(this, Formatting.Indented));
        }


    }
}
