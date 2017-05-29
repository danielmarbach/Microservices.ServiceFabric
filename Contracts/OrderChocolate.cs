using NServiceBus;

namespace Contracts
{
    public class OrderChocolate : ICommand
    {
        public string ChocolateType { get; set; }
        public string OrderId { get; set; }
    }
}