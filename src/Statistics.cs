using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Management;

namespace PZServerDiscordBot
{
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
    }
}
