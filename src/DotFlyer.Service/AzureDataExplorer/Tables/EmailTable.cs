namespace DotFlyer.Service.AzureDataExplorer.Tables;

/// <summary>
/// Contains the schema and mapping for the email table in Azure Data Explorer.
/// </summary>
public class EmailTable : BaseTable
{
    private static readonly EmailTable table = new()
    {
        TableName = "DotFlyerEmails",
        MappingName = "DotFlyerEmailsMapping",
        Schema =
        [
            new() { Name = nameof(EmailData.IngestDateTimeUtc), Type = typeof(DateTime).FullName! },
            new() { Name = nameof(EmailData.SendGridStatusCodeInt), Type = typeof(int).FullName! },
            new() { Name = nameof(EmailData.SendGridStatusCodeString), Type = typeof(string).FullName! },
            new() { Name = nameof(EmailData.SendGridResponseContent), Type = typeof(string).FullName! },
            new() { Name = nameof(EmailData.FromEmail), Type = typeof(string).FullName! },
            new() { Name = nameof(EmailData.FromName), Type = typeof(string).FullName! },
            new() { Name = nameof(EmailData.To), Type = typeof(string).FullName! },
            new() { Name = nameof(EmailData.Cc), Type = typeof(string).FullName! },
            new() { Name = nameof(EmailData.Bcc), Type = typeof(string).FullName! },
            new() { Name = nameof(EmailData.Subject), Type = typeof(string).FullName! },
            new() { Name = nameof(EmailData.Body), Type = typeof(string).FullName! },
            new() { Name = nameof(EmailData.Attachments), Type = typeof(string).FullName! }
        ]
    };

    public static EmailTable Instance => table;
}