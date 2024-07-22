using System;
using System.Diagnostics;
using System.IO;
using System.Threading;

public static class ServerUtility
{
    private const string  serverFile    = ".\\server.bat";
    public static Process ServerProcess = null;
    public static bool AbortNextTimedServerRestart = false;

    public static bool CanStartServer()
    {
        return !IsServerRunning() && !ServerBackupCreator.IsRunning;
    }

    public static bool IsServerRunning()
    {
        return ServerProcess != null && !ServerProcess.HasExited;
    }
    
    public static string GetServerConfigIniFilePath()
    {
        string[] fileList = Directory.GetFiles(ServerPath.ServerSettingsPath(), "*.ini", SearchOption.TopDirectoryOnly);
        return fileList.Length > 0 ? fileList[0] : null;
    }

    public static string GetServerConfigIniFileName(bool cropExtension = false)
    {
        string filePath = GetServerConfigIniFilePath();
        return string.IsNullOrEmpty(filePath) ? null 
                                              : (cropExtension ? Path.GetFileNameWithoutExtension(filePath) 
                                                               : Path.GetFileName(filePath));
    }

    public static uint InitiateServerRestart(uint intervalMS)
    {
        uint restartInMinutes = intervalMS / (60 * 1000);
        
        // This updated interval will be set back to the value in settings when StartServer() is called.
        Scheduler.GetItem("ServerRestart")?.UpdateInterval(intervalMS);
        ResetServerRestartAnnouncerInterval();
        
        Application.StartTime = DateTime.UtcNow.AddMinutes(restartInMinutes);

        return restartInMinutes;
    }

    public static void ResetServerRestartInterval()
    {
        Scheduler.GetItem("ServerRestart")?.UpdateInterval(Application.BotSettings.ServerScheduleSettings.GetServerRestartSchedule());
        ResetServerRestartAnnouncerInterval();
    }

    public static void ResetServerRestartAnnouncerInterval()
    {
        ScheduleItem serverRestartAnnouncer = Scheduler.GetItem("ServerRestartAnnouncer");
        
        if(serverRestartAnnouncer != null)
        {
            serverRestartAnnouncer.Args = null;
            serverRestartAnnouncer.UpdateInterval();
        }
    }

    public static class Commands
    {
        public static void ServerMsg(string message)
        {
            try
            {
                ServerProcess.StandardInput.WriteLine(string.Format("servermsg \"{0}\"", message));
                ServerProcess.StandardInput.Flush();
            }
            catch(Exception)
            {}
        }

        public static Process StartServer()
        {
            if(CanStartServer())
            {
                // Set server restart interval value back to the value defined in settings just in case of some function
                // updated the default interval value for earlier restart.
                ResetServerRestartInterval();

                ProcessStartInfo startInfo = new ProcessStartInfo(serverFile)
                {
                    RedirectStandardInput = true,
                    UseShellExecute = false
                };

                ServerProcess = new Process
                {
                    StartInfo = startInfo
                };
                ServerProcess.Start();
            }

            return ServerProcess;
        }

        public static void StopServer()
        {
            SaveServer();

            try
            {
                ServerProcess.StandardInput.WriteLine("quit");
                ServerProcess.StandardInput.Flush();
            }
            catch(Exception)
            {}
        }

        public static void RestartServer()
        {
            if(IsServerRunning())
            {
                StopServer();

                while(IsServerRunning())
                    Thread.Sleep(250);

                StartServer();
            }
        }

        public static void SaveServer()
        {
            try
            {
                ServerProcess.StandardInput.WriteLine("save");
                ServerProcess.StandardInput.Flush();
            }
            catch(Exception) 
            {}
        }

        public static void AddUser(string username, string password)
        {
            try
            {
                ServerProcess.StandardInput.WriteLine(string.Format("adduser \"{0}\" \"{1}\"", username, password));
                ServerProcess.StandardInput.Flush();
            }
            catch(Exception)
            {}
        }

        public static void AddUserToWhiteList(string username)
        {
            try
            {
                ServerProcess.StandardInput.WriteLine(string.Format("addusertowhitelist \"{0}\"", username));
                ServerProcess.StandardInput.Flush();
            }
            catch(Exception)
            {}
        }

        public static void RemoveUserFromWhiteList(string username)
        {
            try
            {
                ServerProcess.StandardInput.WriteLine(string.Format("removeuserfromwhitelist \"{0}\"", username));
                ServerProcess.StandardInput.Flush();
            }
            catch(Exception)
            {}
        }

        public static void BanId(ulong id)
        {
            try
            {
                ServerProcess.StandardInput.WriteLine(string.Format("banid \"{0}\"", id.ToString()));
                ServerProcess.StandardInput.Flush();
            }
            catch(Exception)
            {}
        }

