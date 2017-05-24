using System;
using NServiceBus;

namespace Contracts
{
    public class OrderChocolate : ICommand
    {
        public OrderChocolate()
        {
            OrderId = Guid.NewGuid();
        }

        public string ChocolateType { get; set; }
        public Guid OrderId { get; }
    }
}