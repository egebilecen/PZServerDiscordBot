using Newtonsoft.Json;
using System.IO;

namespace Settings
{
    public class ServerLogParserSettings
    {
        public uint PerkParserCacheDuration = 10; // minute
    }

    public class BotSettings
    {
        public const string settingsFile = ".\\pzdiscordbot.conf";
        public ulong        GuildId;
        public ulong        CommandChannelId;
        public ulong        LogChannelId;
        public ulong        PublicChannelId;

        public ServerLogParserSettings ServerLogParserSettings = new ServerLogParserSettings();

        public void Save()
        {
            File.WriteAllText(settingsFile, JsonConvert.SerializeObject(this, Formatting.None));
        }
    }
}
