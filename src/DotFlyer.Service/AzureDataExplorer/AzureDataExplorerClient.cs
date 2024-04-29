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
    /// Creates or updates tables in Azure Data Explorer.
    /// </summary>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/> instance to signal the request to cancel the operation.</param>
    /// <returns>The <see cref="Task"/> representing the asynchronous operation.</returns>
    public async Task CreateOrUpdateTablesAsync(CancellationToken cancellationToken = default)
    {
        await CreateOrUpdateTableAsync(EmailTable.Instance, cancellationToken);
        await CreateOrUpdateTableAsync(SMSTable.Instance, cancellationToken);
    }

    /// <summary>
    /// Ingests data into Azure Data Explorer.
    /// </summary>
    /// <typeparam name="TData">The type of data to ingest.</typeparam>
    /// <param name="data">The data to ingest.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/> instance to signal the request to cancel the operation.</param>
    /// <returns>The <see cref="Task"/> representing the asynchronous operation.</returns>
    /// <exception cref="ArgumentException">Thrown when the data type is invalid.</exception>
    public async Task IngestDataAsync<TData>(TData data, CancellationToken cancellationToken = default)
    {
        try
        {
            if (data is EmailData emailData)
            {
                await IngestDataAsync(emailData, EmailTable.Instance.TableName, EmailTable.Instance.MappingName, cancellationToken);
            }
            else if (data is SMSData smsData)
            {
                await IngestDataAsync(smsData, SMSTable.Instance.TableName, SMSTable.Instance.MappingName, cancellationToken);
            }
            else
            {
                throw new ArgumentException($"Invalid data type: {typeof(TData).Name}");
            }
        }
        catch (Exception ex)
        {
            logger.LogError(ex, $"Failed to ingest data: {ex.Message}");
        }
    }

    /// <summary>
    /// Creates or updates table in Azure Data Explorer.
    /// </summary>
    /// <param name="table">Table to create or update.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/> instance to signal the request to cancel the operation.</param>
    /// <returns>The <see cref="Task"/> representing the asynchronous operation.</returns>
    private async Task CreateOrUpdateTableAsync(BaseTable table, CancellationToken cancellationToken = default)
    {
        TableSchema schema = new() { Name = table.TableName };

        table.Schema.ForEach(column =>
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
            CslCommandGenerator.GenerateTableMappingCreateOrAlterCommand(IngestionMappingKind.Json, table.TableName, table.MappingName, table.Mapping);

        await cslAdminProvider.ExecuteControlCommandAsync(cslAdminProvider.DefaultDatabaseName, createTableMappingCommand);
    }

    /// <summary>
    /// Ingests data into Azure Data Explorer.
    /// </summary>
    /// <typeparam name="TData">The type of data to ingest.</typeparam>
    /// <param name="data">The data to ingest.</param>
    /// <param name="tableName">The name of the table to ingest data into.</param>
    /// <param name="tableMapping">The name of the table mapping to use for ingestion.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/> instance to signal the request to cancel the operation.</param>
    /// <returns>The <see cref="Task"/> representing the asynchronous operation.</returns>
    private async Task IngestDataAsync<TData>(TData data, string tableName, string tableMapping, CancellationToken cancellationToken = default)
    {
        using MemoryStream dataStream = new(JsonSerializer.SerializeToUtf8Bytes(data));

        await kustoIngestClient.IngestFromStreamAsync(dataStream, new(cslAdminProvider.DefaultDatabaseName, tableName)
        {
            Format = DataSourceFormat.json,
            IngestionMapping = new() { IngestionMappingReference = tableMapping }
        });
    }
}
