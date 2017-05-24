using System;
using System.Threading.Tasks;
using Contracts;
using NServiceBus;

namespace ChocolateOrder
{
    public class OrderProcessManager : Saga<OrderProcessManager.OrderProcessData>
        , IAmStartedByMessages<OrderChocolate>,
        IHandleTimeouts<OrderProcessManager.BuyersRemorsePeriodOver>
    {
        protected override void ConfigureHowToFindSaga(SagaPropertyMapper<OrderProcessData> mapper)
        {
            mapper.ConfigureMapping<OrderChocolate>(m => m.OrderId)
                .ToSaga(s => s.OrderId);
        }

        public Task Handle(OrderChocolate message, IMessageHandlerContext context)
        {
            Logger.Log($"##### OrderProcessManager saga for {Data.OrderId} removerse period started.");

            return RequestTimeout<BuyersRemorsePeriodOver>(context, DateTime.UtcNow.AddSeconds(5));
        }

        public Task Timeout(BuyersRemorsePeriodOver state, IMessageHandlerContext context)
        {
            Logger.Log($"##### OrderProcessManager saga for {Data.OrderId} buyers remorse period over.");

            return Task.CompletedTask;
        }

        public class OrderProcessData :
            ContainSagaData
        {
            public Guid OrderId { get; set; }
        }

        public class BuyersRemorsePeriodOver
        {
        }
    }
}