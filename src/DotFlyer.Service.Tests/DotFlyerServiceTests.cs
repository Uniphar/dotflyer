﻿namespace DotFlyer.Service.Tests;

[TestClass, TestCategory("Integration")]
public class DotFlyerServiceTests
{
    private static ServiceBusClient? _serviceBusClient;
    private static ServiceBusSender? _emailServiceBusSender;
    private static ServiceBusSender? _smsServiceBusSender;
    private static System.Net.Http.HttpClient? _httpClient;
    private static ICslQueryProvider? _cslQueryProvider;

    private static string? _senderEmail;
    private static string? _receiverEmail;

    private static string? _receiverNumber;

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

        _emailServiceBusSender = _serviceBusClient.CreateSender("dotflyer-email");
        _smsServiceBusSender = _serviceBusClient.CreateSender("dotflyer-sms");

        var sendgridApiKeyIntegrationTest = await secretClient.GetSecretAsync("SendGrid--ApiKeyIntegrationTest", cancellationToken: _cancellationToken);

        _httpClient = new() { BaseAddress = new("https://api.sendgrid.com/v3/") };
        _httpClient.DefaultRequestHeaders.TryAddWithoutValidation("Authorization", $"Bearer {sendgridApiKeyIntegrationTest.Value.Value}");

        _senderEmail = (await secretClient.GetSecretAsync("integration-test-dotflyer-sender", cancellationToken: _cancellationToken)).Value.Value;
        _receiverEmail = (await secretClient.GetSecretAsync("integration-test-dotflyer-receiver", cancellationToken: _cancellationToken)).Value.Value;

        _receiverNumber = (await secretClient.GetSecretAsync("integration-test-dotflyer-receiver-number", cancellationToken: _cancellationToken)).Value.Value;

        var _adxHostAddress = (await secretClient.GetSecretAsync("AzureDataExplorer--HostAddress", cancellationToken: _cancellationToken)).Value.Value;

        var kcsb = new KustoConnectionStringBuilder(_adxHostAddress, "devops")
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
            From = new()
            {
                Email = _senderEmail!,
                Name = "Integration Test"
            },
            To = [
                new()
                {
                    Email = _receiverEmail!,
                    Name = "Integration Test Destination Address"
                }
            ],
            Tags = new Dictionary<string, string>()
            {
                { "TestName", "DotFlyer Service Integration Test" },
                { "TestRandomGuid", randomGuid.ToString() }
            }
        };

        ServiceBusMessage message = new(Encoding.UTF8.GetBytes(JsonSerializer.Serialize(emailMessage)));

        await _emailServiceBusSender!.SendMessageAsync(message);

        EmailData emailData = await _cslQueryProvider!
            .WaitSingleQueryResult<EmailData>($"[\"{EmailTable.Instance.TableName}\"] | where Subject == \"{emailMessage.Subject}\"", TimeSpan.FromMinutes(5), _cancellationToken);

        emailData.Should().NotBeNull();
        emailData.Subject.Should().Be(emailMessage.Subject);
        emailData.Body.Should().Be(emailMessage.Body);
        emailData.FromEmail.Should().Be(emailMessage.From.Email);
        emailData.FromName.Should().Be(emailMessage.From.Name);
        emailData.To.Should().Be(JsonSerializer.Serialize(emailMessage.To));
        emailData.Tags.Should().Be(JsonSerializer.Serialize(emailMessage.Tags));
        emailData.SendGridStatusCodeInt.Should().Be(202);
        emailData.SendGridStatusCodeString.Should().Be("Accepted");
    }

    [TestMethod]
    public async Task DotFlyerService_ShouldSendSMS()
    {
        var randomGuid = Guid.NewGuid();

        SMSMessage smsMessage = new()
        {
            Body = $"Test SMS body {randomGuid}",
            To = _receiverNumber!,
            Tags = new Dictionary<string, string>()
            {
                { "TestName", "DotFlyer Service Integration Test" },
                { "TestRandomGuid", randomGuid.ToString() }
            }
        };

        ServiceBusMessage message = new(Encoding.UTF8.GetBytes(JsonSerializer.Serialize(smsMessage)));

        await _smsServiceBusSender!.SendMessageAsync(message);

        SMSData smsData = await _cslQueryProvider!
            .WaitSingleQueryResult<SMSData>($"[\"{SMSTable.Instance.TableName}\"] | where Body == \"{smsMessage.Body}\"", TimeSpan.FromMinutes(5), _cancellationToken);

        smsData.Should().NotBeNull();
        smsData.Body.Should().Be(smsMessage.Body);
        smsData.To.Should().Be(smsMessage.To);
        smsData.TwilioStatusCodeInt.Should().Be(201);
        smsData.TwilioStatusCodeString.Should().Be("Created");
        smsData.Tags.Should().Be(JsonSerializer.Serialize(smsMessage.Tags));
    }

    [ClassCleanup]
    public static async Task ClassCleanup()
    {
        if (_emailServiceBusSender != null) await _emailServiceBusSender.DisposeAsync();
        if (_smsServiceBusSender != null) await _smsServiceBusSender.DisposeAsync();
        if (_serviceBusClient != null) await _serviceBusClient.DisposeAsync();
        _httpClient?.Dispose();
        _cslQueryProvider?.Dispose();
    }
}
