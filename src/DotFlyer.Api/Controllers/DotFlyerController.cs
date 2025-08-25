using Microsoft.AspNetCore.Authorization;

namespace DotFlyer.Api.Controllers;

[ApiController]
[Route("dotflyer")]
public class DotFlyerController : ControllerBase
{
    [HttpPost("sms", Name = "PostSMS")]
    [Authorize(Policy = "AllOrSMS")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> PostSMS(
        [FromBody] SMSMessage smsMessage,
        [FromServices] SmsTopicSender smsTopicSender,
        [FromServices] IValidator<SMSMessage> validator,
        CancellationToken cancellationToken)
    {
        await smsTopicSender.SendMessageAsync(smsMessage, cancellationToken);
        return Ok();
    }

    [HttpPost("email", Name = "PostEmail")]
    [Authorize(Policy = "AllOrEmail")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> PostEmail(
        [FromBody] EmailMessage emailMessage,
        [FromServices] EmailTopicSender emailTopicSender,
        [FromServices] IValidator<EmailMessage> validator,  
        CancellationToken cancellationToken)
    {
        await emailTopicSender.SendMessageAsync(emailMessage, cancellationToken);
        return Ok();
    }
}
