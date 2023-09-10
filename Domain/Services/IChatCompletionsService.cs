using Domain.DTO;
using Domain.Entities;

namespace Domain.Services
{
    public interface IChatCompletionsService
    {
        Task<ServiceResult<ChatCompletion>> Get(string chatId, CancellationToken cancellationToken = default);
        Task<ServiceResult<string>> Save(ChatCompletion chatCompletion, CancellationToken cancellationToken = default);
    }
}
