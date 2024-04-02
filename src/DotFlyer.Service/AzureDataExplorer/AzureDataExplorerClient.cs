namespace DotFlyer.Service.AzureDataExplorer;

public class AzureDataExplorerClient(
    ILogger<AzureDataExplorerClient> logger,
    ICslAdminProvider cslAdminProvider,
    IKustoIngestClient kustoIngestClient)
{
    public async Task InitializeAsync(CancellationToken cancellationToken = default)
    {
        TableSchema schema = new() { Name = EmailTable.TableName };

        foreach (var column in EmailTable.Schema)
        {
            schema.AddColumnIfMissing(new()
            {
                Name = column.Name,
                Type = column.Type
            });
        }

        var createTableCommand = CslCommandGenerator.GenerateTableCreateMergeCommand(schema);

        await cslAdminProvider.ExecuteControlCommandAsync(cslAdminProvider.DefaultDatabaseName, createTableCommand);

        var createTableMappingCommand =
            CslCommandGenerator.GenerateTableMappingCreateOrAlterCommand(IngestionMappingKind.Json, EmailTable.TableName, EmailTable.MappingName, EmailTable.Mapping);

        await cslAdminProvider.ExecuteControlCommandAsync(cslAdminProvider.DefaultDatabaseName, createTableMappingCommand);
    }

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
