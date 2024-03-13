namespace DotFlyer.Shared.Payload;

internal class EmailMessage
{
    public required string FromEmail { get; set; }

    public required string FromName { get; set; }

    public required string ToEmail { get; set; }

    public required string ToName { get; set; }

    public required string Subject { get; set; }

    public required string Body { get; set; }
}
