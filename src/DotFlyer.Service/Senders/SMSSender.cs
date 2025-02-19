﻿namespace DotFlyer.Service.Senders;

/// <summary>
/// The SMS sender service.
/// </summary>
/// <param name="configuration">The <see cref="SMSSenderConfiguration"/> instance that contains the Twilio configuration.</param>
/// <param name="twilioRestClient">The <see cref="ITwilioRestClient"/> instance that is used to send SMS messages.</param>
/// <param name="telemetryClient">The <see cref="TelemetryClient"/> instance that is used to log telemetry data.</param>
/// <param name="adxClient">The <see cref="IAzureDataExplorerClient"/> instance that is used to ingest data into Azure Data Explorer.</param>
public class SMSSender(
    SMSSenderConfiguration configuration,
    ITwilioRestClient twilioRestClient,
    TelemetryClient telemetryClient,
    IAzureDataExplorerClient adxClient) : ISMSSender
{
    /// <summary>
    /// Sends an SMS message.
    /// </summary>
    /// <param name="smsMessage">The <see cref="SMSMessage"/> instance to send.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/> instance to signal the request to cancel the operation.</param>
    /// <returns>The <see cref="Task"/> representing the asynchronous operation.</returns>
    /// <exception cref="ArgumentException">Thrown when SMS message does not have a recipient in the 'To' field.</exception>
    /// <exception cref="HttpRequestException">Thrown when the SMS message could not be sent.</exception>
    public async Task SendAsync(SMSMessage smsMessage, CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(smsMessage.To, nameof(smsMessage.To));

        CreateMessageOptions options = new(new PhoneNumber(smsMessage.To))
        {
            Body = smsMessage.Body,
            From = new PhoneNumber(string.IsNullOrWhiteSpace(smsMessage.From) ? configuration.FromPhoneNumber : smsMessage.From),
        };

        Request twilioRequest = new(Twilio.Http.HttpMethod.Post,
                                    Twilio.Rest.Domain.Api,
                                    $"/2010-04-01/Accounts/{configuration.AccountSID}/Messages.json",
                                    postParams: options.GetParams());

        twilioRequest.SetAuth(configuration.ApiKeySID, configuration.ApiKeySecret);

        var result = await twilioRestClient.HttpClient.MakeRequestAsync(twilioRequest);

        await adxClient.IngestDataAsync(SMSData.ConvertToAdxModel(smsMessage, options.From.ToString(), result.StatusCode, result.Content), cancellationToken);

        switch (result.StatusCode)
        {
            case HttpStatusCode.Created:
                break;

            case HttpStatusCode.BadRequest:
                telemetryClient.TrackInvalidSMSPayload(result.Content);
                break;

            default:
                throw new HttpRequestException($"Failed to send SMS message: {result.Content}");
        }
    }
}

public class SMSSenderConfiguration
{
    public required string AccountSID { get; set; }

    public required string ApiKeySID { get; set; }

    public required string ApiKeySecret { get; set; }

    public required string FromPhoneNumber { get; set; }
}