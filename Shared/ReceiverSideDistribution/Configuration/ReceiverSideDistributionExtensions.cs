using NServiceBus;
using NServiceBus.Configuration.AdvanceExtensibility;
using NServiceBus.Features;

public static class ReceiverSideDistributionExtensions
{
    public static PartitionAwareReceiverSideDistributionConfiguration EnableReceiverSideDistribution(this RoutingSettings routingSettings, string[] discriminators)
    {
        var settings = routingSettings.GetSettings();

        PartitionAwareReceiverSideDistributionConfiguration config;
        if (!settings.TryGet(out config))
        {
            config = new PartitionAwareReceiverSideDistributionConfiguration(routingSettings, discriminators);
            settings.Set<PartitionAwareReceiverSideDistributionConfiguration>(config);
            settings.Set(typeof(ReceiverSideDistribution).FullName, FeatureState.Enabled);
        }

        return config;
    }
}