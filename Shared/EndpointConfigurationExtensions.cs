using System;
using System.Fabric;
using Microsoft.ServiceFabric.Data;
using NServiceBus;
using NServiceBus.Persistence.ServiceFabric;

public static class EndpointConfigurationExtensions
{
    public static TransportExtensions<AzureServiceBusTransport> ApplyCommonConfiguration(this EndpointConfiguration endpointConfiguration, IReliableStateManager stateManager, ServicePartitionInformation partitionInformation)
    {
        endpointConfiguration.SendFailedMessagesTo("error");
        endpointConfiguration.AuditProcessedMessagesTo("audit");
        endpointConfiguration.UseSerialization<JsonSerializer>();
        endpointConfiguration.EnableInstallers();

        var instance = partitionInformation as NamedPartitionInformation;
        if (instance != null)
        {
            endpointConfiguration.RegisterComponents(c => c.RegisterSingleton(instance));
        }

        var information = partitionInformation as Int64RangePartitionInformation;
        if (information != null)
        {
            endpointConfiguration.RegisterComponents(c => c.RegisterSingleton(information));
        }

        var persistence = endpointConfiguration.UsePersistence<ServiceFabricPersistence>();
        persistence.StateManager(stateManager);

        var recoverability = endpointConfiguration.Recoverability();
        recoverability.DisableLegacyRetriesSatellite();
        // for demo purposes
        recoverability.Immediate(d => d.NumberOfRetries(0));
        recoverability.Delayed(d => d.NumberOfRetries(0));

        var transport = endpointConfiguration.UseTransport<AzureServiceBusTransport>();
        var connectionString = Environment.GetEnvironmentVariable("AzureServiceBus.ConnectionString");
        if (string.IsNullOrWhiteSpace(connectionString))
        {
            throw new Exception("Could not read the 'AzureServiceBus.ConnectionString' environment variable. Check the sample prerequisites.");
        }
        transport.ConnectionString(connectionString);
        transport.UseForwardingTopology();

        return transport;
    }
}
