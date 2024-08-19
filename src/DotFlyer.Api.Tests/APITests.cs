﻿namespace DotFlyer.Api.Tests;

[TestClass, TestCategory("Integration")]
public class APITests
{
    private static string? _apiHost;
    private static string? _instance;
    private static string? _scope;

    private static CancellationToken _cancellationToken;
    private static SecretClient? _secretClient;
    private static ICslQueryProvider? _cslQueryProvider;

    private static string? _smsReceiverNumber;

    private static string? _senderEmail;
    private static string? _receiverEmail;

    private static readonly string _senderRoleClientIdKeyVaultName = "integration-test-dotflyer-api-dotflyer-sender-all-client-id";
    private static readonly string _senderRoleClientSecretKeyVaultName = "integration-test-dotflyer-api-dotflyer-sender-all-client-secret";
    private static readonly string _smsSenderRoleClientIdKeyVaultName = "integration-test-dotflyer-api-dotflyer-sender-sms-client-id";
    private static readonly string _smsSenderRoleClientSecretKeyVaultName = "integration-test-dotflyer-api-dotflyer-sender-sms-client-secret";
    private static readonly string _emailSenderRoleClientIdKeyVaultName = "integration-test-dotflyer-api-dotflyer-sender-email-client-id";
    private static readonly string _emailSenderRoleClientSecretKeyVaultName = "integration-test-dotflyer-api-dotflyer-sender-email-client-secret";

    [ClassInitialize]
    public static async Task ClassInitialize(TestContext context)
    {
        _cancellationToken = context.CancellationTokenSource.Token;

        DefaultAzureCredential credential = new();

        _apiHost = Environment.GetEnvironmentVariable("API_HOST");
        _instance = $"https://login.microsoftonline.com/{Environment.GetEnvironmentVariable("AZURE_ENTRA_EXTERNAL_TENANT_ID")}";

        _secretClient = new(new($"https://{Environment.GetEnvironmentVariable("AZURE_KEY_VAULT_NAME")}.vault.azure.net/"), credential);

        var appClientId = (await _secretClient!.GetSecretAsync("dotflyer-api-client-id", cancellationToken: _cancellationToken)).Value.Value;
        _scope = $"api://dotflyer-api/{appClientId}/.default";

        var _adxHostAddress = (await _secretClient.GetSecretAsync("AzureDataExplorer--HostAddress", cancellationToken: _cancellationToken)).Value.Value;

        var kcsb = new KustoConnectionStringBuilder(_adxHostAddress, "devops")
            .WithAadTokenProviderAuthentication(async () =>
                (await credential.GetTokenAsync(new(["https://kusto.kusto.windows.net/.default"]), cancellationToken: _cancellationToken)).Token);

        _cslQueryProvider = KustoClientFactory.CreateCslQueryProvider(kcsb);

        _smsReceiverNumber = (await _secretClient.GetSecretAsync("integration-test-dotflyer-receiver-number", cancellationToken: _cancellationToken)).Value.Value;

        _senderEmail = (await _secretClient.GetSecretAsync("integration-test-dotflyer-sender", cancellationToken: _cancellationToken)).Value.Value;
        _receiverEmail = (await _secretClient.GetSecretAsync("integration-test-dotflyer-receiver", cancellationToken: _cancellationToken)).Value.Value;
    }

