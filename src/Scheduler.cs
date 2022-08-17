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
    public ulong                IntervalMS      { get; set; }
    public DateTime             NextExecuteTime { get; set; }

    public ScheduleItem(string name, ulong intervalMS, Action<List<object>> func, List<object> args)
    {
        Name           = name;
        Function       = func;
        Args           = args;

        UpdateInterval(intervalMS);
    }

    public void UpdateInterval(ulong intervalMS=0)
    {
        if(intervalMS > 0) IntervalMS = intervalMS;
        NextExecuteTime = DateTime.Now.AddMilliseconds(IntervalMS);
    }
}

public static class Scheduler
    {
        private static Timer clock;
        private static List<ScheduleItem> scheduleItems = new List<ScheduleItem>();

        public static void Start(ulong intervalMS)
        {
            clock = new Timer();
            clock.Interval = intervalMS;
            clock.Elapsed += new ElapsedEventHandler(ClockElapsed);
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
