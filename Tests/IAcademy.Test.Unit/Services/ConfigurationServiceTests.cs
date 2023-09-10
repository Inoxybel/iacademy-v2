using Domain.DTO.Configuration;
using Domain.Entities;
using Domain.Infra;
using FluentAssertions;
using IAcademy.Test.Shared.Builders;
using Moq;
using Service;

namespace IAcademy.Test.Unit.Services
{
    public class ConfigurationServiceTests
    {
        private readonly Mock<IConfigurationRepository> _repositoryMock;
        private readonly ConfigurationService _service;

        public ConfigurationServiceTests()
        {
            _repositoryMock = new Mock<IConfigurationRepository>();
            _service = new ConfigurationService(_repositoryMock.Object);
        }

        [Fact]
        public async Task Get_SHOULD_Return_Configuration_WHEN_It_Exists()
        {
            var configuration = new ConfigurationBuilder().Build();

            _repositoryMock.Setup(r => r.Get(configuration.Id, It.IsAny<CancellationToken>())).ReturnsAsync(configuration);

            var result = await _service.Get(configuration.Id);

            result.Success.Should().BeTrue();
            result.Data.Should().Be(configuration);
        }

        [Fact]
        public async Task Get_SHOULD_Return_Error_WHEN_Configuration_Does_Not_Exist()
        {
            var result = await _service.Get("id");

            result.Success.Should().BeFalse();
            result.ErrorMessage.Should().Be("Configuration not finded.");
        }

        [Fact]
        public async Task Create_SHOULD_Return_Success_WHEN_Repository_Save_Is_Successful()
        {
            var configuration = new ConfigurationBuilder().Build();
            _repositoryMock.Setup(r => r.Save(It.IsAny<Configuration>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);

            var result = await _service.Create(new ConfigurationRequest());

            result.Success.Should().BeTrue();
            result.Data.Id.Should().NotBeNull();
            result.ErrorMessage.Should().BeEmpty();
        }

        [Fact]
        public async Task Create_SHOULD_Return_Error_WHEN_Repository_Save_Fails()
        {
            var configurationRequest = new ConfigurationRequest();

            _repositoryMock.Setup(r => r.Save(It.IsAny<Configuration>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(false);

            var result = await _service.Create(configurationRequest);

            result.Success.Should().BeFalse();
            result.ErrorMessage.Should().Be("Error on save configuration");
            result.Data.Should().BeNull();
        }

        [Fact]
        public async Task Update_SHOULD_Return_Success_WHEN_Update_Is_Successful()
        {
            var configurationRequest = new ConfigurationRequest()
            {
                Summary = new InputPropertiesBuilder()
                .WithInitialInput("new")
                .WithFinalInput("new")
                .Build(),
                FirstContent = new InputPropertiesBuilder()
                .WithInitialInput("new")
                .WithFinalInput("new")
                .Build(),
                NewContent = new InputPropertiesBuilder()
                .WithInitialInput("new")
                .WithFinalInput("new")
                .Build(),
                Exercise = new InputPropertiesBuilder()
                .WithInitialInput("new")
                .WithFinalInput("new")
                .Build(),
                Correction = new InputPropertiesBuilder()
                .WithInitialInput("new")
                .WithFinalInput("new")
                .Build(),
                Pendency = new InputPropertiesBuilder()
                .WithInitialInput("new")
                .WithFinalInput("new")
                .Build()
            };

            var existingConfiguration = new ConfigurationBuilder().Build();

            _repositoryMock.Setup(r => r.Get(existingConfiguration.Id, It.IsAny<CancellationToken>()))
                .ReturnsAsync(existingConfiguration);

            _repositoryMock.Setup(r => r.Update(existingConfiguration.Id, configurationRequest, It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);

            var result = await _service.Update(existingConfiguration.Id, configurationRequest);

            result.Success.Should().BeTrue();
            result.Data.Should().BeTrue();
            result.ErrorMessage.Should().BeEmpty();
        }

        [Fact]
        public async Task Update_ShouldReturnError_WhenConfigurationDoesNotExist()
        {
            var configurationRequest = new ConfigurationRequest();

            var result = await _service.Update("id", configurationRequest);

            result.Success.Should().BeFalse();
            result.ErrorMessage.Should().Be("Configuration not finded.");
            result.Data.Should().BeFalse();
        }

        [Fact]
        public async Task Update_SHOULD_Return_Error_WHEN_Update_Fails()
        {
            var configurationRequest = new ConfigurationRequest();
            var existingConfiguration = new ConfigurationBuilder().Build();

            _repositoryMock.Setup(r => r.Get(existingConfiguration.Id, It.IsAny<CancellationToken>()))
                .ReturnsAsync(existingConfiguration);

            _repositoryMock.Setup(r => r.Update(existingConfiguration.Id, configurationRequest, It.IsAny<CancellationToken>()))
                .ReturnsAsync(false);

            var result = await _service.Update(existingConfiguration.Id, configurationRequest);

            result.Success.Should().BeFalse();
            result.ErrorMessage.Should().Be("Error on update configuration");
            result.Data.Should().BeFalse();
        }

    }
}
