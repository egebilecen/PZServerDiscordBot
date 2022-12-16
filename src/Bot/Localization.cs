using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;

public static class Localization
{
    public const string LocalizationFile = "./localization.json";
    private static Dictionary<string, string> localization = null;
    private static readonly Dictionary<string, string> defaultLocalization = new Dictionary<string, string>
    {
        // Warnings
        { "warn_debug_mode", "WARNING: Bot is running in DEBUG configuration." },

        // Errors
        { "err_bot_token", $"Couldn't retrieve bot token from \"bot_token.txt\" file.\nPlease refer to \"{Application.BotRepoURL}#writing-the-discord-bot-token-into-file\"." },
    };

    static Localization()
    {
        if(!File.Exists(LocalizationFile))
        {
            CreateFile();
            return;
        }
            
        try
        {
            localization = JsonConvert.DeserializeObject<Dictionary<string, string>>(File.ReadAllText(LocalizationFile));
        }
        catch(Exception ex)
        { 
            Logger.LogException(ex, "Localization constructor"); 
        }
    }

    private static void CreateFile()
    {
        File.WriteAllText(LocalizationFile, JsonConvert.SerializeObject(localization ?? defaultLocalization, Formatting.Indented));
    }

    public static string Get(string key)
    {
        if(localization == null)
        {
            localization = defaultLocalization;
            Logger.WriteLog("[Localization] No localization found, using default.");
        }

        if(localization.ContainsKey(key))
            return localization[key];

        Logger.WriteLog($"[Localization] No localization found for key \"{key}\"");
        return "LOCALIZATION EROR";
    }
}
