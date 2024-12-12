namespace DotFlyer.Api.Validators;

public class SMSMessageValidator : AbstractValidator<SMSMessage>
{
    public SMSMessageValidator()
    {
        RuleFor(x => x.To).CustomAsync(async (to, context, cancellationToken) =>
        {
            if (string.IsNullOrWhiteSpace(to))
            {
                context.AddFailure("'To' field is required");
                return;
            }

            if ((await PhoneNumberResource.FetchAsync(to)).Valid is false)
            {
                context.AddFailure("'To' field should be a valid phone number in E.164 format");
            }
        });


        RuleFor(x => x.Body).NotEmpty().WithMessage("'Body' field is required");
    }
}
