using System.Collections.Generic;
using System.Threading.Tasks;

public static partial class Schedules
{
    public static void BotVersionChecker(List<object> args)
    {
        _ = Task.Run(async () => await BotUtility.NotifyLatestBotVersion());
    }
}
