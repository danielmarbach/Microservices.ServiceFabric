using System.Linq;
using NServiceBus;
using NServiceBus.Configuration.AdvanceExtensibility;
using NServiceBus.Routing;
using NServiceBus.Transport;

public static class SenderSideDistributionExtensions
{
    public static PartitionAwareSenderSideDistributionConfiguration RegisterPartitionedDestinationEndpoint<T>(this RoutingSettings<T> routingSettings, string destinationEndpoint, string[] partitions) where T : TransportDefinition
    {
        var settings = routingSettings.GetSettings();

        var distributionConfiguration = new PartitionAwareSenderSideDistributionConfiguration(routingSettings, destinationEndpoint, partitions);

        var sendDistributionStrategy = new PartitionAwareDistributionStrategy(destinationEndpoint, distributionConfiguration.MapMessageToPartitionKey, DistributionStrategyScope.Send);
        var distributionPolicy = settings.GetOrCreate<DistributionPolicy>();
        distributionPolicy.SetDistributionStrategy(sendDistributionStrategy);

        var destinationEndpointInstances = partitions.Select(key => new EndpointInstance(destinationEndpoint, key)).ToList();

        var endpointInstances = settings.GetOrCreate<EndpointInstances>();
        endpointInstances.AddOrReplaceInstances(destinationEndpoint, destinationEndpointInstances);

        return distributionConfiguration;
    }
}