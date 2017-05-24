using System;
using NServiceBus;

namespace Contracts
{
    public class ShipOrder : ICommand
    {
        public Guid OrderId { get; set; }
    }
}