using System;
using System.Threading.Tasks;
using NServiceBus;
using NServiceBus.Pipeline;

class HackEndpointNameBehavior : IBehavior<IAuditContext, IAuditContext>
{
    public HackEndpointNameBehavior(string endpoint)
    {
        this.endpoint = endpoint;
    }

    public Task Invoke(IAuditContext context, Func<IAuditContext, Task> next)
    {
        context.AddAuditData(Headers.ProcessingEndpoint, endpoint);

        return next(context);
    }

    readonly string endpoint;

    internal class Registration : RegisterStep
    {
        public Registration(string endpointName) : base((string)"HackEndpointName", typeof(HackEndpointNameBehavior),
            (string)"Hacks the endpoint name", b => new HackEndpointNameBehavior(endpointName))
        {
            InsertAfterIfExists("AuditHostInformation");
        }
    }
}