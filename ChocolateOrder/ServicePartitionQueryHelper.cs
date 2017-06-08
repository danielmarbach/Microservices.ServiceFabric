using System;
using System.Fabric;
using System.Linq;
using System.Threading.Tasks;

public static class ServicePartitionQueryHelper
{
    public static async Task<PartitionsInformation> QueryServicePartitions(Uri serviceName, Guid? partitionId = null)
    {
        using (var client = new FabricClient())
        {
            var servicePartitionList = await client.QueryManager.GetPartitionListAsync(serviceName)
                .ConfigureAwait(false);

            var partitionInformations =
                servicePartitionList.Select(x => new {
                    PartitionKey = x.PartitionInformation.Kind == ServicePartitionKind.Named
                        ? ((NamedPartitionInformation)x.PartitionInformation).Name
                        : ((Int64RangePartitionInformation)x.PartitionInformation).HighKey.ToString(),
                    PartitionId = x.PartitionInformation.Id
                }).ToList();

            var partitionInformation = new PartitionsInformation
            {
                LocalPartitionKey = partitionInformations.SingleOrDefault(p => p.PartitionId == partitionId)?.PartitionKey,
                Partitions = partitionInformations.Select(p => p.PartitionKey).ToArray()
            };

            Logger.Log($"{serviceName} under partition {partitionId} uses { partitionInformation.LocalPartitionKey }. Found partitions: {string.Join("; ", partitionInformation.Partitions)}");
            
            return partitionInformation;
        }
    }
}

public class PartitionsInformation
{
    public string LocalPartitionKey { get; set; }
    public string[] Partitions { get; set; }
}