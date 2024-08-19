namespace DotFlyer.Api.TopicSenders;

/// <summary>
/// The email topic sender.
/// </summary>
/// <param name="azureClientFactory">The <see cref="IAzureClientFactory{T}"/>.</param>
public class EmailTopicSender(IAzureClientFactory<ServiceBusSender> azureClientFactory) : BaseTopicSender(azureClientFactory)
{
    public static string Name => "email-topic-sender";

    public override string SenderClientName => Name;
}
