using System;

public static class ScheduleUtility
{
    public static void CheckServerRestartSchedule()
    {
        var serverRestartSchedule = Scheduler.GetItem("ServerRestart");

        if(serverRestartSchedule != null)
        {
            if(Application.BotSettings.ServerScheduleSettings.ServerRestartScheduleTimes.Count > 0)
            {
                ulong intervalMS = 30 * 1000;

                if(serverRestartSchedule.IntervalMS != intervalMS)
                    serverRestartSchedule.UpdateInterval(intervalMS);
            }
            else ServerUtility.ResetServerRestartInterval();
        }
    }
}
