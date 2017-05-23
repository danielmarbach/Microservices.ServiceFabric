using System;
using System.Collections.Generic;
using NServiceBus;
using NServiceBus.Configuration.AdvanceExtensibility;

public class PartitionAwareReceiverSideDistributionConfiguration :
    ExposeSettings
{
    Dictionary<Type, Func<object, string>> messageTypeMappers = new Dictionary<Type, Func<object, string>>();

    public PartitionAwareReceiverSideDistributionConfiguration(RoutingSettings routingSettings, string[] partitions)
        : base(routingSettings.GetSettings())
    {
        Partitions = new HashSet<string>(partitions);
    }

    internal HashSet<string> Partitions { get; }

    public void AddPartitionMappingForMessageType<T>(Func<T, string> mapMessageToPartitionKey)
    {
        messageTypeMappers[typeof(T)] = message => mapMessageToPartitionKey((T)message);
    }

    internal string MapMessageToPartitionKey(object message)
    {
        var messageType = message.GetType();

        if (!messageTypeMappers.ContainsKey(messageType))
        {
            throw new Exception($"No partition mapping is found for message type '{messageType}'.");
        }

        var mapper = messageTypeMappers[messageType];

        var partition = mapper(message);

        if (Partitions.Contains(partition))
        {
            return partition;
        }
        throw new Exception($"Partition '{partition}' returned by partition mapping of '{messageType}' did not match any of the registered local partitions '{string.Join(",", Partitions)}'.");
    }
}