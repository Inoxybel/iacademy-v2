using Domain.DTO.Correction;
using FluentValidation;

namespace Domain.Validators
{
    public class CorrectionUpdateRequestValidator : AbstractValidator<CorrectionUpdateRequest>
    {
        public CorrectionUpdateRequestValidator()
        {
            RuleFor(x => x.OwnerId)
                .NotEmpty().WithMessage("The Owner ID is required.")
                .Length(36).WithMessage("The Owner ID must be 36 characters long.");

            RuleFor(x => x.ExerciseId)
                .NotEmpty().WithMessage("The Exercise ID is required.")
                .Length(36).WithMessage("The Exercise ID must be 36 characters long.");

            RuleFor(x => x.Corrections)
                .NotEmpty().WithMessage("Corrections are required.")
                .Must(list => list.Count > 0).WithMessage("There must be at least one correction.")
                .ForEach(rule => rule.SetValidator(new CorrectionItemValidator()));
        }
    }
}
