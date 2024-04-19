namespace DotFlyer.Service;

/// <summary>
/// The email sender service.
/// </summary>
/// <param name="credential">The <see cref="DefaultAzureCredential"/> instance to authenticate with Azure services.</param>
/// <param name="sendGridClient">The <see cref="ISendGridClient"/> instance to send emails.</param>
/// <param name="adxClient">The <see cref="IAzureDataExplorerClient"/> instance to ingest email data.</param>
public class EmailSender(
    DefaultAzureCredential credential,
    ISendGridClient sendGridClient,
    IAzureDataExplorerClient adxClient) : IEmailSender
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
        if (emailMessage.To.Count == 0)
        {
            throw new ArgumentException("Email message must have at least one recipient in the 'To' field.");
        }

        SendGridMessage sendGridMessage = new()
        {
            From = new(emailMessage.FromEmail, emailMessage.FromName),
            Subject = emailMessage.Subject,
            HtmlContent = emailMessage.Body
        };

        emailMessage.To.ForEach(emailRecipient => sendGridMessage.AddTo(new EmailAddress(emailRecipient.Email, emailRecipient.Name)));
        emailMessage.Cc.ForEach(emailRecipient => sendGridMessage.AddCc(new EmailAddress(emailRecipient.Email, emailRecipient.Name)));
        emailMessage.Bcc.ForEach(emailRecipient => sendGridMessage.AddBcc(new EmailAddress(emailRecipient.Email, emailRecipient.Name)));

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

        var result = await sendGridClient.SendEmailAsync(sendGridMessage, cancellationToken);

        var resultContent = await result.Body.ReadAsStringAsync(cancellationToken);

        await adxClient.IngestDataAsync(EmailData.ConvertToAdxModel(emailMessage, result.StatusCode, resultContent), cancellationToken);

        if (result.StatusCode != HttpStatusCode.Accepted)
        {
            throw new HttpRequestException(resultContent);
        }
    }
}
