namespace DotFlyer.Service.Tests;

[TestClass, TestCategory("Unit")]
public class SendGridEmailSenderTests
{
    private Mock<ISendGridClient> _sendGridClientMock;
    private Mock<IAzureDataExplorerClient> _azureDataExplorerClientMock;

    private EmailMessage? _emailMessage;

    private SendGridEmailSender _sendGridEmailSender;

    public SendGridEmailSenderTests()
    {
        _sendGridClientMock = new Mock<ISendGridClient>();
        _azureDataExplorerClientMock = new Mock<IAzureDataExplorerClient>();

        _sendGridEmailSender = new(_sendGridClientMock.Object, _azureDataExplorerClientMock.Object);
    }

    [TestInitialize]
    public void TestInitialize()
    {
        _emailMessage = new()
        {
            Body = "Test email body",
            Subject = "Test email subject",
            FromEmail = "unit@test.io",
            FromName = "Unit Test",
            To =
            [
                new() { Name = "Test", Email = "test@test.io"}
            ]
        };
    }

    [TestMethod]
    public async Task SendGridEmailSender_ShouldThrowArgumentException_WhenEmailMessageToIsEmpty()
    {
        _emailMessage!.To = [];

        await Assert.ThrowsExceptionAsync<ArgumentException>(() => _sendGridEmailSender.SendAsync(_emailMessage!));
    }

    [TestMethod]
    public async Task SendGridEmailSender_ShouldThrowHttpRequestException_WhenResponseIsNotAccepted()
    {
        _sendGridClientMock
            .Setup(x => x.SendEmailAsync(It.IsAny<SendGridMessage>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new Response(HttpStatusCode.BadRequest, new StringContent("Bad Request"), null));

        await Assert.ThrowsExceptionAsync<HttpRequestException>(() => _sendGridEmailSender.SendAsync(_emailMessage!));
    }
}
