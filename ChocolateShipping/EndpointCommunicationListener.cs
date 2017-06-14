using System;
using System.Fabric;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Contracts;
using Microsoft.ServiceFabric.Data;
using Microsoft.ServiceFabric.Services.Communication.Runtime;
using NServiceBus;

namespace ChocolateShipping
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

            endpointConfiguration = new EndpointConfiguration("chocolateshipping");

            var transport = endpointConfiguration.ApplyCommonConfiguration(stateManager, servicePartitionInformation, context);

            ConfigureLocalPartitionsChocolateShipping(endpointConfiguration, partitionInfo);
            ConfigureReceiverSideDistributionShipOrder(transport, partitionInfo);

            return null;
        }

        static void ConfigureLocalPartitionsChocolateShipping(EndpointConfiguration endpointConfiguration,
            PartitionsInformation partitionInfo)
        {
            endpointConfiguration.RegisterPartitionsForThisEndpoint(
                localPartitionKey: partitionInfo.LocalPartitionKey,
                allPartitionKeys: partitionInfo.Partitions);
        }

        static void ConfigureReceiverSideDistributionShipOrder(TransportExtensions transportConfig, PartitionsInformation partitionInfo)
        {
            var partitionKeysAsInts = partitionInfo.Partitions.Select(i => new { Key = i, AsInt = Convert.ToInt32(i) }).OrderBy(i => i.AsInt);

            var routing = transportConfig.Routing();
            var receiverSideDistribution = routing.EnableReceiverSideDistribution(partitionInfo.Partitions);
            receiverSideDistribution.AddPartitionMappingForMessageType<ShipOrder>(
                mapMessageToPartitionKey: shipOrder =>
                {
                    var zipCodeAsNumber = Convert.ToInt32(shipOrder.ZipCode);
                    int current = -1;
                    foreach (var keys in partitionKeysAsInts)
                    {
                        var next = keys.AsInt;
                        if (zipCodeAsNumber > current && zipCodeAsNumber <= next)
                        {
                            return keys.Key;
                        }

                        current = next;
                    }
                    throw new Exception($"Invalid zip code '{zipCodeAsNumber}' for message of type '{shipOrder.GetType()}'.");
                });
        }

        #region NotImportant
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
        #endregion
    }
}