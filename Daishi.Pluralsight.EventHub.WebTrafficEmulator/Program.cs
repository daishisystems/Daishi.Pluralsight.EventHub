using System;
using System.Text;
using System.Threading;
using Microsoft.ServiceBus.Messaging;

namespace Daishi.Pluralsight.EventHub.WebTrafficEmulator
{
    internal static class Program
    {
        private const string EventHubName = "{Event Hub name}";
        private const string ConnectionString = "{send connection string}";

        private static void Main(string[] args)
        {
            Console.WriteLine("Press Ctrl-C to stop the sender process");
            Console.WriteLine("Press Enter to start now");
            Console.ReadLine();
            SendingRandomMessages();
        }

        private static void SendingRandomMessages()
        {
            var eventHubClient = EventHubClient.CreateFromConnectionString(ConnectionString, EventHubName);
            while (true)
            {
                try
                {
                    var message = Guid.NewGuid().ToString();
                    Console.WriteLine("{0} > Sending message: {1}", DateTime.Now, message);
                    eventHubClient.Send(new EventData(Encoding.UTF8.GetBytes(message)));
                }
                catch (Exception exception)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("{0} > Exception: {1}", DateTime.Now, exception.Message);
                    Console.ResetColor();
                }

                Thread.Sleep(200);
            }
        }
    }
}