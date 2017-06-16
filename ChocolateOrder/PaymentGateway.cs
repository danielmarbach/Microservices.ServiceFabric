using System.Fabric;
using System.Threading.Tasks;
using Contracts;
using NServiceBus;

namespace ChocolateOrder
{
    public class PaymentGateway : IHandleMessages<MakePayment>
    {
        public NamedPartitionInformation PartitionInformation { get; set; }

        // TODO 6
        public async Task Handle(MakePayment message, IMessageHandlerContext context)
        {
            Logger.Log($"PaymentGateway: Paying {message.OrderId} with amount {message.Amount} and type {message.ChocolateType} on partition { PartitionInformation.Name }.");
            await Task.Delay(1000);
            Logger.Log($"PaymentGateway: Payed {message.OrderId} with amount {message.Amount} and type {message.ChocolateType} on partition { PartitionInformation.Name }.");
            await context.Reply(new PaymentResponse());
        }
    }
}