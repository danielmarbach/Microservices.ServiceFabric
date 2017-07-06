using System;
using System.Fabric;
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

            var provider = services.BuildServiceProvider();
            var context = provider.GetService<StatelessServiceContext>();
            var configurationPackage = context.CodePackageActivationContext.GetConfigurationPackageObject("Config");

            var connectionString = configurationPackage.Settings.Sections["NServiceBus"].Parameters["ConnectionString"];

            var transport = endpointConfiguration.UseTransport<SqlServerTransport>();
            if (string.IsNullOrWhiteSpace(connectionString.Value))
            {
                throw new Exception("Could not read the 'NServiceBus.ConnectionString' environment variable. Check the sample prerequisites.");
            }
            transport.ConnectionString(connectionString.Value);
            //transport.UseForwardingTopology();

            // let's query the remote service
            // TODO 9
            var chocolateOrderPartitionInformation = ServicePartitionQueryHelper
                .QueryServicePartitions(new Uri("fabric:/Microservices.ServiceFabric/ChocolateOrder")).GetAwaiter().GetResult();

            var routing = transport.Routing();
            var distribution = routing.RegisterPartitionedDestinationEndpoint(
                destinationEndpoint: "chocolateorder",
                partitions: chocolateOrderPartitionInformation.Partitions);

            distribution.AddPartitionMappingForMessageType<OrderChocolate>(
                mapMessageToPartitionKey: message => message.ChocolateType);

            var endpointInstance = Endpoint.Start(endpointConfiguration).GetAwaiter().GetResult();
            services.AddSingleton<IMessageSession>(endpointInstance);
        }
    }
}