using System;
using System.Diagnostics;
using System.IO;
using System.Threading;

public static class ServerUtility
{
    private const string  serverFile              = ".\\server.bat";
    public static Process serverProcess           = null;
    public static int     initialJavaProcessCount = 0;

    // If it works, it works!
    public static bool IsServerRunning()
    {
        int javaProcess = 0;

        foreach(Process process in Process.GetProcesses())
        {
            if(process.ProcessName.Contains("java"))
                javaProcess++;
        }

        return javaProcess > initialJavaProcessCount;
    }
    
    public static string GetServerConfigIniFilePath()
    {
        string[] fileList = Directory.GetFiles(ServerPath.serverSettingsPath, "*.ini", SearchOption.TopDirectoryOnly);
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
                serverProcess.StandardInput.WriteLine(string.Format("servermsg \"{0}\"", message));
                serverProcess.StandardInput.Flush();
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

                serverProcess = new Process();
                serverProcess.StartInfo = startInfo;
                serverProcess.Start();
            }

            return serverProcess;
        }

        public static void StopServer()
        {
            try
            {
                serverProcess.StandardInput.WriteLine("quit");
                serverProcess.StandardInput.Flush();
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
                serverProcess.StandardInput.WriteLine("save");
                serverProcess.StandardInput.Flush();
            }
            catch(Exception) 
            {}
        }

        public static void AddUser(string username, string password)
        {
            try
            {
                serverProcess.StandardInput.WriteLine(string.Format("adduser \"{0}\" \"{1}\"", username, password));
                serverProcess.StandardInput.Flush();
            }
            catch(Exception)
            {}
        }

        public static void AddUserToWhiteList(string username)
        {
            try
            {
                serverProcess.StandardInput.WriteLine(string.Format("addusertowhitelist \"{0}\"", username));
                serverProcess.StandardInput.Flush();
            }
            catch(Exception)
            {}
        }

        public static void RemoveUserFromWhiteList(string username)
        {
            try
            {
                serverProcess.StandardInput.WriteLine(string.Format("removeuserfromwhitelist \"{0}\"", username));
                serverProcess.StandardInput.Flush();
            }
            catch(Exception)
            {}
        }

        public static void BanId(ulong id)
        {
            try
            {
                serverProcess.StandardInput.WriteLine(string.Format("banid \"{0}\"", id.ToString()));
                serverProcess.StandardInput.Flush();
            }
            catch(Exception)
            {}
        }

        public static void UnbanId(ulong id)
        {
            try
            {
                serverProcess.StandardInput.WriteLine(string.Format("unbanid \"{0}\"", id.ToString()));
                serverProcess.StandardInput.Flush();
            }
            catch(Exception)
            {}
        }

        public static void BanUser(string username)
        {
            try
            {
                serverProcess.StandardInput.WriteLine(string.Format("banuser \"{0}\"", username));
                serverProcess.StandardInput.Flush();
            }
            catch(Exception)
            {}
        }

        public static void UnbanUser(string username)
        {
            try
            {
                serverProcess.StandardInput.WriteLine(string.Format("unbanuser \"{0}\"", username));
                serverProcess.StandardInput.Flush();
            }
            catch(Exception)
            {}
        }

        public static void GrantAdmin(string username)
        {
            try
            {
                serverProcess.StandardInput.WriteLine(string.Format("grantadmin \"{0}\"", username));
                serverProcess.StandardInput.Flush();
            }
            catch(Exception)
            {}
        }

        public static void RemoveAdmin(string username)
        {
            try
            {
                serverProcess.StandardInput.WriteLine(string.Format("removeadmin \"{0}\"", username));
                serverProcess.StandardInput.Flush();
            }
            catch(Exception)
            {}
        }

        public static void KickUser(string username)
        {
            try
            {
                serverProcess.StandardInput.WriteLine(string.Format("kickuser \"{0}\"", username));
                serverProcess.StandardInput.Flush();
            }
            catch(Exception)
            {}
        }

        public static void StartRain()
        {
            try
            {
                serverProcess.StandardInput.WriteLine("startrain");
                serverProcess.StandardInput.Flush();
            }
            catch(Exception)
            {}
        }

        public static void StopRain()
        {
            try
            {
                serverProcess.StandardInput.WriteLine("stoprain");
                serverProcess.StandardInput.Flush();
            }
            catch(Exception)
            {}
        }

        public static void Teleport(string username1, string username2)
        {
            try
            {
                serverProcess.StandardInput.WriteLine(string.Format("teleport \"{0}\" \"{1}\"", username1, username2));
                serverProcess.StandardInput.Flush();
            }
            catch(Exception)
            {}
        }

        public static void AddItem(string username, string item)
        {
            try
            {
                serverProcess.StandardInput.WriteLine(string.Format("additem \"{0}\" \"{1}\"", username, item));
                serverProcess.StandardInput.Flush();
            }
            catch(Exception)
            {}
        }

        public static void AddXP(string username, string perk, uint xp)
        {
            try
            {
                serverProcess.StandardInput.WriteLine(string.Format("addxp \"{0}\" \"{1}={2}\"", username, perk, xp));
                serverProcess.StandardInput.Flush();
            }
            catch(Exception)
            {}
        }

        public static void Chopper()
        {
            try
            {
                serverProcess.StandardInput.WriteLine("chopper");
                serverProcess.StandardInput.Flush();
            }
            catch(Exception)
            {}
        }

        public static void GodMode(string username)
        {
            try
            {
                serverProcess.StandardInput.WriteLine(string.Format("godmode \"{0}\"", username));
                serverProcess.StandardInput.Flush();
            }
            catch(Exception)
            {}
        }

        public static void Invisible(string username)
        {
            try
            {
                serverProcess.StandardInput.WriteLine(string.Format("invisible \"{0}\"", username));
                serverProcess.StandardInput.Flush();
            }
            catch(Exception)
            {}
        }

        public static void NoClip(string username)
        {
            try
            {
                serverProcess.StandardInput.WriteLine(string.Format("noclip \"{0}\"", username));
                serverProcess.StandardInput.Flush();
            }
            catch(Exception)
            {}
        }

        public static void ShowOptions()
        {
            try
            {
                serverProcess.StandardInput.WriteLine(string.Format("showoptions"));
                serverProcess.StandardInput.Flush();
            }
            catch(Exception)
            {}
        }

        public static void ReloadOptions()
        {
            try
            {
                serverProcess.StandardInput.WriteLine(string.Format("reloadoptions"));
                serverProcess.StandardInput.Flush();
            }
            catch(Exception)
            {}
        }
    }
}