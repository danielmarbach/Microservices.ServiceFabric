using System;
using NServiceBus;

namespace Contracts
{
    public class OrderShipped : IEvent
    {
        public Guid OrderId { get; set; }
    }
}