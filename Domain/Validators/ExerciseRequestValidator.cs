using Domain.DTO.Exercise;
using FluentValidation;

namespace Domain.Validators;

public class ExerciseRequestValidator : AbstractValidator<ExerciseRequest>
{
    public ExerciseRequestValidator()
    {
        RuleFor(x => x.OwnerId)
            .NotEmpty().WithMessage("The Owner ID is required.")
            .Length(36).WithMessage("The Owner ID must be 36 characters long.");

        RuleFor(x => x.CorrectionId)
            .NotEmpty().WithMessage("The Correction ID is required.")
            .Length(36).WithMessage("The Correction ID must be 36 characters long.");

        RuleFor(x => x.ConfigurationId)
            .NotEmpty().WithMessage("The Configuration ID is required.")
            .Length(36).WithMessage("The Configuration ID must be 36 characters long.");

        RuleFor(x => x.Status)
            .NotNull().WithMessage("The Status must not be null.")
            .IsInEnum().WithMessage("Status is not a valid option.");

        RuleFor(x => x.Type)
            .NotNull().WithMessage("ExerciseType is required.")
            .IsInEnum().WithMessage("Type is not a valid option.");

        RuleFor(x => x.TopicIndex)
            .NotEmpty().WithMessage("The TopicIndex is required.")
            .Matches("^[0-9]+$").WithMessage("The TopicIndex can only contain numeric characters.")
            .MaximumLength(2).WithMessage("The TopicIndex cannot exceed 2 characters.");

        RuleFor(x => x.Title)
            .NotEmpty().WithMessage("The Title is required.")
            .MaximumLength(200).WithMessage("The Title cannot exceed 200 characters.");

        RuleFor(x => x.Exercises)
            .NotEmpty().WithMessage("Exercises are required.")
            .Must(list => list.Count > 0).WithMessage("There must be at least one activity.")
            .ForEach(rule => rule.SetValidator(new ActivityValidator()));
    }
}
