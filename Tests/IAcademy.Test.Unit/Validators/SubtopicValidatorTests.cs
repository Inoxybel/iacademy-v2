using Domain.Entities.Summary;
using Domain.Validators;
using FluentValidation.TestHelper;

namespace IAcademy.Test.Unit.Validators;

public class SubtopicValidatorTests
{
    private readonly SubtopicValidator _validator;

    public SubtopicValidatorTests()
    {
        _validator = new SubtopicValidator();
    }

    [Theory]
    [InlineData(null, false)]
    [InlineData("", false)]
    [InlineData("1", true)]
    [InlineData("1.2", true)]
    [InlineData("1.23", false)]
    [InlineData("1a", false)]
    public void ValidateIndex(string index, bool expected)
    {
        var model = new Subtopic { Index = index };
        var result = _validator.TestValidate(model);

        if (expected)
        {
            result.ShouldNotHaveValidationErrorFor(x => x.Index);
        }
        else
        {
            result.ShouldHaveValidationErrorFor(x => x.Index);
        }
    }

    [Theory]
    [InlineData(null, false)]
    [InlineData("", false)]
    [InlineData("a", true)]
    [InlineData("This is a valid title that is under 200 characters", true)]
    [InlineData("This title is more than 200 characters, which is longer than what is allowed for this field. Therefore, this should return a validation error because it exceeds the maximum allowed length for the title field in the Subtopic class.", false)]
    public void ValidateTitle(string title, bool expected)
    {
        var model = new Subtopic { Title = title };
        var result = _validator.TestValidate(model);

        if (expected)
        {
            result.ShouldNotHaveValidationErrorFor(x => x.Title);
        }
        else
        {
            result.ShouldHaveValidationErrorFor(x => x.Title);
        }
    }

    [Theory]
    [InlineData(null, true)]
    [InlineData("", true)]
    [InlineData("12345678901234567890123456789012345", false)]
    [InlineData("1234567890123456789012345678901234567", false)]
    [InlineData("123456789012345678901234567890123456", true)]
    public void ValidateContentId(string contentId, bool expected)
    {
        var model = new Subtopic { ContentId = contentId };
        var result = _validator.TestValidate(model);

        if (expected)
        {
            result.ShouldNotHaveValidationErrorFor(x => x.ContentId);
        }
        else
        {
            result.ShouldHaveValidationErrorFor(x => x.ContentId);
        }
    }
}
