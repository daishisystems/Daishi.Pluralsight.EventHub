using System;

namespace Daishi.Pluralsight.EventHub.Core
{
    /// <summary>
    ///     <see cref="ApplicationMetadataEvent" /> is an event that contains metadata
    ///     originating from an imaginary up-stream web application.
    /// </summary>
    public class ApplicationMetadataEvent
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

        /// <summary>Returns a string that represents the current object.</summary>
        /// <returns>A string that represents the current object.</returns>
        public override string ToString()
        {
            return $"{Device}: {IPAddress}";
        }
    }
}