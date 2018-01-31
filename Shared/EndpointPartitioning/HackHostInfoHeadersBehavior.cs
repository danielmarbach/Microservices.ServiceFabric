using System;
using System.Threading.Tasks;
using NServiceBus;
using NServiceBus.Pipeline;

class HackHostInfoHeadersBehavior : IBehavior<IOutgoingLogicalMessageContext, IOutgoingLogicalMessageContext>
{
    public HackHostInfoHeadersBehavior(string endpoint)
    {
        this.endpoint = endpoint;
    }

    public Task Invoke(IOutgoingLogicalMessageContext context, Func<IOutgoingLogicalMessageContext, Task> next)
    {
        context.Headers[Headers.OriginatingEndpoint] = endpoint;

        return next(context);
    }

    readonly string endpoint;

    internal class Registration : RegisterStep
    {
        public Registration(string endpointName) : base((string)"HackHostInfoHeaders", typeof(HackHostInfoHeadersBehavior),
            (string)"Hacks the endpoint name", b => new HackHostInfoHeadersBehavior(endpointName))
        {
            InsertAfterIfExists("AddHostInfoHeaders");
        }
    }
}