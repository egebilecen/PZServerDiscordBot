using System;

public static class ServerPath
{
    public static string BasePath = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile) + "\\Zomboid\\";

    public static string ServerLogsPath()
    {
        string path = BasePath + "Logs\\";
        return path;
    }

    public static string ServerSettingsPath()
    {
        string path = BasePath + "Server\\";
        return path;
    }

    public static string ServerSavesPath()
    {
        string path = BasePath + "Saves\\Multiplayer\\" + ServerUtility.GetServerConfigIniFileName(true) + "\\";
        return path;
    }

    public static string MapTimeFilePath()
    {
        string path = ServerSavesPath() + "map_t.bin";
        return path;
    }
}
