using System;
using System.Collections.Generic;
using System.Fabric;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.ServiceFabric.Services.Communication.Runtime;
using Microsoft.ServiceFabric.Services.Runtime;
using Microsoft.ServiceFabric.Services.Remoting.Runtime;

namespace OrderChocolate
{
    // TODO 2
    internal sealed class OrderChocolate : StatelessService, IChocolateService
    {
        public OrderChocolate(StatelessServiceContext context)
            : base(context)
        { }

        // TODO 3
        protected override IEnumerable<ServiceInstanceListener> CreateServiceInstanceListeners()
        {
            return new[]{ new ServiceInstanceListener(this.CreateServiceRemotingListener) };
        }

        protected override async Task RunAsync(CancellationToken cancellationToken)
        {
            long iterations = 0;

            while (true)
            {
                cancellationToken.ThrowIfCancellationRequested();

                ServiceEventSource.Current.ServiceMessage(this.Context, $"Working-{Context.InstanceId}-{++iterations}");

                await Task.Delay(TimeSpan.FromSeconds(1), cancellationToken);
            }
        }

        // TODO 4
        public Task<Result> SayHello()
        {
            return Task.FromResult(new Result
            {
                InstanceId = Context.InstanceId,
                Message = $"Hello Warsaw! {this.Context.CodePackageActivationContext.CodePackageVersion}"
            });
        }
    }
}
