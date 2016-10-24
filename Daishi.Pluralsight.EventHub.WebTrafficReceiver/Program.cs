using System;
using Microsoft.ServiceBus.Messaging;

namespace Daishi.Pluralsight.EventHub.WebTrafficReceiver
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            try
            {
                const string eventHubConnectionString =
                    "Endpoint=sb://shield.servicebus.windows.net/;SharedAccessKeyName=Admin;SharedAccessKey=8FyaeOiU8q8TxMtQ8ZRbBEaA/2so9lm2TSMRo7oYGaU=";
                const string eventHubName = "telemetry";
                const string storageAccountName = "aegisanalytics";
                const string storageAccountKey =
                    "vXKCLoUsia4HMHBSf1Jx4YzbKMkbk9Hex5kCVgTqD0thQHKdj6uNQsBzHpPp4uOsJ36b/6YYUXAMn/qVv/gOjA==";
                var storageConnectionString =
                    $"DefaultEndpointsProtocol=https;AccountName={storageAccountName};AccountKey={storageAccountKey}";

                var eventProcessorHostName = Guid.NewGuid().ToString();
                var eventProcessorHost = new EventProcessorHost(eventProcessorHostName, eventHubName,
                    EventHubConsumerGroup.DefaultGroupName, eventHubConnectionString, storageConnectionString);
                Console.WriteLine("Registering EventProcessor...");
                var options = new EventProcessorOptions();
                options.ExceptionReceived += (sender, e) => { Console.WriteLine(e.Exception); };
                eventProcessorHost.RegisterEventProcessorAsync<WebTrafficEventProcessor>(options).Wait();

                Console.WriteLine("Receiving. Press enter key to stop worker.");
                Console.ReadLine();
                eventProcessorHost.UnregisterEventProcessorAsync().Wait();
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
                Console.ReadLine();
            }
        }
    }
}