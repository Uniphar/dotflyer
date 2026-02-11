namespace DotFlyer.Service.Tests;

[TestClass, TestCategory("Unit")]
public class SMSSenderTests
{
    private Mock<ITwilioRestClient> _twilioRestClientMock;
    private Mock<IAzureDataExplorerClient> _azureDataExplorerClientMock;

    private SMSMessage? _smsMessage;

    private SMSSender _smsSender;

    public SMSSenderTests()
    {
        _twilioRestClientMock = new Mock<ITwilioRestClient>();
        _azureDataExplorerClientMock = new Mock<IAzureDataExplorerClient>();

        SMSSenderConfiguration config = new()
        {
            AccountSID = "AC1234567890",
            ApiKeySID = "SK123456",
            ApiKeySecret = "AS123456",
            FromPhoneNumber = "+1234567890"
        };

        var telemetryConfiguration = new TelemetryConfiguration
        {
            ConnectionString = "InstrumentationKey=00000000-0000-0000-0000-000000000000",
            DisableTelemetry = true
        };
        var telemetryClient = new TelemetryClient(telemetryConfiguration);

        _smsSender = new(config, _twilioRestClientMock.Object, telemetryClient, _azureDataExplorerClientMock.Object);
    }

    [TestInitialize]
    public void TestInitialize()
    {
        _smsMessage = new()
        {
            Body = "Test SMS body",
            To = "+1987654321"
        };
    }

    [TestMethod]
    public async Task SMSSender_ShouldNotThrowHttpRequestException_WhenResponseIsBadRequest()
    {
        Mock<Twilio.Http.HttpClient> httpClientMock = new();

        httpClientMock
            .Setup(h => h.MakeRequestAsync(It.IsAny<Request>()))
            .ReturnsAsync(new Twilio.Http.Response(HttpStatusCode.BadRequest, "Bad Request", null));

        _twilioRestClientMock
            .SetupGet(x => x.HttpClient)
            .Returns(httpClientMock.Object);

        await _smsSender.SendAsync(_smsMessage!);
    }

    [TestMethod]
    public async Task SMSSender_ShouldThrowHttpRequestException_WhenResponseIsNotAcceptedOrBadRequest()
    {
        Mock<Twilio.Http.HttpClient> httpClientMock = new();

        httpClientMock
            .Setup(h => h.MakeRequestAsync(It.IsAny<Request>()))
            .ReturnsAsync(new Twilio.Http.Response(HttpStatusCode.Unauthorized, "Unauthorized", null));

        _twilioRestClientMock
            .SetupGet(x => x.HttpClient)
            .Returns(httpClientMock.Object);

        await Assert.ThrowsAsync<HttpRequestException>(() => _smsSender.SendAsync(_smsMessage!));
    }
}
