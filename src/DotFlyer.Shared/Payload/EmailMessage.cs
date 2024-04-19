namespace DotFlyer.Shared.Payload;

/// <summary>
/// Email message payload.
/// </summary>
public class EmailMessage
{
    public required string FromEmail { get; set; }

    public required string FromName { get; set; }

    public List<EmailRecipient> To { get; set; } = [];

    public List<EmailRecipient> Cc { get; set; } = [];

    public List<EmailRecipient> Bcc { get; set; } = [];

    public required string Subject { get; set; }

    public required string Body { get; set; }

    public List<string> Attachments { get; set; } = [];
}

/// <summary>
/// Email recipient payload.
/// </summary>
public class EmailRecipient
{
    public required string Email { get; set; }

    public required string Name { get; set; }
}