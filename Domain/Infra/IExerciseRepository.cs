using CrossCutting.Enums;
using Domain.Entities;

namespace Domain.Infra;

public interface IExerciseRepository
{
    Task<Exercise> Get(string exerciseId, CancellationToken cancellationToken = default);
    Task<List<Exercise>> GetAllByOwnerIdAndType(string ownerId, ExerciseType type, CancellationToken cancellationToken = default);
    Task<bool> Save(Exercise exercise, CancellationToken cancellationToken = default);
    Task<bool> Update(string exerciseId, Exercise exercise, CancellationToken cancellationToken = default);
}