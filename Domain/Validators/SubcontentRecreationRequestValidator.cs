using Domain.DTO.Content;
using FluentValidation;

namespace Domain.Validators;

public class SubcontentRecreationRequestValidator : AbstractValidator<SubcontentRecreationRequest>
{
    public SubcontentRecreationRequestValidator()
    {
        RuleFor(x => x.SubcontentIndex)
            .GreaterThanOrEqualTo(0).WithMessage("The Subcontent Index must be greater than or equal to 0");
    }
}
