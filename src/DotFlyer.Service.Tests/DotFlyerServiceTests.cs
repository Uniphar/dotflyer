namespace DotFlyer.Service.Tests;

[TestClass, TestCategory("Integration")]
public class DotFlyerServiceTests
{
    private static ServiceBusClient? _serviceBusClient;
    private static ServiceBusSender? _serviceBusSender;
    private static HttpClient? _httpClient;

    private static string? _senderEmail;
    private static string? _receiverEmail;

    [ClassInitialize]
    public static async Task ClassInitialize(TestContext context)
    {
        var azureKeyVaultName = Environment.GetEnvironmentVariable("AZURE_KEY_VAULT_NAME");

        SecretClient secretClient = new(new($"https://{azureKeyVaultName}.vault.azure.net/"), new DefaultAzureCredential());

        var serviceBusNamespaceName = await secretClient.GetSecretAsync("AzureServiceBus--Name");

        _serviceBusClient = new(serviceBusNamespaceName.Value.Value, new DefaultAzureCredential());

        _serviceBusSender = _serviceBusClient.CreateSender("dotflyer-email");

        var sendgridApiKeyIntegrationTest = await secretClient.GetSecretAsync("SendGrid--ApiKeyIntegrationTest");

        _httpClient = new() { BaseAddress = new("https://api.sendgrid.com/v3/") };
        _httpClient.DefaultRequestHeaders.TryAddWithoutValidation("Authorization", $"Bearer {sendgridApiKeyIntegrationTest.Value.Value}");

        _senderEmail = (await secretClient.GetSecretAsync("integration-test-dotflyer-sender")).Value.Value;
        _receiverEmail = (await secretClient.GetSecretAsync("integration-test-dotflyer-receiver")).Value.Value;
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

        var message = new ServiceBusMessage(Encoding.UTF8.GetBytes(JsonSerializer.Serialize(emailMessage)));

        await _serviceBusSender!.SendMessageAsync(message);

        // TODO: Assert that email is sent using ADX
    }

    [ClassCleanup]
    public static async Task ClassCleanup()
    {
        if (_serviceBusSender != null) await _serviceBusSender.DisposeAsync();
        if (_serviceBusClient != null) await _serviceBusClient.DisposeAsync();
        _httpClient?.Dispose();
    }
}
