using Domain.DTO;
using Domain.Entities.Configuration;
using Domain.Entities.Contents;

namespace Domain.Services
{
    public interface IGeneratorService
    {
        Task<ServiceResult<string>> MakeExercise(string contentId, CancellationToken cancellationToken = default);
        Task<ServiceResult<string>> MakeExercise(Content content, InputProperties configuration, string configurationId, CancellationToken cancellationToken = default);
        Task<ServiceResult<string>> MakePendency(string contentId, string correction, Configuration configuration, CancellationToken cancellationToken = default);
    }
}
