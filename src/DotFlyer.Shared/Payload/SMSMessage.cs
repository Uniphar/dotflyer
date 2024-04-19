namespace DotFlyer.Shared.Payload;

/// <summary>
/// SMS message payload.
/// </summary>
public class SMSMessage
{
    public required string To { get; set; }

    public required string Body { get; set; }

    public Dictionary<string, string>? Tags { get; set; }
}