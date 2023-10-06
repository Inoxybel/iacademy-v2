using Domain.Entities.Exercise;
using FluentValidation;

namespace Domain.Validators;

public class ActivityValidator : AbstractValidator<Activity>
{
    public ActivityValidator()
    {
        RuleFor(x => x.Identification)
            .GreaterThanOrEqualTo(1).WithMessage("Identification must be greater than or equal to 1.");

        RuleFor(x => x.Type)
            .NotNull().WithMessage("Activity Type is required.")
            .IsInEnum().WithMessage("Type is not a valid option.");

        RuleFor(x => x.Question)
            .NotEmpty().WithMessage("Question is required.")
            .MaximumLength(1000).WithMessage("Question cannot exceed 1000 characters.");
    }
}
