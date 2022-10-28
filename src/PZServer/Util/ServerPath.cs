using System;
using System.IO;

public static class ServerPath
{
    public static string BasePath = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile) + "\\Zomboid\\";

    public static string LogPath()
    {
        string path = BasePath + "Logs\\";
        return path;
    }

    public static string ServerSettingsPath()
    {
        string path = BasePath + "Server\\";
        return path;
    }
}
