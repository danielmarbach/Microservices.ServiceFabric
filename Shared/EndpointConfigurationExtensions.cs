using System;
using System.Fabric;
using Microsoft.ServiceFabric.Data;
using NServiceBus;
using NServiceBus.Persistence.ServiceFabric;

public static class EndpointConfigurationExtensions
{
    public static TransportExtensions ApplyCommonConfiguration(this EndpointConfiguration endpointConfiguration, IReliableStateManager stateManager, ServicePartitionInformation partitionInformation, StatefulServiceContext context)
    {
        var assemblyScanner = endpointConfiguration.AssemblyScanner();
        assemblyScanner.ExcludeAssemblies("netstandard");

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

        var configurationPackage = context.CodePackageActivationContext.GetConfigurationPackageObject("Config");

        var connectionString = configurationPackage.Settings.Sections["NServiceBus"].Parameters["ConnectionString"];

        var transport = endpointConfiguration.UseTransport<AzureServiceBusTransport>();
        if (string.IsNullOrWhiteSpace(connectionString.Value))
        {
            throw new Exception("Could not read the 'NServiceBus.ConnectionString'. Check the sample prerequisites.");
        }
        transport.ConnectionString(connectionString.Value);
        transport.UseForwardingTopology();

        return transport;
    }
}
