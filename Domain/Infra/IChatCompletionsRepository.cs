using Domain.Entities;

namespace Domain.Infra;

public interface IChatCompletionsRepository
{
    Task<ChatCompletion> Get(string chatId, CancellationToken cancellationToken = default);
    Task<string> Save(ChatCompletion chatCompletion, CancellationToken cancellationToken = default);
}

