using Microsoft.AspNetCore.Authorization;

namespace DotFlyer.Api.Controllers;

[ApiController]
[Route("dotflyer")]
public class DotFlyerController(SmsTopicSender smsTopicSender, EmailTopicSender emailTopicSender) : ControllerBase
{
    [HttpPost("sms", Name = "PostSMS")]
    [Authorize(Policy = "AllOrSMS")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesDefaultResponseType]
    public async Task<IActionResult> PostSMS([FromBody] SMSMessage smsMessage, CancellationToken cancellationToken)
    {
        await smsTopicSender.SendMessageAsync(smsMessage, cancellationToken);
        return Ok();
    }

    [HttpPost("email", Name = "PostEmail")]
    [Authorize(Policy = "AllOrEmail")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesDefaultResponseType]
    public async Task<IActionResult> PostEmail([FromBody] EmailMessage emailMessage, CancellationToken cancellationToken)
    {
        await emailTopicSender.SendMessageAsync(emailMessage, cancellationToken);
        return Ok();
    }
}
