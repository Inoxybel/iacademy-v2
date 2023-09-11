using CrossCutting.Enums;
using Domain.DTO.Exercise;
using Domain.DTO;
using Domain.Entities;

namespace Domain.Services;

public interface IExerciseService
{
    Task<ServiceResult<Exercise>> Get(string exerciseId, CancellationToken cancellationToken = default);
    Task<ServiceResult<List<Exercise>>> GetAllByIds(IEnumerable<string> exerciseIds, CancellationToken cancellationToken = default);
    Task<ServiceResult<List<Exercise>>> GetAllByOwnerIdAndType(string ownerId, ExerciseType type, CancellationToken cancellationToken = default);
    Task<ServiceResult<string>> Save(ExerciseRequest exercise, CancellationToken cancellationToken = default);
    Task<ServiceResult<List<string>>> SaveAll(IEnumerable<Exercise> exercises, CancellationToken cancellationToken = default);
    Task<ServiceResult<string>> MakeExercise(string contentId, CancellationToken cancellationToken = default);
    Task<ServiceResult<string>> MakePendency(string exerciseId, CancellationToken cancellationToken = default);
    Task<ServiceResult<bool>> Update(string exerciseId, ExerciseRequest exercise, CancellationToken cancellationToken = default);
}