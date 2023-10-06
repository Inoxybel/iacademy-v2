using Domain.Entities.Contents;
using Domain.Validators;
using FluentValidation.TestHelper;

namespace IAcademy.Test.Unit.Validators;

public class BodyValidatorTests
{
    private readonly BodyValidator _validator;
    private static readonly List<Subcontent> emptyList = new();
    private static readonly List<Subcontent> populedList = new()
    {
        new Subcontent()
        {
            SubcontentHistory = new List<SubcontentHistory>()
            {
                new SubcontentHistory()
                {
                    Content = "",
                    CreatedDate = DateTime.Today,
                    DisabledDate = DateTime.MinValue
                }
            }
        }
    };

    public BodyValidatorTests()
    {
        _validator = new BodyValidator();
    }

    public static IEnumerable<object[]> ContentValidationData()
    {
        yield return new object[] { null, false };
        yield return new object[] { emptyList, false };
        yield return new object[] { populedList, true };
    }

    [Theory]
    [MemberData(nameof(ContentValidationData))]
    public void ValidateContents(List<Subcontent> contents, bool expected)
    {
        var model = new Body()
        {
            Contents = contents
        };

        var result = _validator.TestValidate(model);

        if (expected)
        {
            result.ShouldNotHaveValidationErrorFor(x => x.Contents);
        }
        else
        {
            result.ShouldHaveValidationErrorFor(x => x.Contents);
        }
    }
}

