using Domain.DTO.Content;
using FluentValidation;

namespace Domain.Validators;

public class AIContentCreationRequestValidator : AbstractValidator<AIContentCreationRequest>
{
    public AIContentCreationRequestValidator()
    {
        RuleFor(request => request.SubtopicIndex)
            .NotEmpty().WithMessage("The Topic Index is required.")
            .Matches("^[0-9.]+$").WithMessage("The Topic Index can only contain numeric characters.");
    }
}
