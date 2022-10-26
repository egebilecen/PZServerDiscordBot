using System;
using System.Diagnostics;
using System.IO;
using System.Threading;

public static class ServerUtility
{
    private const string  serverFile    = ".\\server.bat";
    public static Process ServerProcess = null;

    public static bool IsServerRunning()
    {
        return ServerProcess != null && !ServerProcess.HasExited;
    }
    
    public static string GetServerConfigIniFilePath()
    {
        string[] fileList = Directory.GetFiles(ServerPath.ServerSettingsPath(), "*.ini", SearchOption.TopDirectoryOnly);
        return fileList.Length > 0 ? fileList[0] : null;
    }

    public static string GetServerConfigIniFileName()
    {
        string filePath = GetServerConfigIniFilePath();
        return string.IsNullOrEmpty(filePath) ? null : Path.GetFileName(filePath);
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
            if(!IsServerRunning())
            {
                ProcessStartInfo startInfo      = new ProcessStartInfo(serverFile);
                startInfo.RedirectStandardInput = true;
                startInfo.UseShellExecute       = false;

                ServerProcess = new Process();
                ServerProcess.StartInfo = startInfo;
                ServerProcess.Start();

                ScheduleItem serverRestartSchedule  = Scheduler.GetItem("ServerRestart");
                ScheduleItem serverRestartAnnouncer = Scheduler.GetItem("ServerRestartAnnouncer");

                if(serverRestartSchedule != null)
                    serverRestartSchedule.UpdateInterval();

                if(serverRestartAnnouncer != null)
                    serverRestartAnnouncer.UpdateInterval();
            }

            return ServerProcess;
        }

        public static void StopServer()
        {
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
    }
}