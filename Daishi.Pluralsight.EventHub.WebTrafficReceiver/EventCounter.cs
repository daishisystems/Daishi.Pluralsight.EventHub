using System;
using System.Diagnostics;

namespace Daishi.Pluralsight.EventHub.WebTrafficReceiver
{
    public sealed class EventCounter
    {
        private static readonly Lazy<EventCounter> lazy =
            new Lazy<EventCounter>(() => new EventCounter());

        private volatile int _eventCount;

        private EventCounter()
        {
            Stopwatch = new Stopwatch();
        }

        public static EventCounter Instance => lazy.Value;

        public int EventCount {
            get { return _eventCount; }
            set { _eventCount = value; }
        }

        public Stopwatch Stopwatch { get; private set; }
    }
}