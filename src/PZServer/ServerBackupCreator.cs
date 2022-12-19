using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Threading.Tasks;

public static class ServerBackupCreator
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
            Logger.WriteLog("[BackupCreator.Start()] Server is running. Cannot create backup.");
            return;
        }

        IsRunning = true;

        if(!Directory.Exists(backupPath))
            Directory.CreateDirectory(backupPath);

        if(logChannel != null)
            await logChannel.SendMessageAsync(Localization.Get("disc_cmd_backup_server_start").KeyFormat(("folder_count", backupNamePathList.Count)));

        int i=0;
        foreach(KeyValuePair<string, string> namePathPair in backupNamePathList)
        {
            if(File.Exists(backupPath+"/"+namePathPair.Key))
                File.Delete(backupPath+"/"+namePathPair.Key);

            if(!Directory.Exists(namePathPair.Value))
            {
                Logger.WriteLog("[BackupCreator.Start()] Couldn't find path \""+namePathPair.Value+"\". Skipping...");
                continue;
            }

            ZipFile.CreateFromDirectory(namePathPair.Value, backupPath+"/"+namePathPair.Key);

            if(logChannel != null)
                await logChannel.SendMessageAsync(Localization.Get("disc_cmd_backup_server_item_done").KeyFormat(("folder_name", namePathPair.Value), ("remaining_folder_count", backupNamePathList.Count - ++i)));
        }

        IsRunning = false;

        Logger.WriteLog("[BackupCreator.Start()] Backup completed.");
                        
        if(logChannel != null)
            await logChannel.SendMessageAsync(Localization.Get("disc_cmd_backup_server_finish"));
    }
}
