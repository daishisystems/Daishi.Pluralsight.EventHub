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

        private static string GenerateRandomIPAddress(Random random)
        {
            return $"{random.Next(0, 255)}." +
                   $"{random.Next(0, 255)}." +
                   $"{random.Next(0, 255)}." +
                   $"{random.Next(0, 255)}";
        }

        private static void SendApplicationMetadata(EventHubClient eventHubClient,
            ApplicationMetadataEvent applicationMetadata)
        {
            var serialisedApplicationMetadata = JsonConvert.SerializeObject(applicationMetadata);

            var eventData = new EventData(Encoding.UTF8.GetBytes(serialisedApplicationMetadata));

            eventHubClient.Send(eventData);
        }

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