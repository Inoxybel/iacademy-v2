using Domain.DTO.Content;
using FluentValidation;

namespace Domain.Validators;

public class ContentRequestValidator : AbstractValidator<ContentRequest>
{
    public ContentRequestValidator()
    {
        RuleFor(x => x.OwnerId)
            .NotEmpty().WithMessage("The Owner ID is required.")
            .Length(36).WithMessage("The Owner ID must be 36 characters long.");

        RuleFor(x => x.SummaryId)
            .NotEmpty().WithMessage("The Summary ID is required.")
            .Length(36).WithMessage("The Summary ID must be 36 characters long.");

        RuleFor(x => x.ConfigurationId)
            .NotEmpty().WithMessage("The Configuration ID is required.")
            .Length(36).WithMessage("The Configuration ID must be 36 characters long.");

        RuleFor(x => x.ExerciseId)
            .Length(36).WithMessage("The ExerciseId, if provided, must be 36 characters long.")
            .When(x => !string.IsNullOrEmpty(x.ExerciseId));

        RuleFor(x => x.Theme)
            .NotEmpty().WithMessage("The Theme is required.")
            .MaximumLength(100).WithMessage("The Theme cannot be more than 100 characters long.");

        RuleFor(x => x.SubtopicIndex)
            .NotEmpty().WithMessage("The Subtopic Index is required.")
            .MaximumLength(3).WithMessage("The SubtopicIndex cannot exceed 3 characters.")
            .Matches("^[0-9.]+$").WithMessage("The SubtopicIndex can only contain numeric characters.");

        RuleFor(x => x.Title)
            .NotEmpty().WithMessage("The Title is required.")
            .MaximumLength(200).WithMessage("The Title cannot be more than 200 characters long.");

        RuleFor(x => x.Body)
            .NotEmpty().WithMessage("The Body is required.")
            .Must(x => x != null && x.Count > 0).WithMessage("The Body must contain at least one item.")
            .ForEach(rule => rule.SetValidator(new BodyValidator()));
    }
}
