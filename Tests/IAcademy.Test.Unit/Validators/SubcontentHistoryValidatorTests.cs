using Domain.Entities.Contents;
using Domain.Validators;
using FluentValidation.TestHelper;

namespace IAcademy.Test.Unit.Validators;
public class SubcontentHistoryValidatorTests
{
    private readonly SubcontentHistoryValidator _validator;

    public SubcontentHistoryValidatorTests()
    {
        _validator = new SubcontentHistoryValidator();
    }

    [Theory]
    [InlineData(null, false)]
    [InlineData("", false)]
    [InlineData("Short", false)]
    [InlineData("ValidContentValidContentValidContentValidContentValidContentValidContentValidContentValidContentValidContentValidContent", true)]
    [InlineData("VeryLongContent", false)]
    public void ValidateContent(string content, bool expected)
    {
        if (content == "VeryLongContent")
            content = content.PadRight(5001, 'a');

        var model = new SubcontentHistory
        {
            Content = content
        };

        var result = _validator.TestValidate(model);

        AssertValidationResult(result, x => x.Content, expected);
    }

    [Theory]
    [InlineData("2023-09-10", true)]
    [InlineData("", false)]
    public void ValidateCreatedDate(string createdDateStr, bool expected)
    {
        DateTime? createdDate = string.IsNullOrEmpty(createdDateStr)
            ? (DateTime?)null
            : DateTime.Parse(createdDateStr);

        var model = new SubcontentHistory
        {
            Content = "ValidContent",
            CreatedDate = createdDate ?? default
        };

        var result = _validator.TestValidate(model);

        AssertValidationResult(result, x => x.CreatedDate, expected);
    }

    [Theory]
    [InlineData("2023-09-10", "2023-09-11", false)]
    [InlineData("2023-09-10", "2023-09-09", true)]
    [InlineData("2023-09-10", "", true)]
    public void ValidateDisabledDate(string createdDateStr, string disabledDateStr, bool expected)
    {
        DateTime createdDate = DateTime.Parse(createdDateStr);
        DateTime? disabledDate = string.IsNullOrEmpty(disabledDateStr)
            ? (DateTime?)null
            : DateTime.Parse(disabledDateStr);

        var model = new SubcontentHistory
        {
            Content = "ValidContent",
            CreatedDate = createdDate,
            DisabledDate = disabledDate ?? default
        };

        var result = _validator.TestValidate(model);

        AssertValidationResult(result, x => x.DisabledDate, expected);
    }

    private static void AssertValidationResult<T>(
        TestValidationResult<SubcontentHistory> result,
        System.Linq.Expressions.Expression<Func<SubcontentHistory, T>> expression,
        bool expected)
    {
        if (expected)
        {
            result.ShouldNotHaveValidationErrorFor(expression);
        }
        else
        {
            result.ShouldHaveValidationErrorFor(expression);
        }
    }
}
