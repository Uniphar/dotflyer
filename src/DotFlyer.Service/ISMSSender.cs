namespace DotFlyer.Service;

/// <summary>
/// Represents an sms sender.
/// </summary>
public interface ISMSSender
{
    /// <summary>
    /// Sends an sms message.
    /// </summary>
    /// <param name="smsMessage">The <see cref="SMSMessage"/> instance to send.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/> instance to signal the request to cancel the operation.</param>
    /// <returns>The <see cref="Task"/> representing the asynchronous operation.</returns>
    public Task SendAsync(SMSMessage smsMessage, CancellationToken cancellationToken = default);
}