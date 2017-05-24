using System;
using NServiceBus;

namespace Contracts
{
    public class MakePayment : ICommand
    {
        public string ChocolateType { get; set; }
        public decimal Amount { get; set; }
        public string OrderId { get; set; }
    }
}