    [TestMethod]
    public async Task Post_SMS_ShouldReturn_401_When_NoTokenProvided()
    {
        SMSMessage smsMessage = new()
        {
            To = _smsReceiverNumber,
            Body = Guid.NewGuid().ToString()
        };

        using var httpClient = GetHttpClient();

        var response = await httpClient.PostAsync("dotflyer/sms", GetStringContent(smsMessage), _cancellationToken);

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [TestMethod]
    public async Task Post_SMS_ShouldReturn_403_When_EmailSenderRoleAndPayloadIsValid()
    {
        SMSMessage smsMessage = new()
        {
            To = _smsReceiverNumber,
            Body = Guid.NewGuid().ToString()
        };

        using var httpClient = GetHttpClient(await GetEmailSenderAccessTokenAsync());

        var response = await httpClient.PostAsync("dotflyer/sms", GetStringContent(smsMessage), _cancellationToken);

        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [TestMethod]
    public async Task Post_SMS_ShouldReturn_200_When_SMSSenderRoleAndPayloadIsValid()
    {
        SMSMessage smsMessage = new()
        {
            To = _smsReceiverNumber,
            Body = Guid.NewGuid().ToString(),
            Tags = new Dictionary<string, string>()
            {
                { "TestName", "DotFlyer API Integration Test" }
            }
        };

        using var httpClient = GetHttpClient(await GetSMSSenderAccessTokenAsync());

        var response = await httpClient.PostAsync("dotflyer/sms", GetStringContent(smsMessage), _cancellationToken);

        response.StatusCode.Should().Be(HttpStatusCode.OK);

        SMSData smsData = await _cslQueryProvider!
            .WaitSingleQueryResult<SMSData>($"[\"{SMSTable.Instance.TableName}\"] | where Body == \"{smsMessage.Body}\"", TimeSpan.FromMinutes(10), _cancellationToken);

        smsData.Should().NotBeNull();
        smsData.Body.Should().Be(smsMessage.Body);
        smsData.To.Should().Be(smsMessage.To);
        smsData.TwilioStatusCodeInt.Should().Be(201);
        smsData.TwilioStatusCodeString.Should().Be("Created");
        smsData.Tags.Should().Be(JsonSerializer.Serialize(smsMessage.Tags));
    }

    [TestMethod]
    public async Task Post_SMS_ShouldReturn_200_When_SenderRoleAndPayloadIsValid()
    {
        SMSMessage smsMessage = new()
        {
            To = _smsReceiverNumber,
            Body = Guid.NewGuid().ToString(),
            Tags = new Dictionary<string, string>()
            {
                { "TestName", "DotFlyer API Integration Test" }
            }
        };

        using var httpClient = GetHttpClient(await GetSenderAccessTokenAsync());

        var response = await httpClient.PostAsync("dotflyer/sms", GetStringContent(smsMessage), _cancellationToken);

        response.StatusCode.Should().Be(HttpStatusCode.OK);

        SMSData smsData = await _cslQueryProvider!
            .WaitSingleQueryResult<SMSData>($"[\"{SMSTable.Instance.TableName}\"] | where Body == \"{smsMessage.Body}\"", TimeSpan.FromMinutes(10), _cancellationToken);

        smsData.Should().NotBeNull();
        smsData.Body.Should().Be(smsMessage.Body);
        smsData.To.Should().Be(smsMessage.To);
        smsData.TwilioStatusCodeInt.Should().Be(201);
        smsData.TwilioStatusCodeString.Should().Be("Created");
        smsData.Tags.Should().Be(JsonSerializer.Serialize(smsMessage.Tags));
    }

    [TestMethod]
    public async Task Post_SMS_ShouldReturn_400_When_SMSSenderRoleAndPayloadIsNotValid()
    {
        var smsMessage = new { };

        using var httpClient = GetHttpClient(await GetSMSSenderAccessTokenAsync());

        var response = await httpClient.PostAsync("dotflyer/sms", GetStringContent(smsMessage), _cancellationToken);

        var responseContent = await response.Content.ReadAsStringAsync();

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        responseContent.Should().Contain("'To' field is required");
        responseContent.Should().Contain("'Body' field is required");
    }

    [TestMethod]
    public async Task Post_Email_ShouldReturn_200_When_SenderRoleAndPayloadIsValid()
    {
        EmailMessage emailMessage = new()
        {
            Subject = "DotFlyer API Test Automation",
            Body = Guid.NewGuid().ToString(),
            From = new()
            {
                Email = _senderEmail,
                Name = "Integration Test"
            },
            To = [
                new()
                {
                    Email = _receiverEmail,
                    Name = "Integration Test Destination Address"
                }
            ],
            Tags = new Dictionary<string, string>()
            {
                { "TestName", "DotFlyer API Integration Test" }
            }
        };

        using var httpClient = GetHttpClient(await GetSenderAccessTokenAsync());

        var response = await httpClient.PostAsync("dotflyer/email", GetStringContent(emailMessage), _cancellationToken);

        response.StatusCode.Should().Be(HttpStatusCode.OK);

        EmailData emailData = await _cslQueryProvider!
            .WaitSingleQueryResult<EmailData>($"[\"{EmailTable.Instance.TableName}\"] | where Body == \"{emailMessage.Body}\"", TimeSpan.FromMinutes(10), _cancellationToken);

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

    public static HttpClient GetHttpClient(string? token = null)
    {
        HttpClient httpClient = new() { BaseAddress = new($"https://{_apiHost}") };

        if (token != null) httpClient.DefaultRequestHeaders.Authorization = new("Bearer", token);

        return httpClient;
    }

    public static StringContent GetStringContent(object content) => new(JsonSerializer.Serialize(content), Encoding.UTF8, "application/json");

    public static async Task<string> GetSenderAccessTokenAsync() =>
        await GetAccessTokenAsync(_senderRoleClientIdKeyVaultName, _senderRoleClientSecretKeyVaultName);

    public static async Task<string> GetSMSSenderAccessTokenAsync() =>
        await GetAccessTokenAsync(_smsSenderRoleClientIdKeyVaultName, _smsSenderRoleClientSecretKeyVaultName);

    public static async Task<string> GetEmailSenderAccessTokenAsync() =>
        await GetAccessTokenAsync(_emailSenderRoleClientIdKeyVaultName, _emailSenderRoleClientSecretKeyVaultName);

    public static async Task<string> GetAccessTokenAsync(string clientId, string clientSecret)
    {
        var clientIdValue = (await _secretClient!.GetSecretAsync(clientId, cancellationToken: _cancellationToken)).Value.Value;
        var clientSecretValue = (await _secretClient!.GetSecretAsync(clientSecret, cancellationToken: _cancellationToken)).Value.Value;

        var result = await ConfidentialClientApplicationBuilder
                            .Create(clientIdValue)
                            .WithClientSecret(clientSecretValue)
                            .WithAuthority(_instance)
                            .Build()
                            .AcquireTokenForClient([_scope!])
                            .ExecuteAsync();

        return result.AccessToken;
    }
}