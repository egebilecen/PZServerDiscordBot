using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public static class StringExtension
{
    public static string KeyFormat(this string str, params (string, string)[] formatPair)
    {
        if(formatPair.Length < 1) return str;

        foreach((string, string) pair in formatPair)
            str = str.Replace("{" + pair.Item1 + "}", pair.Item2);

        return str;
    }
}
