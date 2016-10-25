using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;
using Daishi.Pluralsight.EventHub.Core;
using Microsoft.ServiceBus.Messaging;
using Newtonsoft.Json;

namespace Daishi.Pluralsight.EventHub.WebTrafficReceiver
{
    internal class WebTrafficEventProcessor : IEventProcessor
    {
        private Stopwatch _checkpointStopWatch;

        async Task IEventProcessor.CloseAsync(PartitionContext context, CloseReason reason)
        {
            Console.WriteLine("Processor Shutting Down. Partition '{0}', Reason: '{1}'.",
                context.Lease.PartitionId, reason);
            if (reason == CloseReason.Shutdown)
            {
                await context.CheckpointAsync();
            }
            Console.ReadLine();
        }

        Task IEventProcessor.OpenAsync(PartitionContext context)
        {
            Console.WriteLine("SimpleEventProcessor initialized.  Partition: '{0}', Offset: '{1}'",
                context.Lease.PartitionId, context.Lease.Offset);
            _checkpointStopWatch = new Stopwatch();
            _checkpointStopWatch.Start();

            EventCounter.Instance.Stopwatch.Start();
            return Task.FromResult<object>(null);
        }

        async Task IEventProcessor.ProcessEventsAsync(PartitionContext context,
            IEnumerable<EventData> messages)
        {
            foreach (var eventData in messages)
            {
                var data = Encoding.UTF8.GetString(eventData.GetBytes());

                var applicationMetadataEvent =
                    JsonConvert.DeserializeObject<ApplicationMetadataEvent>(data);

                EventCounter.Instance.EventCount++;

                Console.WriteLine("Message received. Partition: '{0}', Event: '{1}'",
                    context.Lease.PartitionId, applicationMetadataEvent);
            }

            if (EventCounter.Instance.EventCount.Equals(100))
            {
                EventCounter.Instance.Stopwatch.Stop();

                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("The process took {0} ms to complete.",
                    EventCounter.Instance.Stopwatch.ElapsedMilliseconds);
                Console.ResetColor();
            }

            //Call checkpoint every 5 minutes, so that worker can resume processing from 5 minutes back if it restarts.
            if (_checkpointStopWatch.Elapsed > TimeSpan.FromMinutes(5))
            {
                await context.CheckpointAsync();
                _checkpointStopWatch.Restart();
            }
        }
    }

}