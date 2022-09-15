using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public static class IniParser
{
    public class IniData
    {
        private List<KeyValuePair<string, string>> iniData;

        public IniData(List<KeyValuePair<string, string>> data)
        {
            iniData = data;
        }

        public string GetValue(string key)
        {
            KeyValuePair<string, string> result = iniData.Single(e => e.Key == key);
            return result.Value;
        }
    }

    public static IniData Parse(string filePath)
    {
        List<KeyValuePair<string, string>> resultData = new List<KeyValuePair<string, string>>();

        try
        {
            string iniFileContent = File.ReadAllText(filePath);
            string[] lines = iniFileContent.Split(new string[]{ Environment.NewLine }, StringSplitOptions.None);

            foreach(string line in lines)
            {
                string lineTrimmed = line.Trim();

                if(string.IsNullOrEmpty(lineTrimmed) || lineTrimmed[0] == '#')
                    continue;

                string[] keyValSplit = lineTrimmed.Split('=');
                resultData.Add(new KeyValuePair<string, string>(keyValSplit[0], keyValSplit[1]));
            }
        }
        catch(Exception)
        {}

        return resultData.Count > 0 ? new IniData(resultData) : null;
    }
}
