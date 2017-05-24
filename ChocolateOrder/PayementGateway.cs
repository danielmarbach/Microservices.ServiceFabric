using System.Fabric;
using System.Threading.Tasks;
using Contracts;
using NServiceBus;

namespace ChocolateOrder
{
    public class PayementGateway : IHandleMessages<MakePayment>
    {
        public NamedPartitionInformation PartitionInformation { get; set; }

        public async Task Handle(MakePayment message, IMessageHandlerContext context)
        {
            Logger.Log($"PayementGateway: Paying {message.OrderId} with amount {message.Amount} and type {message.ChocolateType} on partition { PartitionInformation.Name }.");
            await Task.Delay(1000);
            Logger.Log($"PayementGateway: Payed {message.OrderId} with amount {message.Amount} and type {message.ChocolateType} on partition { PartitionInformation.Name }.");
            await context.Reply(new PayementResponse());
        }
    }
}