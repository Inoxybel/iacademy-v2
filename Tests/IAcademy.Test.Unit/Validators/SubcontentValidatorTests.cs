using Domain.Entities.Contents;
using Domain.Validators;
using FluentAssertions;

namespace IAcademy.Test.Unit.Validators;
public class SubcontentValidatorTests
{
    private readonly SubcontentValidator _validator;

    public SubcontentValidatorTests()
    {
        _validator = new SubcontentValidator();
    }

    [Fact]
    public void SubcontentWithValidHistory_SHOULD_BeValid()
    {
        var subcontent = new Subcontent
        {
            SubcontentHistory = new List<SubcontentHistory>
                {
                    new SubcontentHistory
                    {
                        Content = "ThisIsAValidContent".PadRight(100,'T'),
                        CreatedDate = DateTime.Now,
                        DisabledDate = DateTime.MinValue
                    }
                }
        };

        var result = _validator.Validate(subcontent);

        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public void SubcontentWithInvalidHistoryContent_SHOULD_BeInvalid()
    {
        var subcontent = new Subcontent
        {
            SubcontentHistory = new List<SubcontentHistory>
                {
                    new SubcontentHistory
                    {
                        Content = "Short",
                        CreatedDate = DateTime.Now
                    }
                }
        };

        var result = _validator.Validate(subcontent);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "SubcontentHistory[0].Content");
    }
}
