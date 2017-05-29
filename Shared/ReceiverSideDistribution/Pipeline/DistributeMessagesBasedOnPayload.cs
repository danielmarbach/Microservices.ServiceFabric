using System;
using System.Threading.Tasks;
using NServiceBus;
using NServiceBus.Pipeline;

class DistributeMessagesBasedOnPayload :
    IBehavior<IIncomingLogicalMessageContext, IIncomingLogicalMessageContext>
{
    string localPartitionKey;
    Forwarder forwarder;
    Func<object, string> mapper;

    public DistributeMessagesBasedOnPayload(string localPartitionKey, Forwarder forwarder, Func<object, string> mapper)
    {
        this.localPartitionKey = localPartitionKey;
        this.forwarder = forwarder;
        this.mapper = mapper;
    }

    public Task Invoke(IIncomingLogicalMessageContext context, Func<IIncomingLogicalMessageContext, Task> next)
    {
        var intent = GetMessageIntent(context);
        var isSubscriptionMessage = intent == MessageIntentEnum.Subscribe || intent == MessageIntentEnum.Unsubscribe;
        var isReply = intent == MessageIntentEnum.Reply;

        if (isSubscriptionMessage || isReply)
        {
            return next(context);
        }

        // If the header is set we assume it's validity was checked by DistributeMessagesBasedOnHeader
        if (context.MessageHeaders.ContainsKey(PartitionHeaders.PartitionKey))
        {
            return next(context);
        }

        var messagePartitionKey = mapper(context.Message.Instance);

        if (string.IsNullOrWhiteSpace(messagePartitionKey))
        {
            throw new PartitionMappingFailedException($"Could not map a partition key for message of type {context.Headers[Headers.EnclosedMessageTypes]}");
        }

        if (messagePartitionKey == localPartitionKey)
        {
            Logger.Log($"::RSD:: Received message: {context.Headers[Headers.EnclosedMessageTypes]} with Mapped PartitionKey={messagePartitionKey} on partition {localPartitionKey}");
            return next(context);
        }

        Logger.Log($"::RSD:: Forwarding received message: {context.Headers[Headers.EnclosedMessageTypes]} with Mapped PartitionKey={messagePartitionKey} on partition {localPartitionKey} to partition {messagePartitionKey}");

        context.Headers[PartitionHeaders.PartitionKey] = messagePartitionKey;

        return forwarder.Forward(context, messagePartitionKey);
    }

    static MessageIntentEnum? GetMessageIntent(IMessageProcessingContext context)
    {
        string intentStr;

        if (context.MessageHeaders.TryGetValue(Headers.MessageIntent, out intentStr))
        {
            return (MessageIntentEnum) Enum.Parse(typeof(MessageIntentEnum), intentStr);
        }

        return null;
    }
}