using Domain.DTO.Content;
using Domain.Entities.Contents;
using Domain.Validators;
using FluentValidation.TestHelper;

namespace IAcademy.Test.Unit.Validators;

public class ContentRequestValidatorTests
{
    private readonly ContentRequestValidator _validator;

    public ContentRequestValidatorTests()
    {
        _validator = new ContentRequestValidator();
    }

    [Theory]
    [InlineData(null, false)]
    [InlineData("", false)]
    [InlineData("12345678-1234-1234-1234-1234567890123", false)]
    [InlineData("12345678-1234-1234-1234-123456789012", true)]
    public void ValidateOwnerId(string ownerId, bool expected)
    {
        var model = new ContentRequest { OwnerId = ownerId };

        var result = _validator.TestValidate(model);

        if (expected)
        {
            result.ShouldNotHaveValidationErrorFor(x => x.OwnerId);
        }
        else
        {
            result.ShouldHaveValidationErrorFor(x => x.OwnerId);
        }
    }

    [Theory]
    [InlineData(null, false)]
    [InlineData("", false)]
    [InlineData("12345678-1234-1234-1234-1234567890123", false)]
    [InlineData("12345678-1234-1234-1234-123456789012", true)]
    public void ValidateSummaryId(string summaryId, bool expected)
    {
        var model = new ContentRequest { SummaryId = summaryId };

        var result = _validator.TestValidate(model);

        if (expected)
        {
            result.ShouldNotHaveValidationErrorFor(x => x.SummaryId);
        }
        else
        {
            result.ShouldHaveValidationErrorFor(x => x.SummaryId);
        }
    }

    [Theory]
    [InlineData(null, false)]
    [InlineData("", false)]
    [InlineData("12345678-1234-1234-1234-1234567890123", false)]
    [InlineData("12345678-1234-1234-1234-123456789012", true)]
    public void ValidateConfigurationId(string configurationId, bool expected)
    {
        var model = new ContentRequest { ConfigurationId = configurationId };

        var result = _validator.TestValidate(model);

        if (expected)
        {
            result.ShouldNotHaveValidationErrorFor(x => x.ConfigurationId);
        }
        else
        {
            result.ShouldHaveValidationErrorFor(x => x.ConfigurationId);
        }
    }

    [Theory]
    [InlineData(null, true)]
    [InlineData("", true)]
    [InlineData("12345678-1234-1234-1234-1234567890123", false)]
    [InlineData("12345678-1234-1234-1234-123456789012", true)]
    public void ValidateExerciseId(string ExerciseId, bool expected)
    {
        var model = new ContentRequest { ExerciseId = ExerciseId };

        var result = _validator.TestValidate(model);

        if (expected)
        {
            result.ShouldNotHaveValidationErrorFor(x => x.ExerciseId);
        }
        else
        {
            result.ShouldHaveValidationErrorFor(x => x.ExerciseId);
        }
    }

    [Theory]
    [InlineData(null, false)]
    [InlineData("", false)]
    [InlineData("12345", false)]
    [InlineData("12", true)]
    public void ValidateSubtopicIndex(string subtopicIndex, bool expected)
    {
        var model = new ContentRequest { SubtopicIndex = subtopicIndex };

        var result = _validator.TestValidate(model);

        if (expected)
        {
            result.ShouldNotHaveValidationErrorFor(x => x.SubtopicIndex);
        }
        else
        {
            result.ShouldHaveValidationErrorFor(x => x.SubtopicIndex);
        }
    }

    [Theory]
    [InlineData(null, false)]
    [InlineData("", false)]
    [InlineData("This is a valid theme", true)]
    [InlineData("This theme is more than one hundred characters long and therefore it should be considered as invalid input.", false)]
    public void ValidateTheme(string theme, bool expected)
    {
        var model = new ContentRequest { Theme = theme };

        var result = _validator.TestValidate(model);

        if (expected)
        {
            result.ShouldNotHaveValidationErrorFor(x => x.Theme);
        }
        else
        {
            result.ShouldHaveValidationErrorFor(x => x.Theme);
        }
    }

    [Fact]
    public void ValidateBody_Invalid()
    {
        var model = new ContentRequest 
        { 
            Body = new Body 
            { 
                Contents = new List<Subcontent>() 
                { 
                    new Subcontent()
                    {
                        SubcontentHistory = new List<SubcontentHistory>()
                        {
                            new SubcontentHistory()
                            {
                                Content = "short",
                                CreatedDate = DateTime.Now
                            }
                        }
                    }
                } 
            } 
        };

        var result = _validator.TestValidate(model);

        Assert.False(result.IsValid);
    }
}

