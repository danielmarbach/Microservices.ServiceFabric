using System;
using System.Fabric;
using System.Threading.Tasks;
using Contracts;
using NServiceBus;
using NServiceBus.Persistence.ServiceFabric;

namespace ChocolateOrder
{
    [ServiceFabricSaga(CollectionName = "orders")]
    public class OrderProcessManager : Saga<OrderProcessManager.OrderProcessData>
        , IAmStartedByMessages<OrderChocolate>,
        IHandleMessages<PaymentResponse>,
        IHandleMessages<OrderShipped>,
        IHandleTimeouts<OrderProcessManager.BuyersRemorsePeriodOver>
    {
        static Random random = new Random();
        

        protected override void ConfigureHowToFindSaga(SagaPropertyMapper<OrderProcessData> mapper)
        {
            mapper.ConfigureMapping<OrderChocolate>(m => m.OrderId).ToSaga(s => s.OrderId);
            mapper.ConfigureMapping<OrderShipped>(m => m.OrderId).ToSaga(s => s.OrderId);
        }

        public NamedPartitionInformation PartitionInformation { get; set; }

        public Task Handle(OrderChocolate message, IMessageHandlerContext context)
        {
            Data.ChocolateType = message.ChocolateType;

            Logger.Log($"OrderProcessManager: Order {Data.OrderId} and type {Data.ChocolateType} on partition { PartitionInformation.Name } removerse period started.");

            return RequestTimeout<BuyersRemorsePeriodOver>(context, DateTime.UtcNow.AddSeconds(1));
        }

        public async Task Timeout(BuyersRemorsePeriodOver state, IMessageHandlerContext context)
        {
            Logger.Log($"OrderProcessManager: Order {Data.OrderId} and type {Data.ChocolateType} on partition { PartitionInformation.Name } buyers remorse period over.");

            await context.Publish(new ChocolateOrdered { ChocolateType = Data.ChocolateType, OrderId = Data.OrderId });
            await context.SendLocal(new MakePayment { ChocolateType = Data.ChocolateType, OrderId = Data.OrderId, Amount = random.Next(1, 100) * new decimal(2.25)});
        }

        public Task Handle(PaymentResponse message, IMessageHandlerContext context)
        {
            Logger.Log($"OrderProcessManager: Order {Data.OrderId} and type {Data.ChocolateType} on partition { PartitionInformation.Name } was payed.");

            var zipcode = random.Next(0, 99001).ToString("d5");
            return context.Send(new ShipOrder { OrderId = Data.OrderId, ZipCode = zipcode });
        }

        public Task Handle(OrderShipped message, IMessageHandlerContext context)
        {
            Logger.Log($"OrderProcessManager: Order {Data.OrderId} and type {Data.ChocolateType} on partition { PartitionInformation.Name } done.");

            MarkAsComplete();
            return Task.CompletedTask;
        }

        public class OrderProcessData :
            ContainSagaData
        {
            public string OrderId { get; set; }
            public string ChocolateType { get; set; }
        }

        public class BuyersRemorsePeriodOver
        {
        }
    }
}