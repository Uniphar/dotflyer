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
    /// <exception cref="Exception">Thrown when failed to send email message.</exception>
    public async Task SendAsync(EmailMessage emailMessage, CancellationToken cancellationToken = default)
    {
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

            throw new Exception(errorMessage);
        }
    }
}
