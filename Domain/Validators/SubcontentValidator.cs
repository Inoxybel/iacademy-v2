using Domain.Entities.Contents;
using FluentValidation;

namespace Domain.Validators;
public class SubcontentValidator : AbstractValidator<Subcontent>
    {
        public SubcontentValidator()
        {
            RuleFor(x => x.SubcontentHistory)
                .ForEach(rule => rule.SetValidator(new SubcontentHistoryValidator()));
        }
    }
