namespace DotFlyer.Service.AzureDataExplorer.Tables;

/// <summary>
/// Contains the schema and mapping for the SMS table in Azure Data Explorer.
/// </summary>
public class SMSTable : BaseTable
{
    private static readonly SMSTable table = new()
    {
        TableName = "DotFlyerSMSs",
        MappingName = "DotFlyerSMSsMapping",
        Schema =
        [
            new() { Name = nameof(SMSData.IngestDateTimeUtc), Type = typeof(DateTime).FullName! },
            new() { Name = nameof(SMSData.TwilioStatusCodeInt), Type = typeof(int).FullName! },
            new() { Name = nameof(SMSData.TwilioStatusCodeString), Type = typeof(string).FullName! },
            new() { Name = nameof(SMSData.TwilioResponseContent), Type = typeof(string).FullName! },
            new() { Name = nameof(SMSData.From), Type = typeof(string).FullName! },
            new() { Name = nameof(SMSData.To), Type = typeof(string).FullName! },
            new() { Name = nameof(SMSData.Body), Type = typeof(string).FullName! },
            new() { Name = nameof(SMSData.Tags), Type = typeof(string).FullName! }
        ]
    };

    public static SMSTable Instance => table;
}