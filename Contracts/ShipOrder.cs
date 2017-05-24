using System;
using NServiceBus;

namespace Contracts
{
    public class ShipOrder : ICommand
    {
        public string OrderId { get; set; }

        public string ZipCode { get; set; }
    }
}