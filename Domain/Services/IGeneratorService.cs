using Domain.DTO;

namespace Domain.Services
{
    public interface IGeneratorService
    {
        Task<ServiceResult<string>> MakeExercise(string contentId, CancellationToken cancellationToken = default);
        Task<ServiceResult<string>> MakePendency(string contentId, string oldExercise, CancellationToken cancellationToken = default);
    }
}
