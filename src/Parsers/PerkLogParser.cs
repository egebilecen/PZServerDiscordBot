using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace ServerLogParsers
{
    public static class PerkLog
    {
        public const  string tempLogFile = ".\\perkLog.temp";
        public static Dictionary<string, UserPerkData> perkCache = null;
        public static DateTime? lastCacheTime = null;

        public class UserPerkData
        {
            public string Username;
            public ulong  SteamId;
            public string LogDate;
            public Dictionary<string, int> Perks = new Dictionary<string, int>();
        }

        public static Regex regex = new Regex(@"\[(.*?)]\ \[(\d+)\]\[(.*?)\]\[.*?\]\[(.*?)\]");

        private static string GetContent(int nthFile=0)
        {
            string zomboidLogDirectory = ServerPath.LogPath();

            if(!Directory.Exists(zomboidLogDirectory)) return string.Empty;

            List<FileInfo> perkLogFiles    = new List<FileInfo>();
            List<FileInfo> sortedDirectory = new DirectoryInfo(zomboidLogDirectory).GetFiles()
                                                                                   .OrderBy(file => file.LastWriteTime)
                                                                                   .ToList();

            foreach(FileInfo _fileInfo in sortedDirectory)
            {
                if(_fileInfo.Name.Contains("PerkLog"))
                    perkLogFiles.Add(_fileInfo);
            }

            if(perkLogFiles.Count == 0
            || nthFile > perkLogFiles.Count - 1) return string.Empty;

            FileInfo fileInfo = perkLogFiles[nthFile];

            try 
            {
                using(FileStream fileStream     = File.Open(fileInfo.FullName, FileMode.Open, FileAccess.Read, FileShare.Read))
                using(StreamReader streamReader = new StreamReader(fileStream))
                {
                    string fileContent = streamReader.ReadToEnd();
                    return fileContent;
                }
            }
            catch(IOException)
            {
                if(File.Exists(tempLogFile))
                    File.Delete(tempLogFile);

                File.Copy(fileInfo.FullName, tempLogFile);

                using(FileStream fileStream     = File.Open(tempLogFile, FileMode.Open, FileAccess.Read, FileShare.Read))
                using(StreamReader streamReader = new StreamReader(fileStream))
                {
                    string fileContent = streamReader.ReadToEnd();
                    return fileContent;
                }
            }
        }

        private static Dictionary<string, UserPerkData> Parse(int nthFile=0)
        {
            string logContent = GetContent(nthFile);

            if(logContent == string.Empty) return null;

            
            Dictionary<string, UserPerkData> userPerkDataList = new Dictionary<string, UserPerkData>();
            string[] logLines = logContent.Split('\n').Reverse().ToArray();

            foreach(string line in logLines)
            {
                var regexMatch = regex.Match(line);

                if(!regexMatch.Success
                || !regexMatch.Groups[4].Value.Contains("Strength=")) continue;

                if(userPerkDataList.ContainsKey(regexMatch.Groups[3].Value))
                    continue;

                UserPerkData userPerkData = new UserPerkData();
                userPerkData.LogDate      = regexMatch.Groups[1].Value;
                userPerkData.SteamId      = Convert.ToUInt64(regexMatch.Groups[2].Value);
                userPerkData.Username     = regexMatch.Groups[3].Value;

                string[] perks = regexMatch.Groups[4].Value.Split(',').Select(elem => elem.Trim()).ToArray();

                foreach(string perk in perks)
                {
                    string[] perkPair = perk.Split('=');
                    userPerkData.Perks.Add(perkPair[0], Convert.ToInt32(perkPair[1]));
                }

                userPerkDataList.Add(userPerkData.Username, userPerkData);
            }

            return userPerkDataList;
        }

        public static Dictionary<string, UserPerkData> Get(int nthFile=0)
        {
            if(perkCache     == null
            || lastCacheTime == null
            || DateTime.Now.Subtract((DateTime)lastCacheTime).TotalMinutes >= Application.botSettings.ServerLogParserSettings.PerkParserCacheDuration)
            {
                perkCache     = Parse(nthFile);
                lastCacheTime = DateTime.Now;

                return perkCache;
            }
            
            return perkCache;
        }
    }
}
