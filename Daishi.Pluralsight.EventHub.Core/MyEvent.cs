using System;

namespace Daishi.Pluralsight.EventHub.Core
{
    /// <summary>
    ///     <see cref="MyEvent" /> is a simple, example event, containing metadata
    ///     originating from an imaginary up-stream web application.
    /// </summary>
    public class MyEvent
    {
        /// <summary>
        ///     <see cref="IPAddress" /> is the IPv4 address associated with an up-stream
        ///     HTTP request.
        /// </summary>
        public string IPAddress { get; set; }

        /// <summary>
        ///     <see cref="Time" /> is the local time upon which this instance was created.
        /// </summary>
        /// <remarks>Azure components favour time expressed in ISO-8601 format.</remarks>
        public DateTime Time { get; set; }

        /// <summary>
        ///     <see cref="Device" /> represents the physical device from which an
        ///     up-stream HTTP request originated.
        /// </summary>
        public Device Device { get; set; }
    }

    /// <summary>
    ///     <see cref="Device" /> refers to a physical device, such as a phone, or
    ///     tablet.
    /// </summary>
    [Flags]
    public enum Device
    {
        /// <summary>
        ///     <see cref="Unknown" /> represents a device whose type cannot be determined.
        /// </summary>
        Unknown = 0,

        /// <summary>
        ///     <see cref="PersonalComputer" /> represents any given PC.
        /// </summary>
        PersonalComputer = 1,

        /// <summary>
        ///     <see cref="Mac" /> represents any given Apple Mac.
        /// </summary>
        Mac = 2,

        /// <summary>
        ///     <see cref="Phone" /> represents a mobile phone.
        /// </summary>
        Phone = 4,

        /// <summary>
        ///     <see cref="Tablet" /> represents any given tablet device.
        /// </summary>
        Tablet = 8
    }
}