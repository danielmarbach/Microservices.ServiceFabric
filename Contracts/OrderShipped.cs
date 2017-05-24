using NServiceBus;

namespace Contracts
{
    public class OrderShipped : IEvent
    {
        public string OrderId { get; set; }
    }
}