using Domain.Entities.Configuration;
using FluentValidation;

namespace Domain.Validators;

public class InputPropertiesValidator : AbstractValidator<InputProperties>
{
    public InputPropertiesValidator()
    {
        RuleFor(x => x.InitialInput)
            .NotEmpty().WithMessage("The Initial Input is required.")
            .MaximumLength(1000).WithMessage("The Initial Input cannot be more than 1000 characters.");

        RuleFor(x => x.FinalInput)
            .NotEmpty().WithMessage("The Final Input is required.")
            .MaximumLength(1000).WithMessage("The Final Input cannot be more than 1000 characters.");
    }
}
