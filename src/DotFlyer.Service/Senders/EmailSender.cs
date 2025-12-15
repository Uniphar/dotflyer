using DotFlyer.EmailTemplates;
using Microsoft.AspNetCore.Components;

namespace DotFlyer.Service.Senders;

/// <summary>
/// The email sender service.
/// </summary>
/// <param name="credential">The <see cref="DefaultAzureCredential"/> instance to authenticate with Azure services.</param>
/// <param name="sendGridClient">The <see cref="ISendGridClient"/> instance to send emails.</param>
/// <param name="telemetryClient">The <see cref="TelemetryClient"/> instance to log telemetry data.</param>
/// <param name="adxClient">The <see cref="IAzureDataExplorerClient"/> instance to ingest email data.</param>
public class EmailSender(
    DefaultAzureCredential credential,
    ISendGridClient sendGridClient,
    TelemetryClient telemetryClient,
    IAzureDataExplorerClient adxClient,
    EmailHtmlRenderer? htmlRenderer = null) : IEmailSender
{
    /// <summary>
    /// Sends an email message.
    /// </summary>
    /// <param name="emailMessage">The <see cref="EmailMessage"/> instance to send.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/> instance to signal the request to cancel the operation.</param>
    /// <returns>Task representing the asynchronous operation.</returns>
    /// <exception cref="ArgumentException">Thrown when email message does not have any recipients in the 'To' field.</exception>
    /// <exception cref="FileNotFoundException">Thrown when an attachment is not found.</exception>
    /// <exception cref="HttpRequestException">Thrown when the email message could not be sent.</exception>"
    public async Task SendAsync(EmailMessage emailMessage, CancellationToken cancellationToken = default)
    {
        if (!emailMessage.To.Any())
        {
            throw new ArgumentException("Email message must have at least one recipient in the 'To' field.");
        }

        string htmlBody = emailMessage.Body;

        if (htmlRenderer != null)
        {
            htmlBody = await htmlRenderer.RenderAsync(emailMessage);
        }

        SendGridMessage sendGridMessage = new()
        {
            From = new(emailMessage.From.Email, emailMessage.From.Name),
            Subject = emailMessage.Subject,
            HtmlContent = htmlBody,
            PlainTextContent = emailMessage.Body
        };

        foreach (var contact in emailMessage.To)
        {
            sendGridMessage.AddTo(new EmailAddress(contact.Email, contact.Name));
        }

        if (emailMessage.Cc != null)
        {
            foreach (var contact in emailMessage.Cc)
            {
                sendGridMessage.AddCc(new EmailAddress(contact.Email, contact.Name));
            }
        }

        if (emailMessage.Bcc != null)
        {
            foreach (var contact in emailMessage.Bcc)
            {
                sendGridMessage.AddBcc(new EmailAddress(contact.Email, contact.Name));
            }
        }

        if (emailMessage.Attachments != null)
        {
            foreach (var attachment in emailMessage.Attachments)
            {
                BlobClient blobClient = new(new(attachment), credential);

                if (await blobClient.ExistsAsync(cancellationToken))
                {
                    using MemoryStream blobStream = new();

                    await blobClient.DownloadToAsync(blobStream, cancellationToken);

                    sendGridMessage.AddAttachment(Path.GetFileName(attachment), Convert.ToBase64String(blobStream.ToArray()));
                }
                else
                {
                    throw new FileNotFoundException($"Attachment not found: {attachment}");
                }
            }
        }

        var result = await sendGridClient.SendEmailAsync(sendGridMessage, cancellationToken);

        var resultContent = await result.Body.ReadAsStringAsync(cancellationToken);

        await adxClient.IngestDataAsync(EmailData.ConvertToAdxModel(emailMessage, result.StatusCode, resultContent), cancellationToken);

        switch (result.StatusCode)
        {
            case HttpStatusCode.Accepted:
                break;

            case HttpStatusCode.BadRequest:
                telemetryClient.TrackInvalidEmailPayload(resultContent);
                break;

            default:
                throw new HttpRequestException(resultContent);
        }
    }
}
