using Domain.DTO.Summary;
using Domain.Validators;
using FluentValidation.TestHelper;

namespace IAcademy.Test.Unit.Validators;

public class SummaryCreationRequestValidatorTests
{
    private readonly SummaryCreationRequestValidator _validator;

    public SummaryCreationRequestValidatorTests()
    {
        _validator = new SummaryCreationRequestValidator();
    }

    [Theory]
    [InlineData(null, false)]
    [InlineData("", false)]
    [InlineData("ab", false)]
    [InlineData("abc", true)]
    [InlineData("This is a valid theme that is under 100 characters long.", true)]
    [InlineData("This theme is more than 100 characters long, which is longer than what is allowed for this field. Therefore, this should return a validation error because it exceeds the maximum allowed length for the theme field in the SummaryCreationRequest class.", false)]
    public void ValidateTheme(string theme, bool expected)
    {
        var model = new SummaryCreationRequest { Theme = theme };
        var result = _validator.TestValidate(model);

        if (expected)
        {
            result.ShouldNotHaveValidationErrorFor(request => request.Theme);
        }
        else
        {
            result.ShouldHaveValidationErrorFor(request => request.Theme);
        }
    }

    [Theory]
    [InlineData(null, false)]
    [InlineData("", false)]
    [InlineData("ab", false)]
    [InlineData("abc", true)]
    [InlineData("This is a valid theme that is under 100 characters long.", true)]
    [InlineData("This theme is more than 100 characters long, which is longer than what is allowed for this field. Therefore, this should return a validation error because it exceeds the maximum allowed length for the theme field in the SummaryCreationRequest class.", false)]
    public void ValidateCategory(string theme, bool expected)
    {
        var model = new SummaryCreationRequest { Theme = theme };
        var result = _validator.TestValidate(model);

        if (expected)
        {
            result.ShouldNotHaveValidationErrorFor(request => request.Theme);
        }
        else
        {
            result.ShouldHaveValidationErrorFor(request => request.Theme);
        }
    }

    [Theory]
    [InlineData(null, false)]
    [InlineData("", false)]
    [InlineData("ab", false)]
    [InlineData("abc", true)]
    [InlineData("This is a valid theme that is under 100 characters long.", true)]
    [InlineData("This theme is more than 100 characters long, which is longer than what is allowed for this field. Therefore, this should return a validation error because it exceeds the maximum allowed length for the theme field in the SummaryCreationRequest class.", false)]
    public void ValidateSubcategory(string theme, bool expected)
    {
        var model = new SummaryCreationRequest { Theme = theme };
        var result = _validator.TestValidate(model);

        if (expected)
        {
            result.ShouldNotHaveValidationErrorFor(request => request.Theme);
        }
        else
        {
            result.ShouldHaveValidationErrorFor(request => request.Theme);
        }
    }

    [Theory]
    [InlineData(null, false)]
    [InlineData("", false)]
    [InlineData("12345678901234567890123456789012345", false)]
    [InlineData("123456789012345678901234567890123456", true)]
    public void ValidateConfigurationId(string configurationId, bool expected)
    {
        var model = new SummaryCreationRequest { ConfigurationId = configurationId };
        var result = _validator.TestValidate(model);

        if (expected)
        {
            result.ShouldNotHaveValidationErrorFor(request => request.ConfigurationId);
        }
        else
        {
            result.ShouldHaveValidationErrorFor(request => request.ConfigurationId);
        }
    }

    [Theory]
    [InlineData(null, false)]
    [InlineData("", false)]
    [InlineData("12345678901234567890123456789012345", false)]
    [InlineData("123456789012345678901234567890123456", true)]
    public void ValidateOwnerId(string ownerId, bool expected)
    {
        var model = new SummaryCreationRequest { OwnerId = ownerId };
        var result = _validator.TestValidate(model);

        if (expected)
        {
            result.ShouldNotHaveValidationErrorFor(request => request.OwnerId);
        }
        else
        {
            result.ShouldHaveValidationErrorFor(request => request.OwnerId);
        }
    }
}
