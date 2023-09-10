using Domain.DTO;
using Domain.Entities;
using Domain.Infra;
using Domain.Services;

namespace Service
{
    public class ChatCompletionsService : IChatCompletionsService
    {
        private readonly IChatCompletionsRepository _repository;

        public ChatCompletionsService(
            IChatCompletionsRepository repository)
        {
            _repository = repository;
        }

        public async Task<ServiceResult<ChatCompletion>> Get(string chatId, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrEmpty(chatId))
                return GetFailResponse("Invalid chatId.");

            var chat = await _repository.Get(chatId, cancellationToken);

            if (chat is null)
                return GetFailResponse("Chat not found.");

            return new()
            {
                Success = true,
                Data = chat
            };
        }

        public async Task<ServiceResult<string>> Save(ChatCompletion chatCompletion, CancellationToken cancellationToken = default)
        {
            var repositoryResult = await _repository.Save(chatCompletion, cancellationToken);

            if (string.IsNullOrEmpty(repositoryResult))
                return new()
                {
                    Success = false,
                    ErrorMessage = "Fail to save the chat."
                };

            return new()
            {
                Success = true,
                Data = repositoryResult
            };
        }

        private static ServiceResult<ChatCompletion> GetFailResponse(string message) => new()
        {
            Success = false,
            ErrorMessage = message
        };
    }
}
