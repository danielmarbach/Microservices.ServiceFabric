using System;
using System.Fabric;
using System.Threading;
using System.Threading.Tasks;
using Contracts;
using Microsoft.ServiceFabric.Data;
using Microsoft.ServiceFabric.Services.Communication.Runtime;
using NServiceBus;

namespace ChocolateOrder
{
    public class EndpointCommunicationListener :
        ICommunicationListener
    {
        StatefulServiceContext context;
        IReliableStateManager stateManager;
        IEndpointInstance endpointInstance;
        EndpointConfiguration endpointConfiguration;
        private ServicePartitionInformation servicePartitionInformation;

        public EndpointCommunicationListener(StatefulServiceContext context, IReliableStateManager stateManager, ServicePartitionInformation partitionPartitionInfo)
        {
            servicePartitionInformation = partitionPartitionInfo;
            this.context = context;
            this.stateManager = stateManager;
        }

        public async Task<string> OpenAsync(CancellationToken cancellationToken)
        {
            Logger.Log = m => ServiceEventSource.Current.ServiceMessage(context, m);

            var partitionInfo = await ServicePartitionQueryHelper
                .QueryServicePartitions(context.ServiceName, context.PartitionId)
                .ConfigureAwait(false);

            endpointConfiguration = new EndpointConfiguration("chocolateorder");

            var transport = endpointConfiguration.ApplyCommonConfiguration(stateManager, servicePartitionInformation);

            var routing = transport.Routing();
            routing.RouteToEndpoint(typeof(ShipOrder), "chocolateshipping");

            ConfigureLocalPartitionsChocolateOrder(endpointConfiguration, partitionInfo);
            ConfigureReceiverSideDistributionOrderShipped(transport, partitionInfo);

            return null;
        }

        static void ConfigureLocalPartitionsChocolateOrder(EndpointConfiguration endpointConfiguration,
            PartitionsInformation partitionInfo)
        {
            endpointConfiguration.RegisterPartitionsForThisEndpoint(
                localPartitionKey: partitionInfo.LocalPartitionKey,
                allPartitionKeys: partitionInfo.Partitions);
        }

        static void ConfigureReceiverSideDistributionOrderShipped(TransportExtensions<AzureServiceBusTransport> transportConfig, PartitionsInformation partitionInfo)
        {
            var routing = transportConfig.Routing();
            var receiverSideDistribution = routing.EnableReceiverSideDistribution(partitionInfo.Partitions);
            receiverSideDistribution.AddPartitionMappingForMessageType<OrderShipped>(
                mapMessageToPartitionKey: orderShipped =>
                {
                    OrderId orderId = orderShipped.OrderId;
                    return orderId.ChocolateType;
                });
        }

        public async Task Run()
        {
            if (endpointConfiguration == null)
            {
                var message =
                    $"{nameof(EndpointCommunicationListener)} Run() method should be invoked after communication listener has been opened and not before.";

                Logger.Log(message);
                throw new Exception(message);
            }

            endpointInstance = await Endpoint.Start(endpointConfiguration)
                .ConfigureAwait(false);
        }

        public Task CloseAsync(CancellationToken cancellationToken)
        {
            return endpointInstance.Stop();
        }

        public void Abort()
        {
            // Fire & Forget Close
            CloseAsync(CancellationToken.None);
        }
    }
}