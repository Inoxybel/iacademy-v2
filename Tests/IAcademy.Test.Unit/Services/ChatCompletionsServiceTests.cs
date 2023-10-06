using Domain.Entities.Chat;
using Domain.Infra;
using Domain.Services;
using FluentAssertions;
using IAcademy.Test.Shared.Builders;
using Moq;
using Service;

namespace IAcademy.Test.Unit.Services
{
    public class ChatCompletionsServiceTests
    {
        private Mock<IChatCompletionsRepository> _repository = new();
        private IChatCompletionsService _service;

        public ChatCompletionsServiceTests()
        {
            _service = new ChatCompletionsService(_repository.Object);
        }

        [Fact]
        public async Task Get_SHOULD_Returns_Fail_WHEN_ChatId_IsNullOrEmpty()
        {
            var result = await _service.Get(string.Empty);

            result.Success.Should().BeFalse();
            result.ErrorMessage.Should().Be("Invalid chatId.");
        }

        [Fact]
        public async Task Get_SHOULD_Returns_Fail_WHEN_Chat_Isnt_Founded()
        {
            var result = await _service.Get("chatId");

            result.Success.Should().BeFalse();
            result.ErrorMessage.Should().Be("Chat not found.");
        }

        [Fact]
        public async Task Get_SHOULD_Returns_Success_WHEN_Chat_Was_Founded()
        {
            var repositoryResponse = new ChatCompletionsBuilder()
                .WithId("chatId")
                .Build();

            _repository.Setup(r => r.Get(It.IsAny<string>(), default))
                .ReturnsAsync(repositoryResponse);

            var result = await _service.Get("chatId");

            result.Success.Should().BeTrue();
            result.ErrorMessage.Should().BeNullOrEmpty();
            result.Data.Should().BeEquivalentTo(repositoryResponse);
        }

        [Fact]
        public async Task Save_SHOULD_Returns_Fail_WHEN_Repository_Doesnt_Save_With_Successful()
        {
            var repositoryRequest = new ChatCompletionsBuilder()
                .WithId("chatId")
                .Build();

            var result = await _service.Save(repositoryRequest);

            result.Success.Should().BeFalse();
            result.ErrorMessage.Should().Be("Fail to save the chat.");
        }

        [Fact]
        public async Task Save_SHOULD_Returns_Success_WHEN_Repository_Save_With_Successful()
        {
            var repositoryRequest = new ChatCompletionsBuilder()
                .WithId("chatId")
                .Build();

            var repositoryResponse = "response";

            _repository.Setup(r => r.Save(It.IsAny<ChatCompletion>(), default))
                .ReturnsAsync(repositoryResponse);

            var result = await _service.Save(repositoryRequest);

            result.Success.Should().BeTrue();
            result.ErrorMessage.Should().BeNullOrEmpty();
            result.Data.Should().BeEquivalentTo(repositoryResponse);
        }
    }
}
