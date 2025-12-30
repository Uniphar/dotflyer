namespace DotFlyer.Service.Tests;

using Microsoft.Extensions.Logging.Abstractions;

[TestClass, TestCategory("Unit")]
public class EmailSenderTests
{
    private Mock<DefaultAzureCredential> _credentialMock;
    private Mock<ISendGridClient> _sendGridClientMock;
    private Mock<ITelemetryChannel> _telemetryChannelMock;
    private Mock<IAzureDataExplorerClient> _azureDataExplorerClientMock;

    private EmailMessage? _emailMessage;

    private readonly EmailSender _emailSender;

    public EmailSenderTests()
    {
        _credentialMock = new Mock<DefaultAzureCredential>();
        _sendGridClientMock = new Mock<ISendGridClient>();
        _telemetryChannelMock = new Mock<ITelemetryChannel>();
        _azureDataExplorerClientMock = new Mock<IAzureDataExplorerClient>();

        var serviceProviderMock = new Mock<IServiceProvider>();
        var emailHtmlRenderer = new EmailHtmlRenderer(serviceProviderMock.Object, NullLogger<EmailHtmlRenderer>.Instance);

        _emailSender = new(_credentialMock.Object, _sendGridClientMock.Object, new TelemetryClient(new() { TelemetryChannel = _telemetryChannelMock.Object }), _azureDataExplorerClientMock.Object, emailHtmlRenderer);
    }

    [TestInitialize]
    public void TestInitialize()
    {
        _emailMessage = new()
        {
            Body = "Test email body",
            Subject = "Test email subject",
            From = new()
            {
                Email = "unit@test.io",
                Name = "Unit Test"
            },
            To =
            [
                new() { Name = "Test", Email = "test@test.io"}
            ]
        };
    }

    [TestMethod]
    public async Task EmailSender_ShouldNotThrowArgumentException_WhenResponseIsBadRequest()
    {
        _sendGridClientMock
            .Setup(x => x.SendEmailAsync(It.IsAny<SendGridMessage>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new SendGrid.Response(HttpStatusCode.BadRequest, new StringContent("Bad Request"), null));

        await _emailSender.SendAsync(_emailMessage!);
    }

    [TestMethod]
    public async Task EmailSender_ShouldThrowHttpRequestException_WhenResponseIsNotAcceptedOrBadRequest()
    {
        _sendGridClientMock
            .Setup(x => x.SendEmailAsync(It.IsAny<SendGridMessage>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new SendGrid.Response(HttpStatusCode.Unauthorized, new StringContent("Unauthorized"), null));

        await Assert.ThrowsAsync<HttpRequestException>(() => _emailSender.SendAsync(_emailMessage!));
    }
}
