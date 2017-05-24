using System;
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

    public struct OrderId
    {
        private OrderId(Guid orderId, string chocolateType)
        {
            Id = orderId;
            ChocolateType = chocolateType;
        }

        public OrderId(string chocolateType)
        {
            Id = Guid.NewGuid();
            ChocolateType = chocolateType;
        }

        public Guid Id { get; }

        public string ChocolateType { get; }

        public static implicit operator string(OrderId id)
        {
            return $"{id.ChocolateType};{id.Id}";
        }

        public static implicit operator OrderId(string id)
        {
            var split = id.Split(new[] { ";" }, StringSplitOptions.RemoveEmptyEntries);
            return new OrderId(Guid.Parse(split[1]), split[0]);
        }
    }
}