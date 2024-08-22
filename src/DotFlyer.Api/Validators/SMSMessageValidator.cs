namespace DotFlyer.Api.Validators;

public class SMSMessageValidator : AbstractValidator<SMSMessage>
{
    public SMSMessageValidator()
    {
        RuleFor(x => x.To).NotEmpty().WithMessage("'To' field is required");
        RuleFor(x => x.Body).NotEmpty().WithMessage("'Body' field is required");
    }
}
