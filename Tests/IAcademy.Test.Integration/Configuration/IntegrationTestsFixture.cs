using FluentAssertions;
using Infra;
using Microsoft.Extensions.DependencyInjection;

namespace IAcademy.Test.Integration.Configuration;

public class IntegrationTestsFixture : IDisposable
{
    public HttpClient httpClient { get; }
    public DbContext DbContext { get; }
    public IServiceProvider serviceProvider { get; }
    private readonly IServiceScope _scope;

    public IntegrationTestsFixture()
    {
        var api = new WebApiApplicationFactory();

        httpClient = api.CreateClient();

        _scope = api.Services.CreateScope();
        serviceProvider = _scope.ServiceProvider;

        DbContext = serviceProvider.GetRequiredService<DbContext>();

        AssertionOptions.AssertEquivalencyUsing(options =>
            options.Using<DateTime>(ctx => ctx.Subject.Should().BeCloseTo(ctx.Expectation, TimeSpan.FromSeconds(1)))
                .WhenTypeIs<DateTime>()
        );
    }

    public void Dispose()
    {
        GC.SuppressFinalize( this );

        _scope?.Dispose();
        httpClient.Dispose();
    }
}