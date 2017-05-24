using System.Fabric;
using System.Threading.Tasks;
using Contracts;
using NServiceBus;

namespace ChocolateShipping
{
    public class ShipOrderHandler : IHandleMessages<ShipOrder>
    {
        public Int64RangePartitionInformation PartitionInformation { get; set; }

        public Task Handle(ShipOrder message, IMessageHandlerContext context)
        {
            Logger.Log($"ShipOrderHandler: Shipping {message.OrderId} on partition [{ PartitionInformation.LowKey}, { PartitionInformation.HighKey }].");
            return context.Publish(new OrderShipped { OrderId = message.OrderId });
        }
    }
}