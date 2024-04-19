namespace DotFlyer.Service.AzureDataExplorer.Models;

/// <summary>
/// Contains the model for SMS data to be ingested into Azure Data Explorer.
/// </summary>
public class SMSData : SMSMessage
{
    public required string From { get; set; }

    public new required string Tags { get; set; }

    public required int TwilioStatusCodeInt { get; set; }

    public required string TwilioStatusCodeString { get; set; }

    public required string TwilioResponseContent { get; set; }

    public required DateTime IngestDateTimeUtc { get; set; }

    public static SMSData ConvertToAdxModel(SMSMessage smsMessage, string from, HttpStatusCode twilioStatusCode, string twilioResponseContent) => new()
    {
        From = from,
        To = smsMessage.To,
        Body = smsMessage.Body,
        Tags = smsMessage.Tags == null ? "{}" : JsonSerializer.Serialize(smsMessage.Tags),
        TwilioStatusCodeInt = (int)twilioStatusCode,
        TwilioStatusCodeString = twilioStatusCode.ToString(),
        TwilioResponseContent = twilioResponseContent,
        IngestDateTimeUtc = DateTime.UtcNow
    };
}