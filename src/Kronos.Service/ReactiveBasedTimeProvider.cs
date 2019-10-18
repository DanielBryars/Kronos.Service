using System;
using System.Reactive.Concurrency;

namespace Kronos.Service
{
    public class ReactiveBasedTimeProvider : ITimeProvider
    {
        private readonly IScheduler _scheduler;

        public ReactiveBasedTimeProvider(IScheduler scheduler)
        {
            _scheduler = scheduler;
        }

        public DateTimeOffset NowLocal()
        {
            return _scheduler.Now;
        }

        public DateTimeOffset NowUtc()
        {
            return _scheduler.Now.ToUniversalTime();
        }
    }
}
