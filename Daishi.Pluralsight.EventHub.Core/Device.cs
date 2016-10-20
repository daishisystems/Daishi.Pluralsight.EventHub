using System;

namespace Daishi.Pluralsight.EventHub.Core
{
    public class Device
    {
        public Guid ID { get; set; }

        public DeviceStatus DeviceStatus { get; set; }
    }

    [Flags]
    public enum DeviceStatus
    {
        Unknown = 0,
        TurnedOff = 1,
        TurnedOn = 2,
        Asleep = 4
    }
}