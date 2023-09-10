using Domain.Entities;
using FluentValidation;

namespace Domain.Validators;

public class TopicValidator : AbstractValidator<Topic>
{
    public TopicValidator()
    {
        RuleFor(x => x.Index)
            .NotEmpty().WithMessage("The Index is required.")
            .Matches("^[0-9]+$").WithMessage("The Topic Index can only contain numeric characters.")
            .MaximumLength(2).WithMessage("The Index cannot exceed 2 characters.");

        RuleFor(x => x.Title)
            .NotEmpty().WithMessage("The Title is required.")
            .MaximumLength(200).WithMessage("The Title cannot exceed 200 characters.");

        RuleFor(x => x.Description)
            .NotEmpty().WithMessage("The Description is required.")
            .MaximumLength(500).WithMessage("The Description cannot exceed 500 characters.");

        RuleFor(x => x.Subtopics)
            .NotEmpty().WithMessage("Subtopics are required.")
            .Must(list => list.Count > 0).WithMessage("There must be at least one subtopic.")
            .ForEach(rule => rule.SetValidator(new SubtopicValidator()));
    }
}
