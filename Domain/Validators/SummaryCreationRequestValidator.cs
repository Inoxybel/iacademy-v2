using Domain.DTO.Summary;
using FluentValidation;

namespace Domain.Validators;

public class SummaryCreationRequestValidator : AbstractValidator<SummaryCreationRequest>
{
    public SummaryCreationRequestValidator()
    {
        RuleFor(request => request.Theme)
            .NotEmpty().WithMessage("The theme is required.")
            .MinimumLength(3).WithMessage("The theme must be more than 3 characters.")
            .MaximumLength(100).WithMessage("The theme cannot exceed 100 characters.");

        RuleFor(request => request.Category)
            .NotEmpty().WithMessage("The category is required.")
            .MinimumLength(3).WithMessage("The category must be more than 3 characters.")
            .MaximumLength(100).WithMessage("The category cannot exceed 100 characters.");

        RuleFor(request => request.Subcategory)
            .NotEmpty().WithMessage("The subcategory is required.")
            .MinimumLength(3).WithMessage("The subcategory must be more than 3 characters.")
            .MaximumLength(100).WithMessage("The subcategory cannot exceed 100 characters.");

        RuleFor(request => request.ConfigurationId)
            .NotEmpty().WithMessage("The configuration ID is required.")
            .Length(36).WithMessage("The configuration ID must be 36 characters long.");

        RuleFor(request => request.OwnerId)
            .NotEmpty().WithMessage("The owner ID is required.")
            .Length(36).WithMessage("The owner ID must be 36 characters long.");

        RuleFor(x => x.Icon)
            .NotEmpty().WithMessage("The SVG Icon string is required.");
    }
}

