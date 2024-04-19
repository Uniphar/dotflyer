namespace DotFlyer.Service.Tests;

[TestClass, TestCategory("Unit")]
public class EmailSenderTests
{
    private Mock<ISendGridClient> _sendGridClientMock;
    private Mock<IAzureDataExplorerClient> _azureDataExplorerClientMock;

    private EmailMessage? _emailMessage;

    private EmailSender _emailSender;

    public EmailSenderTests()
    {
        _sendGridClientMock = new Mock<ISendGridClient>();
        _azureDataExplorerClientMock = new Mock<IAzureDataExplorerClient>();

        _emailSender = new(_sendGridClientMock.Object, _azureDataExplorerClientMock.Object);
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
    public async Task EmailSender_ShouldThrowArgumentException_WhenEmailMessageToIsEmpty()
    {
        _emailMessage!.To = [];

        await Assert.ThrowsExceptionAsync<ArgumentException>(() => _emailSender.SendAsync(_emailMessage!));
    }

    [TestMethod]
    public async Task EmailSender_ShouldThrowHttpRequestException_WhenResponseIsNotAccepted()
    {
        _sendGridClientMock
            .Setup(x => x.SendEmailAsync(It.IsAny<SendGridMessage>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new Response(HttpStatusCode.BadRequest, new StringContent("Bad Request"), null));

        await Assert.ThrowsExceptionAsync<HttpRequestException>(() => _emailSender.SendAsync(_emailMessage!));
    }
}
