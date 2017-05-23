using NServiceBus;
using NServiceBus.Features;

class HardcodeReplyToAddressToLogicalAddressFeature : Feature
{
    protected override void Setup(FeatureConfigurationContext context)
    {
        var settings = context.Settings;
        var addressToLogicalAddress = new HardcodeReplyToAddressToLogicalAddress(settings.InstanceSpecificQueue());
        context.Pipeline.Register(addressToLogicalAddress, "Hardcodes the ReplyToAddress to the instance specific address of this endpoint.");
    }
}