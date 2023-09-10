using Domain.Entities;
using FluentValidation;

namespace Domain.Validators;

public class BodyValidator : AbstractValidator<Body>
{
    public BodyValidator()
    {
        RuleFor(x => x.Content)
            .NotEmpty().WithMessage("The Content is required.")
            .MinimumLength(100).WithMessage("The Content cannot be less than 100 characters long.")
            .MaximumLength(5000).WithMessage("The Content cannot be more than 5000 characters long.");

        RuleFor(x => x.CreatedDate)
            .NotEmpty().WithMessage("The Created Date is required.");

        RuleFor(x => x.DisabledDate)
            .Must((body, date) => date == default || date > body.CreatedDate)
            .WithMessage("The Disabled Date must be later than the Created Date, if provided.");
    }
}
