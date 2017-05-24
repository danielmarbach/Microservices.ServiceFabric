using System;
using NServiceBus;

namespace Contracts
{
    public class PaymentResponse : IMessage
    {
        public Guid OrderId { get; }
    }
}