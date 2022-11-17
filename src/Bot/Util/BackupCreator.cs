using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;

public class StreamWithProgress : Stream
{
    private readonly Stream _stream;
    private readonly IProgress<int> _readProgress;
    private readonly IProgress<int> _writeProgress;

    public StreamWithProgress(Stream stream, IProgress<int> readProgress, IProgress<int> writeProgress)
    {
        _stream = stream;
        _readProgress = readProgress;
        _writeProgress = writeProgress;
    }

    public override bool CanRead { get { return _stream.CanRead; } }
    public override bool CanSeek {  get { return _stream.CanSeek; } }
    public override bool CanWrite {  get { return _stream.CanWrite; } }
    public override long Length {  get { return _stream.Length; } }
    public override long Position
    {
        get { return _stream.Position; }
        set { _stream.Position = value; }
    }

    public override void Flush() { _stream.Flush(); }
    public override long Seek(long offset, SeekOrigin origin) { return _stream.Seek(offset, origin); }
    public override void SetLength(long value) { _stream.SetLength(value); }

    public override int Read(byte[] buffer, int offset, int count)
    {
        int bytesRead = _stream.Read(buffer, offset, count);

        _readProgress?.Report(bytesRead);
        return bytesRead;
    }

    public override void Write(byte[] buffer, int offset, int count)
    {
        _stream.Write(buffer, offset, count);
        _writeProgress?.Report(count);
    }
}

public static class BackupCreator
{
    public static bool IsRunning { get; private set; } = false;
    private static readonly string backupPath = "./server_backup";

    private static readonly Dictionary<string, string> backupNamePathList = new Dictionary<string, string> 
    {
        { "db.zip", ServerPath.ServerDatabasePath() },
        { "server.zip", ServerPath.ServerSettingsPath() },
        { "Lua.zip", ServerPath.ServerLuaPath() },
    };
    private static readonly Dictionary<string, bool> backupProgressTracker = new Dictionary<string, bool>();

    public static void Start()
    {
        var logChannel = DiscordUtility.GetTextChannelById(Application.BotSettings.LogChannelId);

        if(ServerUtility.IsServerRunning())
        {
            Logger.WriteLog(string.Format("[{0}][BackupCreator.Start()] Server is running. Cannot create backup.", Logger.GetLoggingDate()));
            
            if(logChannel != null)
                logChannel.SendMessageAsync("Cannot create a backup while server is running.");
            return;
        }

        IsRunning = true;
        backupProgressTracker.Clear();

        // Will be set to true when all files added into archive.
        foreach(string key in backupNamePathList.Keys)
            backupProgressTracker[key] = false;

        if(!Directory.Exists(backupPath))
            Directory.CreateDirectory(backupPath);

        if(logChannel != null)
            logChannel.SendMessageAsync("Server backup started.");

        bool isHandlerAssigned = false;

        foreach(KeyValuePair<string, string> namePathPair in backupNamePathList)
        {
            if(File.Exists(backupPath+"/"+namePathPair.Key))
                File.Delete(backupPath+"/"+namePathPair.Key);

            if(!Directory.Exists(namePathPair.Value))
            {
                Logger.WriteLog(string.Format("[{0}][BackupCreator.Start()] Couldn't find path \""+namePathPair.Value+"\". Skipping...", Logger.GetLoggingDate()));
                backupProgressTracker[namePathPair.Key] = true;
                continue;
            }

            isHandlerAssigned = true;

            CreateFromDirectory(namePathPair.Value, backupPath+"/"+namePathPair.Key, new ProgressReporter<double>(p =>
            {
                string backupText = string.Format("Backup in progress for `{0}`: **{1:P2}**.", namePathPair.Value, p);

                if(logChannel != null)
                    logChannel.SendMessageAsync(backupText);

                if(p == 1)
                {
                    bool allCompleted = true;
                    backupProgressTracker[namePathPair.Key] = true;

                    foreach(KeyValuePair<string, bool> progressPair in backupProgressTracker)
                    {
                        if(progressPair.Value == false)
                        {
                            allCompleted = false;
                            break;
                        }
                    }

                    IsRunning = !allCompleted;

                    if(allCompleted)
                        logChannel.SendMessageAsync("Server backup is completed!");
                }
            }));
        }

        if(!isHandlerAssigned)
        {
            if(logChannel != null)
                logChannel.SendMessageAsync("There isn't anything to backup.");

            IsRunning = false;
        }
    }

    private static void CreateFromDirectory(string sourceDirectoryName, string destinationArchiveFileName, IProgress<double> progress)
    {
        sourceDirectoryName = Path.GetFullPath(sourceDirectoryName);

        FileInfo[] sourceFiles = new DirectoryInfo(sourceDirectoryName).GetFiles("*", SearchOption.AllDirectories);
        
        double totalBytes = sourceFiles.Sum(f => f.Length);
        long currentBytes = 0;

        using (ZipArchive archive = ZipFile.Open(destinationArchiveFileName, ZipArchiveMode.Create))
        {
            foreach (FileInfo file in sourceFiles)
            {
                string entryName = file.FullName.Substring(sourceDirectoryName.Length);
                ZipArchiveEntry entry = archive.CreateEntry(entryName);

                entry.LastWriteTime = file.LastWriteTime;

                using (Stream inputStream = File.OpenRead(file.FullName))
                using (Stream outputStream = entry.Open())
                {
                    Stream progressStream = new StreamWithProgress(inputStream,
                        new ProgressReporter<int>(i =>
                        {
                            currentBytes += i;
                            progress.Report(currentBytes / totalBytes);
                        }), null);

                    progressStream.CopyTo(outputStream);
                }
            }
        }
    }

    private static void ExtractToDirectory(string sourceArchiveFileName, string destinationDirectoryName, IProgress<double> progress)
    {
        using (ZipArchive archive = ZipFile.OpenRead(sourceArchiveFileName))
        {
            double totalBytes = archive.Entries.Sum(e => e.Length);
            long currentBytes = 0;

            foreach (ZipArchiveEntry entry in archive.Entries)
            {
                string fileName = Path.Combine(destinationDirectoryName, entry.FullName);

                Directory.CreateDirectory(Path.GetDirectoryName(fileName));
                using (Stream inputStream = entry.Open())
                using(Stream outputStream = File.OpenWrite(fileName))
                {
                    Stream progressStream = new StreamWithProgress(outputStream, null,
                        new ProgressReporter<int>(i =>
                        {
                            currentBytes += i;
                            progress.Report(currentBytes / totalBytes);
                        }));

                    inputStream.CopyTo(progressStream);
                }

                File.SetLastWriteTime(fileName, entry.LastWriteTime.LocalDateTime);
            }
        }
    }
}

public class ProgressReporter<T> : IProgress<T>
{
    private T lastProgress = default;
    private readonly Action<T> _handler;

    public ProgressReporter(Action<T> handler)
    {
        _handler = handler;
    }

    void IProgress<T>.Report(T value)
    {
        if(!EqualityComparer<T>.Default.Equals(value, lastProgress))
            _handler(value);

        lastProgress = value;
    }
}
