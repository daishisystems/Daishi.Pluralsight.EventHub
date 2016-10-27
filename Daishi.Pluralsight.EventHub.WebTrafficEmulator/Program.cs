using System;
using System.Collections.Generic;
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
        private static List<string> _partitionKeys;

        private static void Main(string[] args)
        {
            Console.WriteLine("Press Ctrl-C to stop the sender process");
            Console.WriteLine("Press Enter to start now");
            Console.ReadLine();

            _partitionKeys = new List<string>
            {
                "17DF2B60-2F85-43D3-9476-9A66F2506406",
                "B9D8A345-36B5-4181-AAE4-29794ADEA4A0",
                "2D5FF03A-E730-4476-8429-DC7F9546A560",
                "123C8798-1E5E-4FDD-BCC7-8032E7A06739"
            };

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
        /// <param name="partitionKey">
        ///     <see cref="partitionKey" /> is the Event Hub Partition Key that determines
        ///     the Event Hub Partition to which
        ///     <see cref="applicationMetadataEvent" /> will be sent.
        /// </param>
        private static void SendApplicationMetadata(EventHubClient eventHubClient,
            ApplicationMetadataEvent applicationMetadataEvent, string partitionKey)
        {
            var serialisedApplicationMetadata = JsonConvert.SerializeObject(applicationMetadataEvent);

            var eventData = new EventData(Encoding.UTF8.GetBytes(serialisedApplicationMetadata))
            {
                PartitionKey = partitionKey
            };

            eventHubClient.Send(eventData);
        }

        /// <summary>
        ///     <see cref="SendRandomApplicationMetadataEvents" /> sends randomly-generated
        ///     <see cref="ApplicationMetadataEvent" /> instances to Event Hub.
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

            const int eventCount = 25;

            foreach (var partitionKey in _partitionKeys)
            {
                for (var j = 0; j < eventCount; j++)
                {
                    try
                    {
                        var applicationMetadata = new ApplicationMetadataEvent
                        {
                            IPAddress = GenerateRandomIPAddress(random),
                            Time = DateTime.UtcNow,
                            Device = (Device) values.GetValue(random.Next(1, values.Length))
                        };

                        SendApplicationMetadata(eventHubClient, applicationMetadata, partitionKey);
                    }
                    catch (Exception exception)
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("{0} > Exception: {1}", DateTime.Now, exception.Message);
                        Console.ResetColor();
                    }

                    Task.Delay(10);
                }

                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("Sent {0} events to {1}.", eventCount, partitionKey);
                Console.ResetColor();
            }

            Console.ReadLine();
        }
    }
}