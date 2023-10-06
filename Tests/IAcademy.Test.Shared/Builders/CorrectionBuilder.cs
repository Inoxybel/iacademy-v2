using Domain.Entities;
using Domain.Entities.Feedback;

namespace IAcademy.Test.Shared.Builders;
public class CorrectionBuilder
{
    private Correction correction;

    public CorrectionBuilder() => correction = CreateDefault();

    private static Correction CreateDefault() => new Correction
    {
        Id = Guid.NewGuid().ToString(),
        ExerciseId = string.Empty,
        OwnerId = string.Empty,
        CreatedDate = DateTime.UtcNow,
        UpdatedDate = DateTime.UtcNow,
        Corrections = new()
        {
            new CorrectionItemBuilder().Build()
        }
    };

    public CorrectionBuilder WithId(string id)
    {
        correction.Id = id;
        return this;
    }

    public CorrectionBuilder WithExerciseId(string exerciseId)
    {
        correction.ExerciseId = exerciseId;
        return this;
    }

    public CorrectionBuilder WithOwnerId(string ownerId)
    {
        correction.OwnerId = ownerId;
        return this;
    }

    public CorrectionBuilder WithCreatedDate(DateTime createdDate)
    {
        correction.CreatedDate = createdDate;
        return this;
    }

    public CorrectionBuilder WithUpdatedDate(DateTime updatedDate)
    {
        correction.UpdatedDate = updatedDate;
        return this;
    }

    public CorrectionBuilder WithCorrections(List<CorrectionItem> corrections)
    {
        correction.Corrections = corrections;
        return this;
    }

    public Correction Build() => correction;
}
