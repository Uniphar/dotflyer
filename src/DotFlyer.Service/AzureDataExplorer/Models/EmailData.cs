namespace DotFlyer.Service.AzureDataExplorer.Models;

/// <summary>
/// Contains the model for email data to be ingested into Azure Data Explorer.
/// </summary>
public class EmailData
{
    public required string FromEmail { get; set; }

    public required string FromName { get; set; }

    public required string To { get; set; }

    public required string Cc { get; set; }

    public required string Bcc { get; set; }

    public required string Subject { get; set; }

    public required string Body { get; set; }

    public required string Attachments { get; set; }

    public required string Tags { get; set; }

    public required int SendGridStatusCodeInt { get; set; }

    public required string SendGridStatusCodeString { get; set; }

    public required string SendGridResponseContent { get; set; }

    public required DateTime IngestDateTimeUtc { get; set; }

    public static EmailData ConvertToAdxModel(EmailMessage emailMessage, HttpStatusCode sendgridStatusCode, string sendgridResponseContent, string? htmlContent = null) => new()
    {
        FromEmail = emailMessage.From.Email,
        FromName = emailMessage.From.Name,
        To = JsonSerializer.Serialize(emailMessage.To),
        Cc = emailMessage.Cc == null ? "[]" : JsonSerializer.Serialize(emailMessage.Cc),
        Bcc = emailMessage.Bcc == null ? "[]" : JsonSerializer.Serialize(emailMessage.Bcc),
        Attachments = emailMessage.Attachments == null ? "[]" : JsonSerializer.Serialize(emailMessage.Attachments.Select(attachment => new Attachment(attachment))),
        Subject = emailMessage.Subject,
        Body = !string.IsNullOrEmpty(htmlContent) ? htmlContent : emailMessage.Body ?? string.Empty,
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
