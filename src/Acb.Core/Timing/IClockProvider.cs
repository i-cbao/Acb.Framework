using System;

namespace Acb.Core.Timing
{
    public interface IClockProvider
    {
        DateTime Now { get; }
        DateTime Normalize(DateTime dateTime);
    }
}
