namespace DotFlyer.Service.AzureDataExplorer.Models;

/// <summary>
/// Contains the model for email data to be ingested into Azure Data Explorer.
/// </summary>
public class EmailData : EmailMessage
{
    public new required string To { get; set; }

    public new required string Cc { get; set; }

    public new required string Bcc { get; set; }

    public required int SendGridStatusCodeInt { get; set; }

    public required string SendGridStatusCodeString { get; set; }

    public required DateTime IngestDateTimeUtc { get; set; }

    public static EmailData ConvertToAdxModel(EmailMessage emailMessage, HttpStatusCode sendgridStatusCode) => new()
    {
        FromEmail = emailMessage.FromEmail,
        FromName = emailMessage.FromName,
        To = JsonSerializer.Serialize(emailMessage.To),
        Cc = JsonSerializer.Serialize(emailMessage.Cc),
        Bcc = JsonSerializer.Serialize(emailMessage.Bcc),
        Subject = emailMessage.Subject,
        Body = emailMessage.Body,
        SendGridStatusCodeInt = (int)sendgridStatusCode,
        SendGridStatusCodeString = sendgridStatusCode.ToString(),
        IngestDateTimeUtc = DateTime.UtcNow
    };
}
