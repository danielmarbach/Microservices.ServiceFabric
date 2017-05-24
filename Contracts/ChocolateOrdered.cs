using System;
using NServiceBus;

namespace Contracts
{
    public class ChocolateOrdered : IEvent
    {
        public string ChocolateType { get; set; }
        public string OrderId { get; set; }
    }
}