using System;

namespace Kronos.Service
{
    public interface ITimeProvider
    {
        DateTimeOffset NowLocal();
        DateTimeOffset NowUtc();
    }
}