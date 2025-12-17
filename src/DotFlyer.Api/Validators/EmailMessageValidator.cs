namespace DotFlyer.Api.Validators;

public class EmailMessageValidator : AbstractValidator<EmailMessage>
{
    public EmailMessageValidator()
    {
        RuleFor(x => x.Subject).NotEmpty().WithMessage("'Subject' field is required");

        // Either Body (non-empty) or a non-string TemplateModel must be provided
        RuleFor(x => x).Custom((email, context) =>
        {
            bool hasBody = !string.IsNullOrWhiteSpace(email.Body);
            bool hasTemplateModel = email.TemplateModel != null && !(email.TemplateModel is string);

            if (!hasBody && !hasTemplateModel)
            {
                context.AddFailure("Either 'Body' or 'TemplateModel' must be provided.");
            }
        });

        RuleFor(x => x.From).NotEmpty().WithMessage("'From' field is required");
        RuleFor(x => x.From).SetValidator(new EmailMessageContactValidator());
        RuleFor(x => x.To).Must(x => x != null && x.Any()).WithMessage("'To' field is required and should contain at least one contact");
        RuleForEach(x => x.To).SetValidator(new EmailMessageContactValidator());
    }
}

public class EmailMessageContactValidator : AbstractValidator<Contact>
{
    public EmailMessageContactValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("'Email' field is required")
            .EmailAddress().WithMessage("'Email' field should be a valid email address");

        RuleFor(x => x.Name).NotEmpty().WithMessage("'Name' field is required");
    }
}