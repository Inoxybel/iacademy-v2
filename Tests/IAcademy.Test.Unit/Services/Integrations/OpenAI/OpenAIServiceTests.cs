using FluentAssertions;
using Microsoft.Extensions.Options;
using Moq;
using Moq.Protected;
using Service.Integrations.OpenAI;
using Service.Integrations.OpenAI.Options;
using System.Net;
using System.Text;

namespace IAcademy.Test.Unit.Services.Integrations.OpenAI
{
    public class OpenAIServiceTests
    {
        private Mock<HttpMessageHandler> _mockHttpMessageHandler;
        private Mock<IOptionsSnapshot<OpenAIOptions>> _mockOptions;
        private HttpClient httpClient;
        private OpenAIService _service;

        public OpenAIServiceTests()
        {
            _mockHttpMessageHandler = new Mock<HttpMessageHandler>();
            _mockOptions = new Mock<IOptionsSnapshot<OpenAIOptions>>();

            _mockOptions.Setup(x => x.Value)
                .Returns(new OpenAIOptions
                {
                    Model = "test-model" 
                });
        }

        [Fact]
        public async Task DoRequest_Should_ReturnsValidObject_WHEN_OpenAIAPI_Responds_With_Success()
        {
            var httpResponseMessage = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent("{\"choices\":[{\"finish_reason\":\"stop\",\"index\": 0,\"message\": {\"content\":\"Message\",\"role\":\"assistant\"}}],\"created\":1677664795,\"id\":\"chatcmpl-7QyqpwdfhqwajicIEznoc6Q47XAyW\",\"model\":\"gpt-4\",\"object\":\"chat.completion\",\"usage\": {\"completion_tokens\": 17,\"prompt_tokens\":57,\"total_tokens\":74}}", Encoding.UTF8, "application/json")
            };

            _mockHttpMessageHandler.Protected()
                .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(httpResponseMessage);

            httpClient = new HttpClient(_mockHttpMessageHandler.Object)
            {
                BaseAddress = new Uri("https://test/")
            };

            _service = new OpenAIService(httpClient, _mockOptions.Object);

            var result = await _service.DoRequest(new(), "Test request");

            result.Should().NotBeNull();
            result.Choices.Should().NotBeNullOrEmpty();
        }

        [Fact]
        public async Task DoRequest_Should_ReturnsEmptyObject_WHEN_OpenAIAPI_Responds_With_Unsucess()
        {
            var httpResponseMessage = new HttpResponseMessage(HttpStatusCode.BadRequest);

            _mockHttpMessageHandler.Protected()
                .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(httpResponseMessage);

            httpClient = new HttpClient(_mockHttpMessageHandler.Object)
            {
                BaseAddress = new Uri("https://test/")
            };

            _service = new OpenAIService(httpClient, _mockOptions.Object);

            var result = await _service.DoRequest(new(), "Test request");

            result.Should().NotBeNull();
            result.Choices.Should().BeNullOrEmpty();
        }

        [Fact]
        public async Task DoRequest_Should_Retry_Twice_WHEN_OpenAI_Responds_Non_4XX_Error()
        {
            var httpResponseMessage = new HttpResponseMessage(HttpStatusCode.InternalServerError);

            _mockHttpMessageHandler.Protected()
                .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(httpResponseMessage);

            httpClient = new HttpClient(_mockHttpMessageHandler.Object)
            {
                BaseAddress = new Uri("https://test/")
            };

            _service = new OpenAIService(httpClient, _mockOptions.Object);

            var result = await _service.DoRequest(new(), "Test request");

            _mockHttpMessageHandler.Protected().Verify(
                "SendAsync", 
                Times.Exactly(2), 
                ItExpr.IsAny<HttpRequestMessage>(), 
                ItExpr.IsAny<CancellationToken>()
            );
        }
    }

}
