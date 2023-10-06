using Domain.DTO;
using Domain.Entities.Chat;
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
                return ServiceResult<ChatCompletion>.MakeErrorResult("Invalid chatId.");

            var chat = await _repository.Get(chatId, cancellationToken);

            if (chat is null)
                return ServiceResult<ChatCompletion>.MakeErrorResult("Chat not found.");

            return ServiceResult<ChatCompletion>.MakeSuccessResult(chat);
        }

        public async Task<ServiceResult<string>> Save(ChatCompletion chatCompletion, CancellationToken cancellationToken = default)
        {
            var repositoryResult = await _repository.Save(chatCompletion, cancellationToken);

            if (string.IsNullOrEmpty(repositoryResult))
                return ServiceResult<string>.MakeErrorResult("Fail to save the chat.");

            return ServiceResult<string>.MakeSuccessResult(repositoryResult);
        }
    }
}
