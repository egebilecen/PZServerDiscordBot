using System.Diagnostics;

public static class ServerUtility
{
    private const string  serverFile              = ".\\server.bat";
    public static Process serverProcess           = null;
    public static int     initialJavaProcessCount = 0;

    public static Process StartServer()
    {
        ProcessStartInfo startInfo      = new ProcessStartInfo(serverFile);
        startInfo.RedirectStandardInput = true;
        startInfo.UseShellExecute       = false;

        serverProcess = new Process();
        serverProcess.StartInfo = startInfo;
        serverProcess.Start();

        return serverProcess;
    }

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
}