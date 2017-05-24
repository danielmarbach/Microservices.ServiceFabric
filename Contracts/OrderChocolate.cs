using NServiceBus;

namespace Contracts
{
    public class OrderChocolate : ICommand
    {
        public string ChocolateType { get; set; }
    }
}