using Domain.DTO.Summary;
using FluentValidation;

namespace Domain.Validators;

public class SummaryRequestValidator : AbstractValidator<SummaryRequest>
{
    public SummaryRequestValidator()
    {
        RuleFor(x => x.OriginId)
            .Length(36).When(x => !string.IsNullOrWhiteSpace(x.OriginId))
            .WithMessage("The Origin ID must be 36 characters long if provided.");

        RuleFor(x => x.OwnerId)
            .NotEmpty().WithMessage("The Owner ID is required.")
            .Length(36).WithMessage("The Owner ID must be 36 characters long.");

        RuleFor(x => x.ChatId);

        RuleFor(x => x.ConfigurationId)
            .NotEmpty().WithMessage("The Configuration ID is required.")
            .Length(36).WithMessage("The Configuration ID must be 36 characters long.");

        RuleFor(x => x.IsAvaliable)
            .NotNull().WithMessage("The IsAvaliable cannot be null.");

        RuleFor(x => x.Category)
            .NotEmpty().WithMessage("The Category is required.")
            .MaximumLength(100).WithMessage("The Category cannot exceed 100 characters.");

        RuleFor(x => x.Subcategory)
            .NotEmpty().WithMessage("The Subcategory is required.")
            .MaximumLength(100).WithMessage("The Subcategory cannot exceed 100 characters.");

        RuleFor(x => x.Theme)
            .NotEmpty().WithMessage("The Theme is required.")
            .MaximumLength(100).WithMessage("The Theme cannot exceed 100 characters.");

        RuleFor(x => x.Topics)
            .NotEmpty().WithMessage("Topics are required.")
            .Must(list => list.Count > 0).WithMessage("There must be at least one topic.")
            .ForEach(rule => rule.SetValidator(new TopicValidator()));
    }
}
