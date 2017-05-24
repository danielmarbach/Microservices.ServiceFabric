using System;
using Contracts;
using Microsoft.Extensions.DependencyInjection;
using NServiceBus;

namespace ChocolateOrder.Front
{
    public static class NServiceBusServiceCollectionExtensions
    {
        public static void AddNServiceBus(this IServiceCollection services)
        {
            var endpointConfiguration = new EndpointConfiguration("chocolateorder.front");

            endpointConfiguration.AuditProcessedMessagesTo("audit");
            endpointConfiguration.UseSerialization<JsonSerializer>();
            endpointConfiguration.EnableInstallers();
            endpointConfiguration.UsePersistence<InMemoryPersistence>();
            endpointConfiguration.SendOnly();

            var transport = endpointConfiguration.UseTransport<AzureServiceBusTransport>();
            var connectionString = Environment.GetEnvironmentVariable("AzureServiceBus.ConnectionString");
            if (string.IsNullOrWhiteSpace(connectionString))
            {
                throw new Exception("Could not read the 'AzureServiceBus.ConnectionString' environment variable. Check the sample prerequisites.");
            }
            transport.ConnectionString(connectionString);
            transport.UseForwardingTopology();

            var remotePartitions = new[] { "Dark", "Brown", "White" };

            var routing = transport.Routing();
            var distribution = routing.RegisterPartitionedDestinationEndpoint(
                destinationEndpoint: "chocolateorder",
                partitions: remotePartitions);

            distribution.AddPartitionMappingForMessageType<OrderChocolate>(
                mapMessageToPartitionKey: message =>
                {
                    return message.ChocolateType;
                });

            var endpointInstance = Endpoint.Start(endpointConfiguration).GetAwaiter().GetResult();
            services.AddSingleton<IMessageSession>(endpointInstance);
        }
    }
}