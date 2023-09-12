using Domain.Entities;
using Domain.Validators;
using FluentValidation.TestHelper;

namespace IAcademy.Test.Unit.Validators;

public class BodyValidatorTests
{
    private readonly BodyValidator _validator;

    public BodyValidatorTests()
    {
        _validator = new BodyValidator();
    }

    [Theory]
    [InlineData(null, false)]
    [InlineData("", false)]
    [InlineData("Short", false)]
    [InlineData("ValidContentValidContentValidContentValidContentValidContentValidContentValidContentValidContentValidContentValidContent", true)]
    [InlineData("VeryLongContent", false)]
    public void ValidateContent(string content, bool expected)
    {
        if (string.Equals(content, "VeryLongContent"))
            content = content.PadRight(5001, 'a');

        var model = new Body
        {
            Content = content,
            CreatedDate = DateTime.Now
        };

        var result = _validator.TestValidate(model);

        if (expected)
        {
            result.ShouldNotHaveValidationErrorFor(x => x.Content);
        }
        else
        {
            result.ShouldHaveValidationErrorFor(x => x.Content);
        }
    }

    [Theory]
    [InlineData("2023-09-10", true)]
    [InlineData("", false)]
    public void ValidateCreatedDate(string createdDateStr, bool expected)
    {
        DateTime? createdDate = string.IsNullOrEmpty(createdDateStr) ? (DateTime?)null : DateTime.Parse(createdDateStr);

        var model = new Body
        {
            Content = "ValidContentValidContentValidContentValidContentValidContentValidContentValidContentValidContentValidContentValidContent",
            CreatedDate = createdDate ?? default
        };

        var result = _validator.TestValidate(model);

        if (expected)
        {
            result.ShouldNotHaveValidationErrorFor(x => x.CreatedDate);
        }
        else
        {
            result.ShouldHaveValidationErrorFor(x => x.CreatedDate);
        }
    }

    [Theory]
    [InlineData("2023-09-10", "2023-09-11", false)]
    [InlineData("2023-09-10", "2023-09-09", true)]
    [InlineData("2023-09-10", "", true)]
    public void ValidateDisabledDate(string createdDateStr, string disabledDateStr, bool expected)
    {
        DateTime createdDate = DateTime.Parse(createdDateStr);
        DateTime? disabledDate = string.IsNullOrEmpty(disabledDateStr) ? null : DateTime.Parse(disabledDateStr);

        var model = new Body
        {
            Content = "ValidContentValidContentValidContentValidContentValidContentValidContentValidContentValidContentValidContentValidContent",
            CreatedDate = createdDate,
            DisabledDate = disabledDate ?? default
        };

        var result = _validator.TestValidate(model);

        if (expected)
        {
            result.ShouldNotHaveValidationErrorFor(x => x.DisabledDate);
        }
        else
        {
            result.ShouldHaveValidationErrorFor(x => x.DisabledDate);
        }
    }
}

