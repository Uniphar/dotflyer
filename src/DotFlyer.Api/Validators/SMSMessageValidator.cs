namespace DotFlyer.Api.Validators;

public class SMSMessageValidator : AbstractValidator<SMSMessage>
{
    public SMSMessageValidator()
    {
        RuleFor(x => x.To)
            .NotEmpty().WithMessage("'To' field is required")
            .CustomAsync(async (to, context, cancellationToken) =>
            {
                if ((await PhoneNumberResource.FetchAsync(to)).Valid is false)
                {
                    context.AddFailure("'To' field should be a valid phone number in E.164 format");
                }
            });


        RuleFor(x => x.Body).NotEmpty().WithMessage("'Body' field is required");
    }
}
