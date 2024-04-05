namespace DotFlyer.Service.Tests;

[TestClass, TestCategory("Integration")]
public class DotFlyerServiceTests
{
    private static ServiceBusClient? _serviceBusClient;
    private static ServiceBusSender? _serviceBusSender;
    private static HttpClient? _httpClient;
    private static ICslQueryProvider? _cslQueryProvider;

    private static string? _senderEmail;
    private static string? _receiverEmail;
    private static string? _adxDatabaseName;

    private static CancellationToken _cancellationToken;

    [ClassInitialize]
    public static async Task ClassInitialize(TestContext context)
    {
        _cancellationToken = context.CancellationTokenSource.Token;

        var azureKeyVaultName = Environment.GetEnvironmentVariable("AZURE_KEY_VAULT_NAME");

        DefaultAzureCredential credential = new();

        SecretClient secretClient = new(new($"https://{azureKeyVaultName}.vault.azure.net/"), credential);

        var serviceBusNamespaceName = await secretClient.GetSecretAsync("AzureServiceBus--Name", cancellationToken: _cancellationToken);

        _serviceBusClient = new(serviceBusNamespaceName.Value.Value, credential);

        _serviceBusSender = _serviceBusClient.CreateSender("dotflyer-email");

        var sendgridApiKeyIntegrationTest = await secretClient.GetSecretAsync("SendGrid--ApiKeyIntegrationTest", cancellationToken: _cancellationToken);

        _httpClient = new() { BaseAddress = new("https://api.sendgrid.com/v3/") };
        _httpClient.DefaultRequestHeaders.TryAddWithoutValidation("Authorization", $"Bearer {sendgridApiKeyIntegrationTest.Value.Value}");

        _senderEmail = (await secretClient.GetSecretAsync("integration-test-dotflyer-sender", cancellationToken: _cancellationToken)).Value.Value;
        _receiverEmail = (await secretClient.GetSecretAsync("integration-test-dotflyer-receiver", cancellationToken: _cancellationToken)).Value.Value;

        var _adxHostAddress = (await secretClient.GetSecretAsync("AzureDataExplorer--HostAddress", cancellationToken: _cancellationToken)).Value.Value;
        _adxDatabaseName = (await secretClient.GetSecretAsync("AzureDataExplorer--DatabaseName", cancellationToken: _cancellationToken)).Value.Value;

        var kcsb = new KustoConnectionStringBuilder(_adxHostAddress, _adxDatabaseName)
            .WithAadTokenProviderAuthentication(async () => 
                (await credential.GetTokenAsync(new(["https://kusto.kusto.windows.net/.default"]), cancellationToken: _cancellationToken)).Token);

        _cslQueryProvider = KustoClientFactory.CreateCslQueryProvider(kcsb);
    }


    [TestMethod]
    public async Task DotFlyerService_ShouldSendEmail()
    {
        var randomGuid = Guid.NewGuid();

        EmailMessage emailMessage = new()
        {
            Subject = $"Test email {randomGuid}",
            Body = $"Test email body {randomGuid}",
            FromEmail = _senderEmail!,
            FromName = "Integration Test",
            To = [
                new()
                {
                    Email = _receiverEmail!,
                    Name = "Integration Test Destination Address"
                }
            ]
        };

        ServiceBusMessage message = new(Encoding.UTF8.GetBytes(JsonSerializer.Serialize(emailMessage)));

        await _serviceBusSender!.SendMessageAsync(message);

        EmailData emailData = await _cslQueryProvider!
            .WaitSingleQueryResult<EmailData>($"[\"{EmailTable.TableName}\"] | where Subject == \"{emailMessage.Subject}\"", TimeSpan.FromMinutes(5), _cancellationToken);

        emailData.Should().NotBeNull();
        emailData.Subject.Should().Be(emailMessage.Subject);
        emailData.Body.Should().Be(emailMessage.Body);
        emailData.FromEmail.Should().Be(emailMessage.FromEmail);
        emailData.FromName.Should().Be(emailMessage.FromName);
        emailData.To.Should().Be(JsonSerializer.Serialize(emailMessage.To));
        emailData.SendGridStatusCodeInt.Should().Be(202);
        emailData.SendGridStatusCodeString.Should().Be("Accepted");
    }

    [ClassCleanup]
    public static async Task ClassCleanup()
    {
        if (_serviceBusSender != null) await _serviceBusSender.DisposeAsync();
        if (_serviceBusClient != null) await _serviceBusClient.DisposeAsync();
        _httpClient?.Dispose();
        _cslQueryProvider?.Dispose();
    }
}
