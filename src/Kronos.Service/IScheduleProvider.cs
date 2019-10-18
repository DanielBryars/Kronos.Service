using System.Collections.Generic;
using Kronos.Framework.Schedule;

namespace Kronos.Service
{
    public interface IScheduleProvider
    {
        ICollection<ScheduledBoolean> GetScheduledItems();
    }
}