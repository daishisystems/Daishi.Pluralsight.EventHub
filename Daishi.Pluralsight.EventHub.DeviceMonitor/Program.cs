using System;
using Daishi.Pluralsight.EventHub.Core;

namespace Daishi.Pluralsight.EventHub.DeviceMonitor
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            var device = new Device
            {
                DeviceStatus = DeviceStatus.TurnedOn | DeviceStatus.Asleep
            };

            Console.WriteLine(device.DeviceStatus);

            var flag = DeviceStatus.TurnedOn;

            Console.WriteLine(device.DeviceStatus.HasFlag(flag));

            flag = DeviceStatus.TurnedOn | DeviceStatus.Asleep;

            Console.WriteLine(device.DeviceStatus.HasFlag(flag));

            flag = DeviceStatus.Unknown | DeviceStatus.TurnedOn;

            Console.WriteLine(device.DeviceStatus.HasFlag(flag));

            flag = DeviceStatus.Unknown;

            Console.WriteLine(device.DeviceStatus.HasFlag(flag));

            Console.ReadLine();
        }
    }
}