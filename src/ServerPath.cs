using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public static class ServerPath
{
    public static string basePath           = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile) + "\\Zomboid\\";
    public static string logPath            = basePath + "Logs\\";
    public static string serverSettingsPath = basePath + "Server\\";
}
