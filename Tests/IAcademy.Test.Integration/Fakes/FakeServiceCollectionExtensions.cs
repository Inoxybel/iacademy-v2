using IAcademy.Test.Integration.Base;
using Microsoft.Extensions.DependencyInjection;

namespace IAcademy.Test.Integration.Fakes;

public static class FakeServiceCollectionExtensions
{
    private static object _lock = new();

    public static void AddServiceWithFaker<T>(this IServiceCollection services, Action<T> configure = null)
        where T : class
    {
        services.AddTransient(sp =>
        {
            T faker;

            lock (_lock)
            {
                faker = FakeServiceProvider.Get<T>();

                if (faker is not null) return faker;

                faker = IntegrationTestBase.Mocker.Get<T>();
                configure?.Invoke(faker);
                FakeServiceProvider.Add(faker);
            }

            return faker;
        });
    }
}