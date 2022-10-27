using System;
using System.IO;

public static class ServerPath
{
    public static string BasePath = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile) + "\\Zomboid\\";

    public static string LogPath()
    {
        string path = BasePath + "Logs\\";

        if(!Directory.Exists(path))
            path = BasePath + "Zomboid\\Logs\\";

        return path;
    }

    public static string ServerSettingsPath()
    {
        string path = BasePath + "Server\\";

        if(!Directory.Exists(path))
            path = BasePath + "Zomboid\\Server\\";

        return path;
    }
}
