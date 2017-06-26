# Preparations

- Start local dev cluster
- Start VS under Admin mode ChaosTestApplication
  - Build Release and Deploy to the local cluster
- Start another VS under Admin mode
- System wide Environment variable called `AzureServiceBus.ConnectionString` with a valid ASB connection string.

# Demo 1

- Create new ChocolateMicroservices application
- Add one stateless microservice called OrderChocolate
- Hit F5 and switch to manage cluster
  - By default Local.5Node.xml has instance count set to 1
  - Show cluster manager and how one instance is deployed on one node
  - Show Diagnostics windows and how the service is working
- Stop debugging
- Change Local.5Node.xml to instance count -1
- Hit F5 and 
  - Show cluster manager and how one instance is deployed on all available nodes
  - Show Diagnostics windows and how the service instances are working
- Switch to http://localhost:8081/chaostest/ and let the chaos test run, show dashboard with update refresh count fast how service heals itself

Nuget packages

```
Microsoft.ServiceFabric.FabricTransport
Microsoft.ServiceFabric.Services.Remoting
```

```
[DataContract]
public class Result
{
    [DataMember]
    public string Message { get; set; }

    [DataMember]
    public long InstanceId { get; set; }
}
public interface IChocolateService : IService
{
    Task<Result> SayHello();
}
```

```
using System;
using System.Threading.Tasks;
using Contracts;
using Microsoft.ServiceFabric.Services.Remoting.Client;

namespace Connector
{
    class Program
    {
        static void Main(string[] args)
        {
            MainAsync().GetAwaiter().GetResult();
        }

        static async Task MainAsync()
        {
            long counter = 0;
            long previous = -1;
            var proxy = ServiceProxy.Create<IChocolateService>(new Uri("fabric:/ChocolateMicroservices/ChocolateService"));
            while (true)
            {
                if (counter++ % 10 == 0)
                {
                    Console.Clear();
                }

                try
                {
                    var result = await proxy.SayHello();
                    Console.WriteLine(result.Message);
                    var current = result.InstanceId;
                    if (previous != -1 && previous != current)
                    {
                        Console.WriteLine($"!! Failed over from {previous} to {current} !!");
                    }
                    previous = current;
                }
                catch (Exception)
                {
                    Console.WriteLine("!");
                }
                await Task.Delay(TimeSpan.FromSeconds(1));
            }
        }
    }
}
```

# Demo 2

- Open up View>Other Windows>Diagnostic Event
- Open up http://localhost:8147/

## With rabbit

Install NServiceBus.RabbitMQ in all proj except Contracts and Application.

Change:

- Shared `ApplyCommonConfiguration`
- FrontEnd `AddNServiceBus`

```
        var connectionString = "host=localhost";
        transport.ConnectionString(connectionString);
        var delayedDelivery = transport.DelayedDelivery();
        delayedDelivery.DisableTimeoutManager();
```

http://localhost:15672/