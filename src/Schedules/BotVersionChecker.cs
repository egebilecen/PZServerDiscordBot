using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public static partial class Schedules
{
    public static void BotVersionChecker(List<object> args)
    {
        _ = Task.Run(async () => await BotUtility.CheckLatestBotVersion());
    }
}
