namespace DotFlyer.Service.AzureDataExplorer.Models;

/// <summary>
/// Contains the model for email data to be ingested into Azure Data Explorer.
/// </summary>
public class EmailData : EmailMessage
{
    public new required string To { get; set; }

    public new required string Cc { get; set; }

    public new required string Bcc { get; set; }

    public new required string Attachments { get; set; }

    public new required string Tags { get; set; }

    public required int SendGridStatusCodeInt { get; set; }

    public required string SendGridStatusCodeString { get; set; }

    public required string SendGridResponseContent { get; set; }

    public required DateTime IngestDateTimeUtc { get; set; }

    public static EmailData ConvertToAdxModel(EmailMessage emailMessage, HttpStatusCode sendgridStatusCode, string sendgridResponseContent) => new()
    {
        FromEmail = emailMessage.FromEmail,
        FromName = emailMessage.FromName,
        To = JsonSerializer.Serialize(emailMessage.To),
        Cc = JsonSerializer.Serialize(emailMessage.Cc),
        Bcc = JsonSerializer.Serialize(emailMessage.Bcc),
        Attachments = JsonSerializer.Serialize(emailMessage.Attachments.Select(attachment => new Attachment(attachment))),
        Subject = emailMessage.Subject,
        Body = emailMessage.Body,
        Tags = emailMessage.Tags == null ? "{}" : JsonSerializer.Serialize(emailMessage.Tags),
        SendGridStatusCodeInt = (int)sendgridStatusCode,
        SendGridStatusCodeString = sendgridStatusCode.ToString(),
        SendGridResponseContent = sendgridResponseContent,
        IngestDateTimeUtc = DateTime.UtcNow
    };

    public class Attachment(string URI)
    {
        public string URI { get; } = URI;
    }
}
