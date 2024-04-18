namespace DotFlyer.Service.AzureDataExplorer;

/// <summary>
/// Interface for an Azure Data Explorer client.
/// </summary>
public interface IAzureDataExplorerClient
{
    /// <summary>
    /// Initializes Azure Data Explorer client.
    /// </summary>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/> instance to signal the request to cancel the operation.</param>
    /// <returns>The <see cref="Task"/> representing the asynchronous operation.</returns>
    public Task CreateOrUpdateTablesAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Ingests email data into Azure Data Explorer.
    /// </summary>
    /// <param name="emailData">The <see cref="EmailData"/> instance to ingest.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/> instance to signal the request to cancel the operation.</param>
    /// <returns>The <see cref="Task"/> representing the asynchronous operation.</returns>
    public Task IngestDataAsync<TData>(TData data, CancellationToken cancellationToken = default);
}
