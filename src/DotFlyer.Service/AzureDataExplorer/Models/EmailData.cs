namespace DotFlyer.Service.AzureDataExplorer.Models;

public class EmailData : EmailMessage
{
    public new required string To { get; set; }

    public required int SendGridStatusCodeInt { get; set; }

    public required string SendGridStatusCodeString { get; set; }

    public static EmailData ConvertToAdxModel(EmailMessage emailMessage, HttpStatusCode sendgridStatusCode) => new()
    {
        FromEmail = emailMessage.FromEmail,
        FromName = emailMessage.FromName,
        To = JsonSerializer.Serialize(emailMessage.To),
        Subject = emailMessage.Subject,
        Body = emailMessage.Body,
        SendGridStatusCodeInt = (int)sendgridStatusCode,
        SendGridStatusCodeString = sendgridStatusCode.ToString()
    };
}
