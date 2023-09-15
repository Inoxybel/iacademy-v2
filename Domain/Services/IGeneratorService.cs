using Domain.DTO;
using Domain.Entities;

namespace Domain.Services
{
    public interface IGeneratorService
    {
        Task<ServiceResult<string>> MakeExercise(string contentId, CancellationToken cancellationToken = default);
        Task<ServiceResult<string>> MakeExercise(Content content, Configuration configuration, CancellationToken cancellationToken = default);
        Task<ServiceResult<string>> MakePendency(string contentId, string oldExercise, CancellationToken cancellationToken = default);
    }
}
