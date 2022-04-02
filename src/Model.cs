using Newtonsoft.Json;
using System.IO;

namespace Model
{
    public class ServerLogParserSettings
    {
        public uint PerkParserCacheDuration = 10; // minute
    }

    public class BotSettings
    {
        public const string settingsFile = ".\\pzdiscordbot.conf";
        public ulong        GuildId;
        public ulong        BotChannelId;

        public ServerLogParserSettings ServerLogParserSettings = new ServerLogParserSettings();

        public void Save()
        {
            File.WriteAllText(settingsFile, JsonConvert.SerializeObject(this, Formatting.None));
        }
    }
}
