namespace DotFlyer.Service.AzureDataExplorer.Tables;

public class EmailTable
{
    public const string TableName = "dotflyer-email";

    public class Column
    {
        public required string Name { get; set; }

        public required string Type { get; set; }
    }

    public static List<Column> Schema =
    [
        new() { Name = nameof(EmailData.FromEmail), Type = typeof(string).FullName! },
        new() { Name = nameof(EmailData.FromName), Type = typeof(string).FullName! },
        new() { Name = nameof(EmailData.To), Type = typeof(string).FullName! },
        new() { Name = nameof(EmailData.Subject), Type = typeof(string).FullName! },
        new() { Name = nameof(EmailData.Body), Type = typeof(string).FullName! },
        new() { Name = nameof(EmailData.SendGridStatusCodeInt), Type = typeof(int).FullName! },
        new() { Name = nameof(EmailData.SendGridStatusCodeString), Type = typeof(string).FullName! },
        new() { Name = nameof(EmailData.IngestDateTimeUtc), Type = typeof(DateTime).FullName! }
    ];

    public const string MappingName = "dotflyer-email-mapping";

    public static List<ColumnMapping> Mapping
    {
        get
        {
            List<ColumnMapping> columnMapping = [];

            foreach (var column in Schema)
            {
                columnMapping.Add(new()
                {
                    ColumnName = column.Name,
                    Properties = new()
                    {
                        { MappingConsts.Path, $"$.{column.Name}" }
                    }
                });
            }

            return columnMapping;
        }
    }
}