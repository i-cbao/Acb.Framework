using System;
using Acb.Core.Timing;

namespace Acb.Core.EventBus
{
    public class DEvent
    {
        public DEvent()
        {
            EventId = Guid.NewGuid();
            EventTime = Clock.Now;
        }

        public Guid EventId { get; }
        public DateTime EventTime { get; }
    }
}
