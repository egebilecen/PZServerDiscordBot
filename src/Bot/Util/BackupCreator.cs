using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Threading.Tasks;

public static class BackupCreator
{
    public static bool IsRunning { get; private set; } = false;
    private static readonly string backupPath = "./server_backup";

    private static readonly Dictionary<string, string> backupNamePathList = new Dictionary<string, string> 
    {
        { "db.zip", ServerPath.ServerDatabasePath() },
        { "server.zip", ServerPath.ServerSettingsPath() },
        { "Lua.zip", ServerPath.ServerLuaPath() },
        { "Saves.zip", ServerPath.ServerBaseSavesPath() },
    };

    public static async Task Start()
    {
        var logChannel = DiscordUtility.GetTextChannelById(Application.BotSettings.LogChannelId);

        if(ServerUtility.IsServerRunning())
        {
            Logger.WriteLog(string.Format("[{0}][BackupCreator.Start()] Server is running. Cannot create backup.", Logger.GetLoggingDate()));
            return;
        }

        IsRunning = true;

        if(!Directory.Exists(backupPath))
            Directory.CreateDirectory(backupPath);

        if(logChannel != null)
            await logChannel.SendMessageAsync("Server backup started. Total of **"+backupNamePathList.Count.ToString()+" folder(s)** will be backed up.");

        int i=0;
        foreach(KeyValuePair<string, string> namePathPair in backupNamePathList)
        {
            if(File.Exists(backupPath+"/"+namePathPair.Key))
                File.Delete(backupPath+"/"+namePathPair.Key);

            if(!Directory.Exists(namePathPair.Value))
            {
                Logger.WriteLog(string.Format("[{0}][BackupCreator.Start()] Couldn't find path \""+namePathPair.Value+"\". Skipping...", Logger.GetLoggingDate()));
                continue;
            }

            ZipFile.CreateFromDirectory(namePathPair.Value, backupPath+"/"+namePathPair.Key);

            if(logChannel != null)
                await logChannel.SendMessageAsync("Backup of `"+namePathPair.Value+"` is done. **("+(backupNamePathList.Count - ++i).ToString()+" folder left)**");
        }

        IsRunning = false;

        Logger.WriteLog(string.Format("[{0}][BackupCreator.Start()] Backup completed.", Logger.GetLoggingDate()));
                        
        if(logChannel != null)
            await logChannel.SendMessageAsync("Server backup is completed!");
    }
}
