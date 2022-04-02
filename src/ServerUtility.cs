using System.Diagnostics;
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

    public static class Commands
    {
        public static void ServerMsg(string message)
        {
            serverProcess.StandardInput.WriteLine(string.Format("servermsg \"{0}\"", message));
            serverProcess.StandardInput.Flush();
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
            if(IsServerRunning())
            {
                serverProcess.StandardInput.WriteLine("quit");
                serverProcess.StandardInput.Flush();
            }
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
            serverProcess.StandardInput.WriteLine("save");
            serverProcess.StandardInput.Flush();
        }

        public static void AddUser(string username, string password)
        {
            serverProcess.StandardInput.WriteLine(string.Format("adduser \"{0}\" \"{1}\"", username, password));
            serverProcess.StandardInput.Flush();
        }

        public static void AddUserToWhiteList(string username)
        {
            serverProcess.StandardInput.WriteLine(string.Format("addusertowhitelist \"{0}\"", username));
            serverProcess.StandardInput.Flush();
        }

        public static void RemoveUserFromWhiteList(string username)
        {
            serverProcess.StandardInput.WriteLine(string.Format("removeuserfromwhitelist \"{0}\"", username));
            serverProcess.StandardInput.Flush();
        }

        public static void BanId(ulong id)
        {
            serverProcess.StandardInput.WriteLine(string.Format("banid \"{0}\"", id.ToString()));
            serverProcess.StandardInput.Flush();
        }

        public static void UnbanId(ulong id)
        {
            serverProcess.StandardInput.WriteLine(string.Format("unbanid \"{0}\"", id.ToString()));
            serverProcess.StandardInput.Flush();
        }

        public static void BanUser(string username)
        {
            serverProcess.StandardInput.WriteLine(string.Format("banuser \"{0}\"", username));
            serverProcess.StandardInput.Flush();
        }

        public static void UnbanUser(string username)
        {
            serverProcess.StandardInput.WriteLine(string.Format("unbanuser \"{0}\"", username));
            serverProcess.StandardInput.Flush();
        }

        public static void GrantAdmin(string username)
        {
            serverProcess.StandardInput.WriteLine(string.Format("grantadmin \"{0}\"", username));
            serverProcess.StandardInput.Flush();
        }

        public static void RemoveAdmin(string username)
        {
            serverProcess.StandardInput.WriteLine(string.Format("removeadmin \"{0}\"", username));
            serverProcess.StandardInput.Flush();
        }

        public static void KickUser(string username)
        {
            serverProcess.StandardInput.WriteLine(string.Format("kickuser \"{0}\"", username));
            serverProcess.StandardInput.Flush();
        }

        public static void StartRain()
        {
            serverProcess.StandardInput.WriteLine("startrain");
            serverProcess.StandardInput.Flush();
        }

        public static void StopRain()
        {
            serverProcess.StandardInput.WriteLine("stoprain");
            serverProcess.StandardInput.Flush();
        }

        public static void Teleport(string username1, string username2)
        {
            serverProcess.StandardInput.WriteLine(string.Format("teleport \"{0}\" \"{1}\"", username1, username2));
            serverProcess.StandardInput.Flush();
        }

        public static void AddItem(string username, string item)
        {
            serverProcess.StandardInput.WriteLine(string.Format("additem \"{0}\" \"{1}\"", username, item));
            serverProcess.StandardInput.Flush();
        }

        public static void AddXP(string username, string perk, uint xp)
        {
            serverProcess.StandardInput.WriteLine(string.Format("addxp \"{0}\" \"{1}={2}\"", username, perk, xp));
            serverProcess.StandardInput.Flush();
        }

        public static void Chopper()
        {
            serverProcess.StandardInput.WriteLine("chopper");
            serverProcess.StandardInput.Flush();
        }

        public static void GodMode(string username)
        {
            serverProcess.StandardInput.WriteLine(string.Format("godmode \"{0}\"", username));
            serverProcess.StandardInput.Flush();
        }

        public static void Invisible(string username)
        {
            serverProcess.StandardInput.WriteLine(string.Format("invisible \"{0}\"", username));
            serverProcess.StandardInput.Flush();
        }

        public static void NoClip(string username)
        {
            serverProcess.StandardInput.WriteLine(string.Format("noclip \"{0}\"", username));
            serverProcess.StandardInput.Flush();
        }
    }
}