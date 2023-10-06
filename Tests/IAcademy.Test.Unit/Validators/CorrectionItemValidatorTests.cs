using Domain.Entities.Feedback;
using Domain.Validators;
using FluentValidation.TestHelper;

namespace IAcademy.Test.Unit.Validators;

public class CorrectionItemValidatorTests
{
    private readonly CorrectionItemValidator _validator;

    public CorrectionItemValidatorTests()
    {
        _validator = new CorrectionItemValidator();
    }

    [Theory]
    [InlineData(-1, false)]
    [InlineData(0, false)]
    [InlineData(1, true)]
    public void ValidateIdentification(int identification, bool expected)
    {
        var model = new CorrectionItem { Identification = identification };
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
    [InlineData(null, false)]
    [InlineData("", false)]
    [InlineData("Valid Question", true)]
    public void ValidateQuestion(string question, bool expected)
    {
        var model = new CorrectionItem { Question = question };
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

    [Theory]
    [InlineData(null, false)]
    [InlineData("", false)]
    [InlineData("Valid Answer", true)]
    public void ValidateAnswer(string answer, bool expected)
    {
        var model = new CorrectionItem { Answer = answer };
        var result = _validator.TestValidate(model);

        if (expected)
        {
            result.ShouldNotHaveValidationErrorFor(x => x.Answer);
        }
        else
        {
            result.ShouldHaveValidationErrorFor(x => x.Answer);
        }
    }

    [Theory]
    [InlineData(true, true)]
    [InlineData(false, true)]
    public void ValidateIsCorrect(bool isCorrect, bool expected)
    {
        var model = new CorrectionItem { IsCorrect = isCorrect };
        var result = _validator.TestValidate(model);

        if (expected)
        {
            result.ShouldNotHaveValidationErrorFor(x => x.IsCorrect);
        }
        else
        {
            result.ShouldHaveValidationErrorFor(x => x.IsCorrect);
        }
    }

    [Theory]
    [InlineData(null, false)]
    [InlineData("", false)]
    [InlineData("Valid Feedback", true)]
    public void ValidateFeedback(string feedback, bool expected)
    {
        var model = new CorrectionItem { Feedback = feedback };
        var result = _validator.TestValidate(model);

        if (expected)
        {
            result.ShouldNotHaveValidationErrorFor(x => x.Feedback);
        }
        else
        {
            result.ShouldHaveValidationErrorFor(x => x.Feedback);
        }
    }
}
