using CrossCutting.Enums;
using Domain.Entities.Exercise;
using Domain.Validators;
using FluentValidation.TestHelper;

namespace IAcademy.Test.Unit.Validators;

public class ActivityValidatorTests
{
    private readonly ActivityValidator _validator;

    public ActivityValidatorTests()
    {
        _validator = new ActivityValidator();
    }

    [Theory]
    [InlineData(0, false)]
    [InlineData(-1, false)]
    [InlineData(1, true)]
    public void ValidateIdentification(int identification, bool expected)
    {
        var model = new Activity
        {
            Identification = identification,
            Type = ActivityType.MultipleChoice,
            Question = "Valid question"
        };

        var result = _validator.TestValidate(model);

        if (expected)
        {
            result.ShouldNotHaveValidationErrorFor(x => x.Identification);
        }
        else
        {
            result.ShouldHaveValidationErrorFor(x => x.Identification);
        }
    }

    [Theory]
    [InlineData(ActivityType.MultipleChoice, true)]
    [InlineData(ActivityType.SingleChoice, true)]
    [InlineData(ActivityType.Code, true)]
    [InlineData((ActivityType)999, false)] 
    public void ValidateType(ActivityType type, bool expected)
    {
        var model = new Activity
        {
            Identification = 1,
            Type = type,
            Question = "Valid question"
        };

        var result = _validator.TestValidate(model);

        if (expected)
        {
            result.ShouldNotHaveValidationErrorFor(x => x.Type);
        }
        else
        {
            result.ShouldHaveValidationErrorFor(x => x.Type);
        }
    }

    [Theory]
    [InlineData("", false)]
    [InlineData(null, false)]
    [InlineData("Valid question", true)]
    [InlineData("1001", false)]
    public void ValidateQuestion(string question, bool expected)
    {
        if (string.Equals(question, "1001"))
            question = question.PadRight(1001);

        var model = new Activity
        {
            Identification = 1,
            Type = ActivityType.MultipleChoice,
            Question = question
        };

        var result = _validator.TestValidate(model);

        if (expected)
        {
            result.ShouldNotHaveValidationErrorFor(x => x.Question);
        }
        else
        {
            result.ShouldHaveValidationErrorFor(x => x.Question);
        }
    }
}

