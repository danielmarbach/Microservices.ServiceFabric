using System;
using System.Collections.Generic;
using System.Fabric;
using System.Fabric.Chaos.DataStructures;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.ServiceFabric.Services.Remoting.Client;
using OrderChocolate;

namespace Connector
{
    class Program
    {
        static void Main(string[] args)
        {
            Task.WaitAll(Main(), WithoutChaos());
        }
        
        static async Task Main()
        {
            Console.ForegroundColor = ConsoleColor.Green;
            long counter = 0;
            long previous = -1;

            // TODO 5
            var proxy = ServiceProxy.Create<IChocolateService>(new Uri("fabric:/ChocolateMicroservices/OrderChocolate"));
            while (true)
            {
                if (counter++ % 20 == 0)
                {
                    Console.Clear();
                }

                try
                {
                    // TODO 6
                    var result = await proxy.SayHello();
                    var current = result.InstanceId;
                    Console.WriteLine($"{current}: {result.Message}");
                    if (previous != -1 && previous != current)
                    {
                        var currentColor = Console.ForegroundColor;
                        Console.ForegroundColor = ConsoleColor.Yellow;
                        Console.WriteLine($"!! Failed over from {previous} to {current} !!");
                        Console.ForegroundColor = currentColor;
                    }
                    previous = current;
                }
                catch (Exception)
                {
                    var currentColor = Console.ForegroundColor;
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("!");
                    Console.ForegroundColor = currentColor;
                }
                await Task.Delay(TimeSpan.FromSeconds(1));
            }
        }

        #region NotImportant

        static Task WithoutChaos()
        {
            return Task.CompletedTask;
        }

        static async Task Chaos()
        {
            var clusterConnectionString = "localhost:19000";
            using (var client = new FabricClient(clusterConnectionString))
            {
                var startTimeUtc = DateTime.UtcNow;
                var stabilizationTimeout = TimeSpan.FromSeconds(10.00);
                var timeToRun = TimeSpan.FromMinutes(3);
                var maxConcurrentFaults = 3;

                var parameters = new ChaosParameters(
                    stabilizationTimeout,
                    maxConcurrentFaults,
                    true, /* EnableMoveReplicaFault */
                    timeToRun);

                try
                {
                    await client.TestManager.StartChaosAsync(parameters);
                }
                catch (FabricChaosAlreadyRunningException)
                {
                    await client.TestManager.StopChaosAsync();
                    await client.TestManager.StartChaosAsync(parameters);
                }

                var filter = new ChaosReportFilter(startTimeUtc, DateTime.MaxValue);

                var eventSet = new HashSet<ChaosEvent>(new ChaosEventComparer());

                while (true)
                {
                    var report = client.TestManager.GetChaosReportAsync(filter).GetAwaiter().GetResult();

                    foreach (var chaosEvent in report.History)
                    {
                        if (eventSet.Add(chaosEvent))
                        {
                            Console.WriteLine(chaosEvent);
                        }
                    }

                    // When Chaos stops, a StoppedEvent is created.
                    // If a StoppedEvent is found, exit the loop.
                    var lastEvent = report.History.LastOrDefault();

                    if (lastEvent is StoppedEvent)
                    {
                        break;
                    }

                    await Task.Delay(TimeSpan.FromSeconds(1.0));
                }
            }
        }

#endregion
    }
}
