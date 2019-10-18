using Kronos.Framework.Schedule;
using System;
using System.Collections.Generic;

namespace Kronos.Service
{
    public class ScheduleProvider : IScheduleProvider
    {
        public ICollection<ScheduledBoolean> GetScheduledItems()
        {
            return new[]
            {
                new ScheduledBoolean("HotWater", "Turns Hotwater on and off", TimeSpan.FromHours(6), TimeSpan.FromHours(7)),
                new ScheduledBoolean("Heating", "Turns Heating on and off", TimeSpan.FromHours(6), TimeSpan.FromHours(12))
            };
        }
    }
}
