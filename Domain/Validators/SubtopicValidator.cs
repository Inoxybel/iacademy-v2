using Domain.Entities;
using FluentValidation;

namespace Domain.Validators;

public class SubtopicValidator : AbstractValidator<Subtopic>
{
    public SubtopicValidator()
    {
        RuleFor(x => x.Index)
            .NotEmpty().WithMessage("The Index is required.")
            .Matches("^[0-9.]+$").WithMessage("The Subtopic Index can only contain numeric characters.")
            .MaximumLength(3).WithMessage("The Subtopic Index cannot exceed 3 characters.");

        RuleFor(x => x.Title)
            .NotEmpty().WithMessage("The Title is required.")
            .MaximumLength(200).WithMessage("The Title cannot exceed 200 characters.");

        RuleFor(x => x.ContentId)
            .Length(36).When(x => !string.IsNullOrEmpty(x.ContentId))
            .WithMessage("The ContentId, if provided, must be 36 characters long.");

        RuleFor(x => x.ExerciseId)
            .Length(36).When(x => !string.IsNullOrEmpty(x.ExerciseId))
            .WithMessage("The ExerciseId, if provided, must be 36 characters long.");
    }
}
