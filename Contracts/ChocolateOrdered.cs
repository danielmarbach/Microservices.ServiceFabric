using System;
using NServiceBus;

namespace Contracts
{
    public class ChocolateOrdered : IEvent
    {
        public string ChocolateType { get; set; }
        public Guid OrderId { get; set; }
    }
}