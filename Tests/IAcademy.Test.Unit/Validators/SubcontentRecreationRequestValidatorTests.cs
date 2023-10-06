using Domain.DTO.Content;
using Domain.Validators;
using FluentValidation.TestHelper;

namespace IAcademy.Test.Unit.Validators;

public class SubcontentRecreationRequestValidatorTests
{
    private readonly SubcontentRecreationRequestValidator _validator;

    public SubcontentRecreationRequestValidatorTests()
    {
        _validator = new SubcontentRecreationRequestValidator();
    }

    [Theory]
    [InlineData(-1, false)]
    [InlineData(0, true)]
    [InlineData(1, true)]
    public void ValidateSubcontentIndex(int subcontentIndex, bool expected)
    {
        var model = new SubcontentRecreationRequest { SubcontentIndex = subcontentIndex };

        var result = _validator.TestValidate(model);

        if (expected)
        {
            result.ShouldNotHaveValidationErrorFor(x => x.SubcontentIndex);
        }
        else
        {
            result.ShouldHaveValidationErrorFor(x => x.SubcontentIndex);
        }
    }
}
