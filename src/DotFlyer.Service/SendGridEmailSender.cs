namespace DotFlyer.Service;

/// <summary>
/// The email sender, which uses SendGrid.
/// </summary>
/// <param name="sendGridClient"></param>
public class SendGridEmailSender(
    ISendGridClient sendGridClient,
    AzureDataExplorerClient adxClient) : IEmailSender
{
    /// <summary>
    /// Sends an email message.
    /// </summary>
    /// <param name="emailMessage">The <see cref="EmailMessage"/> instance to send.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/> instance to signal the request to cancel the operation.</param>
    /// <returns>Task representing the asynchronous operation.</returns>
    /// <exception cref="ArgumentException">Thrown when email message does not have any recipients in the 'To' field.</exception>
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

        var result = await sendGridClient.SendEmailAsync(sendGridMessage, cancellationToken);

        await adxClient.IngestEmailDataAsync(EmailData.ConvertToAdxModel(emailMessage, result.StatusCode), cancellationToken);

        if (result.StatusCode != HttpStatusCode.Accepted)
        {
            var errorMessage = await result.Body.ReadAsStringAsync(cancellationToken);

            throw new HttpRequestException(errorMessage);
        }
    }
}
