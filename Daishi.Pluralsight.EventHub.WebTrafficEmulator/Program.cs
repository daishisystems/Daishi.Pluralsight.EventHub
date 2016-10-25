using System;
using System.Configuration;
using System.Text;
using System.Threading.Tasks;
using Daishi.Pluralsight.EventHub.Core;
using Microsoft.ServiceBus.Messaging;
using Newtonsoft.Json;

namespace Daishi.Pluralsight.EventHub.WebTrafficEmulator
{
    internal static class Program
    {
        private static void Main(string[] args)
        {
            Console.WriteLine("Press Ctrl-C to stop the sender process");
            Console.WriteLine("Press Enter to start now");
            Console.ReadLine();
            SendRandomApplicationMetadataEvents();
        }

        /// <summary>
        ///     <see cref="GenerateRandomIPAddress" /> returns a randomly-generated IP
        ///     address in string-format.
        /// </summary>
        /// <param name="random"></param>
        /// <returns>A randomly-generated IP address in string-format.</returns>
        private static string GenerateRandomIPAddress(Random random)
        {
            return $"{random.Next(0, 255)}." +
                   $"{random.Next(0, 255)}." +
                   $"{random.Next(0, 255)}." +
                   $"{random.Next(0, 255)}";
        }

        /// <summary>
        ///     <see cref="SendApplicationMetadata" /> sends
        ///     <see cref="applicationMetadataEvent" /> to the Event Hub instance
        ///     associated with <see cref="eventHubClient" />.
        /// </summary>
        /// <param name="eventHubClient">
        ///     <see cref="eventHubClient" /> is the Event Hub client proxy.
        /// </param>
        /// <param name="applicationMetadataEvent">
        ///     <see cref="ApplicationMetadataEvent" /> is an event that contains metadata
        ///     originating from an imaginary up-stream web application.
        /// </param>
        private static void SendApplicationMetadata(EventHubClient eventHubClient,
            ApplicationMetadataEvent applicationMetadataEvent)
        {
            var serialisedApplicationMetadata = JsonConvert.SerializeObject(applicationMetadataEvent);

            var eventData = new EventData(Encoding.UTF8.GetBytes(serialisedApplicationMetadata));

            eventHubClient.Send(eventData);
        }

        /// <summary>
        ///     <see cref="SendRandomApplicationMetadataEvents" /> sends randomly-generated
        ///     <see cref="ApplicationMetadataEvent" /> instances to Event Hub until the
        ///     process is terminated, or a specified number of <see cref="Exception" />s
        ///     occur.
        /// </summary>
        private static void SendRandomApplicationMetadataEvents()
        {
            EventHubClient eventHubClient;

            try
            {
                var eventHubName = ConfigurationManager.AppSettings["EventHubName"];
                var eventHubConnectionString = ConfigurationManager.AppSettings["EventHubConnectionString"];

                eventHubClient = EventHubClient
                    .CreateFromConnectionString(eventHubConnectionString, eventHubName);
            }
            catch (Exception exception)
            {
                Console.WriteLine("A problem occurred connecting to Event Hub: " + exception);
                return;
            }

            var random = new Random();
            var values = Enum.GetValues(typeof (Device));

            var exceptionCount = 0;

            while (exceptionCount < 10)
            {
                try
                {
                    var applicationMetadata = new ApplicationMetadataEvent
                    {
                        IPAddress = GenerateRandomIPAddress(random),
                        Time = DateTime.UtcNow,
                        Device = (Device) values.GetValue(random.Next(values.Length))
                    };

                    SendApplicationMetadata(eventHubClient, applicationMetadata);

                    Console.WriteLine("Sent metadata pertaining to IP "
                                      + applicationMetadata.IPAddress + "!");
                }
                catch (Exception exception)
                {
                    exceptionCount++;
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("{0} > Exception: {1}", DateTime.Now, exception.Message);
                    Console.ResetColor();
                }

                Task.Delay(100);
            }
        }
    }
}