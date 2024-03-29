namespace DotFlyer.Service.AzureDataExplorer.Tables;

public class EmailTable
{
    public const string TableName = "dotflyer-email";

    public static Tuple<string, string>[] Schema =
    [
        Tuple.Create(nameof(EmailData.FromEmail), typeof(string).FullName!),
        Tuple.Create(nameof(EmailData.FromName), typeof(string).FullName!),
        Tuple.Create(nameof(EmailData.To), typeof(string).FullName!),
        Tuple.Create(nameof(EmailData.Subject), typeof(string).FullName!),
        Tuple.Create(nameof(EmailData.Body), typeof(string).FullName!),
        Tuple.Create(nameof(EmailData.SendGridStatusCodeInt), typeof(int).FullName!),
        Tuple.Create(nameof(EmailData.SendGridStatusCodeString), typeof(string).FullName!)
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
                    ColumnName = column.Item1,
                    Properties = new()
                    {
                        { MappingConsts.Path, $"$.{column.Item1}" }
                    }
                });
            }

            return columnMapping;
        }
    }
}
