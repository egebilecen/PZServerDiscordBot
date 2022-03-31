using Newtonsoft.Json;
using System.IO;

namespace Model
{
    public class BotSettings
    {
        public const string settingsFile = ".\\pzdiscordbot.conf";
        public ulong GuildId;
        public ulong BotChannelId;

        public void Save()
        {
            File.WriteAllText(settingsFile, JsonConvert.SerializeObject(this, Formatting.None));
        }
    }
}
