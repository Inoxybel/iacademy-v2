using Domain.Entities.Feedback;
using FluentValidation;

namespace Domain.Validators;

public class CorrectionItemValidator : AbstractValidator<CorrectionItem>
{
    public CorrectionItemValidator()
    {
        RuleFor(x => x.Identification)
            .GreaterThanOrEqualTo(1).WithMessage("Identification must be greater than or equal to 1.");

        RuleFor(x => x.Question)
            .NotEmpty().WithMessage("Question is required.")
            .MaximumLength(2000).WithMessage("Question cannot exceed 2000 characters.");

        RuleFor(x => x.Answer)
            .NotEmpty().WithMessage("Answer is required.")
            .MaximumLength(2000).WithMessage("Answer cannot exceed 2000 characters.");

        RuleFor(x => x.IsCorrect)
            .NotNull().WithMessage("IsCorrect is required.");

        RuleFor(x => x.Feedback)
            .NotEmpty().WithMessage("Feedback is required.")
            .MaximumLength(2000).WithMessage("Feedback cannot exceed 2000 characters.");
    }
}
