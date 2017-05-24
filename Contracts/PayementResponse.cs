using System;
using NServiceBus;

namespace Contracts
{
    public class PayementResponse : IMessage
    {
        public Guid OrderId { get; }
    }
}