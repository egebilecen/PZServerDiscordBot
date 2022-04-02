using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

public class ScheduleItem
{
    public string               Name            { get; }
    public Action<List<object>> Function        { get; }
    public List<object>         Args            { get; set; }
    public ulong                IntervalMinute  { get; set; }
    public DateTime             NextExecuteTime { get; set; }

    public ScheduleItem(string name, ulong intervalMinute, Action<List<object>> func, List<object> args)
    {
        Name           = name;
        Function       = func;
        Args           = args;

        UpdateInterval(intervalMinute);
    }

    public void UpdateInterval(ulong intervalMinute=0)
    {
        if(intervalMinute > 0) IntervalMinute = intervalMinute;
        NextExecuteTime = DateTime.Now.AddMinutes(IntervalMinute);
    }
}

public static class Scheduler
{
    private static Timer clock;
    private static List<ScheduleItem> scheduleItems = new List<ScheduleItem>();

    public static void Start()
    {
        clock = new Timer();
        clock.Interval = 60 * 1000; // every minute
        clock.Elapsed += ClockElapsed;
        clock.Start();
    }

    public static void Stop()
    {
        clock.Elapsed -= ClockElapsed;
        clock.Stop();
    }

    public static void AddItem(ScheduleItem item)
    {
        scheduleItems.Add(item);
    }

    public static ScheduleItem GetItem(string name)
    {
        return scheduleItems.Find(item => item.Name == name);
    }

    private static void ClockElapsed(object sender, ElapsedEventArgs e)
    {
        DateTime now = DateTime.Now;

        foreach(ScheduleItem item in scheduleItems)
        {
            if(now >= item.NextExecuteTime)
            {
                item.Function(item.Args);
                item.UpdateInterval();
            }
        }
    }

    public static void Dispose()
    {
        if(clock != null)
            clock.Dispose();
    }
}
