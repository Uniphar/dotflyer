namespace DotFlyer.Api.TopicSenders;

/// <summary>
/// The SMS topic sender.
/// </summary>
/// <param name="azureClientFactory">The <see cref="IAzureClientFactory{T}"/>.</param>
public class SmsTopicSender(IAzureClientFactory<ServiceBusSender> azureClientFactory) : BaseTopicSender(azureClientFactory)
{
    public static string Name => "sms-topic-sender";

    public override string SenderClientName => Name;
}
