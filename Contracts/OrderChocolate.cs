using NServiceBus;

namespace Contracts
{
    public class OrderChocolate : ICommand
    {
        public OrderChocolate(string chocolateType)
        {
            OrderId = new OrderId(chocolateType);
            ChocolateType = chocolateType;
        }

        public string ChocolateType { get; }
        public string OrderId { get; }
    }
}