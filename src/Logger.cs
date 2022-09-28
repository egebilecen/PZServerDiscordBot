using System;
using System.IO;

public static class Logger
{
    private const string logFile = ".\\pzbot.log";

    public static void WriteLog(string text)
    {
        var file = File.AppendText(logFile);
        file.WriteLine(text);
        file.Close();
    }

    public static void LogException(Exception ex, string additional_msg="")
    {
        string ex_msg = "\n---------------\n" +
                        "Exception: "+ex.GetType().FullName +
                        "\nMessage: "+ex.Message +
                        "\nStack trace: "+ex.StackTrace.Trim() +
                        "\nDate: "+DateTime.Now.ToString("dd/MM/yyyy, HH:mm:ss") +
                        (additional_msg != "" ? "\n"+additional_msg : "") +
                        "\n---------------\n";
        File.AppendAllText(logFile, ex_msg);
    }
}
