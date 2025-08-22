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
    [ProducesDefaultResponseType]
    public async Task<IActionResult> PostSMS(
        [FromBody] SMSMessage smsMessage,
        [FromServices] SmsTopicSender smsTopicSender,
        [FromServices] IValidator<SMSMessage> validator,
        CancellationToken cancellationToken)
    {
        var validationResult = await validator.ValidateAsync(smsMessage, cancellationToken);
        if (!validationResult.IsValid)
        {
            var errors = validationResult.Errors
                .GroupBy(e => e.PropertyName)
                .ToDictionary(
                    g => g.Key,
                    g => g.Select(e => e.ErrorMessage).ToArray()
                );

            return BadRequest(new { errors });
        }

        await smsTopicSender.SendMessageAsync(smsMessage, cancellationToken);
        return Ok();
    }

    [HttpPost("email", Name = "PostEmail")]
    [Authorize(Policy = "AllOrEmail")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesDefaultResponseType]
    public async Task<IActionResult> PostEmail(
        [FromBody] EmailMessage emailMessage,
        [FromServices] EmailTopicSender emailTopicSender,
        [FromServices] IValidator<EmailMessage> validator,  
        CancellationToken cancellationToken)
    {
        var validationResult = await validator.ValidateAsync(emailMessage, cancellationToken);
        if (!validationResult.IsValid)
        {
            var errors = validationResult.Errors
                .GroupBy(e => e.PropertyName)
                .ToDictionary(
                    g => g.Key,
                    g => g.Select(e => e.ErrorMessage).ToArray()
                );

            return BadRequest(new { errors });
        }

        await emailTopicSender.SendMessageAsync(emailMessage, cancellationToken);
        return Ok();
    }
}
