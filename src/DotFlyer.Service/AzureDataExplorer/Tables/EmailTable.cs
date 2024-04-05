namespace DotFlyer.Service.AzureDataExplorer.Tables;

public class EmailTable
{
    public const string TableName = "DotFlyerEmails";

    public class Column
    {
        public required string Name { get; set; }

        public required string Type { get; set; }
    }

    public static List<Column> Schema =
    [
        new() { Name = nameof(EmailData.IngestDateTimeUtc), Type = typeof(DateTime).FullName! },
        new() { Name = nameof(EmailData.SendGridStatusCodeInt), Type = typeof(int).FullName! },
        new() { Name = nameof(EmailData.SendGridStatusCodeString), Type = typeof(string).FullName! },
        new() { Name = nameof(EmailData.FromEmail), Type = typeof(string).FullName! },
        new() { Name = nameof(EmailData.FromName), Type = typeof(string).FullName! },
        new() { Name = nameof(EmailData.To), Type = typeof(string).FullName! },
        new() { Name = nameof(EmailData.Cc), Type = typeof(string).FullName! },
        new() { Name = nameof(EmailData.Bcc), Type = typeof(string).FullName! },
        new() { Name = nameof(EmailData.Subject), Type = typeof(string).FullName! },
        new() { Name = nameof(EmailData.Body), Type = typeof(string).FullName! }
    ];

    public const string MappingName = "DotFlyerEmailsMapping";

    public static List<ColumnMapping> Mapping
    {
        get
        {
            List<ColumnMapping> columnMapping = [];

            Schema.ForEach(column =>
            {
                columnMapping.Add(new()
                {
                    ColumnName = column.Name,
                    Properties = new()
                    {
                        { MappingConsts.Path, $"$.{column.Name}" }
                    }
                });
            });

            return columnMapping;
        }
    }
}