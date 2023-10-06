using Domain.Entities.Contents;
using FluentValidation;

namespace Domain.Validators;

public class BodyValidator : AbstractValidator<Body>
{
    public BodyValidator()
    {
        RuleFor(x => x.Contents)
            .NotEmpty().WithMessage("The Contents is required.")
            .Must(x => x != null && x.Count > 0).WithMessage("The Contents must contain at least one item.")
            .ForEach(rule => rule.SetValidator(new SubcontentValidator()));
    }
}
