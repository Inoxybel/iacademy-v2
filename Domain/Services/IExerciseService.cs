using CrossCutting.Enums;
using Domain.DTO.Exercise;
using Domain.DTO;
using Domain.Entities;

namespace Domain.Services;

public interface IExerciseService
{
    Task<ServiceResult<Exercise>> Get(string exerciseId, CancellationToken cancellationToken = default);
    Task<ServiceResult<List<Exercise>>> GetAllByOwnerIdAndType(string ownerId, ExerciseType type, CancellationToken cancellationToken = default);
    Task<ServiceResult<string>> Save(ExerciseRequest exercise, CancellationToken cancellationToken = default);
    Task<ServiceResult<bool>> MakeExercise(string contentId, MakeExerciseRequest request, CancellationToken cancellationToken = default);
    Task<ServiceResult<bool>> Update(string exerciseId, ExerciseRequest exercise, CancellationToken cancellationToken = default);
}