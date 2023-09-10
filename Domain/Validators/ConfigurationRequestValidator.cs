using Domain.DTO.Configuration;
using FluentValidation;

namespace Domain.Validators;

public class ConfigurationRequestValidator : AbstractValidator<ConfigurationRequest>
{
    public ConfigurationRequestValidator()
    {
        RuleFor(x => x.Summary)
            .SetValidator(new InputPropertiesValidator())
            .WithMessage("Invalid Summary properties.");

        RuleFor(x => x.FirstContent)
            .SetValidator(new InputPropertiesValidator())
            .WithMessage("Invalid First Content properties.");

        RuleFor(x => x.NewContent)
            .SetValidator(new InputPropertiesValidator())
            .WithMessage("Invalid New Content properties.");

        RuleFor(x => x.Exercise)
            .SetValidator(new InputPropertiesValidator())
            .WithMessage("Invalid Exercise properties.");

        RuleFor(x => x.Correction)
            .SetValidator(new InputPropertiesValidator())
            .WithMessage("Invalid Correction properties.");

        RuleFor(x => x.Pendency)
            .SetValidator(new InputPropertiesValidator())
            .WithMessage("Invalid Pendency properties.");
    }
}
