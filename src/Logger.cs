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
}