        public static void UnbanId(ulong id)
        {
            try
            {
                ServerProcess.StandardInput.WriteLine(string.Format("unbanid \"{0}\"", id.ToString()));
                ServerProcess.StandardInput.Flush();
            }
            catch(Exception)
            {}
        }

        public static void BanUser(string username)
        {
            try
            {
                ServerProcess.StandardInput.WriteLine(string.Format("banuser \"{0}\"", username));
                ServerProcess.StandardInput.Flush();
            }
            catch(Exception)
            {}
        }

        public static void UnbanUser(string username)
        {
            try
            {
                ServerProcess.StandardInput.WriteLine(string.Format("unbanuser \"{0}\"", username));
                ServerProcess.StandardInput.Flush();
            }
            catch(Exception)
            {}
        }

        public static void GrantAdmin(string username)
        {
            try
            {
                ServerProcess.StandardInput.WriteLine(string.Format("grantadmin \"{0}\"", username));
                ServerProcess.StandardInput.Flush();
            }
            catch(Exception)
            {}
        }

        public static void RemoveAdmin(string username)
        {
            try
            {
                ServerProcess.StandardInput.WriteLine(string.Format("removeadmin \"{0}\"", username));
                ServerProcess.StandardInput.Flush();
            }
            catch(Exception)
            {}
        }

        public static void KickUser(string username)
        {
            try
            {
                ServerProcess.StandardInput.WriteLine(string.Format("kickuser \"{0}\"", username));
                ServerProcess.StandardInput.Flush();
            }
            catch(Exception)
            {}
        }

        public static void StartRain()
        {
            try
            {
                ServerProcess.StandardInput.WriteLine("startrain");
                ServerProcess.StandardInput.Flush();
            }
            catch(Exception)
            {}
        }

        public static void StopRain()
        {
            try
            {
                ServerProcess.StandardInput.WriteLine("stoprain");
                ServerProcess.StandardInput.Flush();
            }
            catch(Exception)
            {}
        }

        public static void Teleport(string username1, string username2)
        {
            try
            {
                ServerProcess.StandardInput.WriteLine(string.Format("teleport \"{0}\" \"{1}\"", username1, username2));
                ServerProcess.StandardInput.Flush();
            }
            catch(Exception)
            {}
        }

        public static void AddItem(string username, string item)
        {
            try
            {
                ServerProcess.StandardInput.WriteLine(string.Format("additem \"{0}\" \"{1}\"", username, item));
                ServerProcess.StandardInput.Flush();
            }
            catch(Exception)
            {}
        }

        public static void AddXP(string username, string perk, uint xp)
        {
            try
            {
                ServerProcess.StandardInput.WriteLine(string.Format("addxp \"{0}\" \"{1}={2}\"", username, perk, xp));
                ServerProcess.StandardInput.Flush();
            }
            catch(Exception)
            {}
        }

        public static void Chopper()
        {
            try
            {
                ServerProcess.StandardInput.WriteLine("chopper");
                ServerProcess.StandardInput.Flush();
            }
            catch(Exception)
            {}
        }

        public static void GodMode(string username)
        {
            try
            {
                ServerProcess.StandardInput.WriteLine(string.Format("godmode \"{0}\"", username));
                ServerProcess.StandardInput.Flush();
            }
            catch(Exception)
            {}
        }

        public static void Invisible(string username)
        {
            try
            {
                ServerProcess.StandardInput.WriteLine(string.Format("invisible \"{0}\"", username));
                ServerProcess.StandardInput.Flush();
            }
            catch(Exception)
            {}
        }

        public static void NoClip(string username)
        {
            try
            {
                ServerProcess.StandardInput.WriteLine(string.Format("noclip \"{0}\"", username));
                ServerProcess.StandardInput.Flush();
            }
            catch(Exception)
            {}
        }

        public static void ShowOptions()
        {
            try
            {
                ServerProcess.StandardInput.WriteLine(string.Format("showoptions"));
                ServerProcess.StandardInput.Flush();
            }
            catch(Exception)
            {}
        }

        public static void ReloadOptions()
        {
            try
            {
                ServerProcess.StandardInput.WriteLine(string.Format("reloadoptions"));
                ServerProcess.StandardInput.Flush();
            }
            catch(Exception)
            {}
        }

        public static void ChangeOption(string option, string newOption)
        {
            try
            {
                ServerProcess.StandardInput.WriteLine(string.Format("changeoption {0} \"{1}\"", option, newOption));
                ServerProcess.StandardInput.Flush();
            }
            catch (Exception)
            {}
        }
    }
}
