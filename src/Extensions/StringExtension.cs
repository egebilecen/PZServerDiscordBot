using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public static class StringExtension
{
    public static string KeyFormat(this string str, params (string, object)[] formatPair)
    {
        if(formatPair.Length < 1) return str;

        foreach((string, object) pair in formatPair)
        {
            if(!str.Contains(pair.Item1))
                Logger.WriteLog($"KeyFormat() - Key \"{pair.Item1}\" not found! str: {str}");

            str = str.Replace("{" + pair.Item1 + "}", pair.Item2.ToString());
        }

        return str;
    }
}
