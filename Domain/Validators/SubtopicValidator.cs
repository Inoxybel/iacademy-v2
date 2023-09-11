﻿using Domain.Entities;
using FluentValidation;

namespace Domain.Validators;

public class SubtopicValidator : AbstractValidator<Subtopic>
{
    public SubtopicValidator()
    {
        RuleFor(x => x.Index)
            .NotEmpty().WithMessage("The Index is required.")
            .Matches("^[0-9]+$").WithMessage("The Subtopic Index can only contain numeric characters.")
            .MaximumLength(2).WithMessage("The Subtopic Index cannot exceed 2 characters.");

        RuleFor(x => x.Title)
            .NotEmpty().WithMessage("The Title is required.")
            .MaximumLength(200).WithMessage("The Title cannot exceed 200 characters.");

        RuleFor(x => x.ContentId)
            .Length(36).WithMessage("The ContentId, if provided, must be 36 characters long.")
            .When(x => !string.IsNullOrEmpty(x.ContentId));
    }
}