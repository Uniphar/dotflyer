namespace DotFlyer.Api.Validators;

public class EmailMessageValidator : AbstractValidator<EmailMessage>
{
    public EmailMessageValidator()
    {
        // TODO : Add validation rules
        RuleFor(x => x.To).NotEmpty().WithMessage("'To' field is required");
        RuleFor(x => x.Body).NotEmpty().WithMessage("'Body' field is required");
    }
}
