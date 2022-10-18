using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public static class ServerPath
{
    public static string BasePath = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile) + "\\Zomboid\\";

    public static string LogPath()
    {
        return BasePath + "Logs\\";
    }

    public static string ServerSettingsPath()
    {
        return BasePath + "Server\\";
    }
}
