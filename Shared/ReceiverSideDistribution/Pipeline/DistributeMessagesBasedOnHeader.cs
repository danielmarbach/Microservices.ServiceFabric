using System;
using System.Threading.Tasks;
using NServiceBus;
using NServiceBus.Pipeline;
using NServiceBus.Transport;

class DistributeMessagesBasedOnHeader :
    IBehavior<IIncomingPhysicalMessageContext, IIncomingPhysicalMessageContext>
{
    string localPartitionKey;
    Forwarder forwarder;

    public DistributeMessagesBasedOnHeader(string localPartitionKey, Forwarder forwarder)
    {
        this.localPartitionKey = localPartitionKey;
        this.forwarder = forwarder;
    }

    public Task Invoke(IIncomingPhysicalMessageContext context, Func<IIncomingPhysicalMessageContext, Task> next)
    {
        var intent = context.Message.GetMesssageIntent();
        var isSubscriptionMessage = intent == MessageIntentEnum.Subscribe || intent == MessageIntentEnum.Unsubscribe;
        var isReply = intent == MessageIntentEnum.Reply;

        if (isSubscriptionMessage || isReply)
        {
            return next(context);
        }

        string messagePartitionKey;
        var hasPartitionKeyHeader = context.MessageHeaders.TryGetValue(PartitionHeaders.PartitionKey, out messagePartitionKey);

        // 1. The header value isn't present (logical behavior will check message contents)
        // 2. The header value matches local partition key
        if (!hasPartitionKeyHeader || messagePartitionKey == localPartitionKey)
        {
            return next(context);
        }

        var message = $"##### Received message: {context.Message.Headers[Headers.EnclosedMessageTypes]} with Mapped PartitionKey={messagePartitionKey} on partition {localPartitionKey}";

        Logger.Log(message);

        return forwarder.Forward(context, messagePartitionKey);
    }
}