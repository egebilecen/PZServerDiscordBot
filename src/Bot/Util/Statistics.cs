using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Management;

public static class Statistics
{
    // http://www.allenconway.net/2013/07/get-cpu-usage-across-all-cores-in-c.html
    public static ulong GetTotalCPUUsagePercentage()
    {
        ManagementObjectSearcher searcher = new ManagementObjectSearcher("SELECT * FROM Win32_PerfFormattedData_PerfOS_Processor");
        
        var cpuUsageList = searcher.Get().Cast<ManagementObject>().Select(mo => new {
            Name = mo["Name"],
            Usage = mo["PercentProcessorTime"]
        }).ToList();

        var query    = cpuUsageList.Where(x => x.Name.ToString() == "_Total").Select(x => x.Usage);
        var cpuUsage = query.SingleOrDefault();
        
        return (ulong)cpuUsage;
    }

    // https://stackoverflow.com/a/31434952/8277139
    public static double GetTotalRAMUsagePercentage()
    {
        var wmiObject = new ManagementObjectSearcher("SELECT * FROM Win32_OperatingSystem");

        var memoryValues = wmiObject.Get().Cast<ManagementObject>().Select(mo => new {
            FreePhysicalMemory = Double.Parse(mo["FreePhysicalMemory"].ToString()),
            TotalVisibleMemorySize = Double.Parse(mo["TotalVisibleMemorySize"].ToString())
        }).FirstOrDefault();

        if(memoryValues != null) 
        {
            var percent = ((memoryValues.TotalVisibleMemorySize - memoryValues.FreePhysicalMemory) / memoryValues.TotalVisibleMemorySize) * 100;
            return percent;
        }

        return 0;
    }

    public static string GetPercentageValueProgressBar(string title, double val)
    {
        char padChar  = ' ';
        char fillChar = '#';
        int  barWidth = 20;

        int repeatCount = ((int)(val - (val % 5))) / 5;
        string progressBar = string.Format("{0} [{1}] {2}%", title, new string(fillChar, repeatCount) + new string(padChar, barWidth - repeatCount), val.ToString("0.##"));
        
        return progressBar;
    }
}
