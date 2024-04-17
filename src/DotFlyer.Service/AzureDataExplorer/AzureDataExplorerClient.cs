namespace DotFlyer.Service.AzureDataExplorer;

/// <summary>
/// Azure Data Explorer client.
/// </summary>
/// <param name="logger">The <see cref="ILogger"/> instance.</param>
/// <param name="cslAdminProvider">The <see cref="ICslAdminProvider"/> instance that allows to execute Azure Data Explorer control commands.</param>
/// <param name="kustoIngestClient">The <see cref="IKustoIngestClient"/> instance that allows to ingest data into Azure Data Explorer.</param>
public class AzureDataExplorerClient(
    ILogger<AzureDataExplorerClient> logger,
    ICslAdminProvider cslAdminProvider,
    IKustoIngestClient kustoIngestClient) : IAzureDataExplorerClient
{
    /// <summary>
    /// Initializes Azure Data Explorer client.
    /// </summary>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/> instance to signal the request to cancel the operation.</param>
    /// <returns>The <see cref="Task"/> representing the asynchronous operation.</returns>
    public async Task InitializeAsync(CancellationToken cancellationToken = default)
    {
        TableSchema schema = new() { Name = EmailTable.TableName };

        EmailTable.Schema.ForEach(column =>
        {
            schema.AddColumnIfMissing(new()
            {
                Name = column.Name,
                Type = column.Type
            });
        });

        var createTableCommand = CslCommandGenerator.GenerateTableCreateMergeCommand(schema);

        await cslAdminProvider.ExecuteControlCommandAsync(cslAdminProvider.DefaultDatabaseName, createTableCommand);

        var createTableMappingCommand =
            CslCommandGenerator.GenerateTableMappingCreateOrAlterCommand(IngestionMappingKind.Json, EmailTable.TableName, EmailTable.MappingName, EmailTable.Mapping);

        await cslAdminProvider.ExecuteControlCommandAsync(cslAdminProvider.DefaultDatabaseName, createTableMappingCommand);
    }

    /// <summary>
    /// Ingests email data into Azure Data Explorer.
    /// </summary>
    /// <param name="emailData">The <see cref="EmailData"/> instance to ingest.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/> instance to signal the request to cancel the operation.</param>
    /// <returns>The <see cref="Task"/> representing the asynchronous operation.</returns>
    public async Task IngestEmailDataAsync(EmailData emailData, CancellationToken cancellationToken = default)
    {
        try
        {
            using MemoryStream emailDataStream = new(JsonSerializer.SerializeToUtf8Bytes(emailData));

            await kustoIngestClient.IngestFromStreamAsync(emailDataStream, new(cslAdminProvider.DefaultDatabaseName, EmailTable.TableName)
            {
                Format = DataSourceFormat.json,
                IngestionMapping = new() { IngestionMappingReference = EmailTable.MappingName }
            });
        }
        catch (Exception ex)
        {
            logger.LogError(ex, $"Failed to ingest email data: {ex.Message}");    
        }
    }
}
