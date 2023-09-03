using System.Net.Http.Headers;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Service.Integrations.OpenAI.Interfaces;
using Service.Integrations.OpenAI.Options;

namespace Service.Integrations.OpenAI.Configuration;

public static class ServiceProviderExtensions
{
    public static IServiceCollection AddOpenAIService(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddOptions<OpenAIOptions>()
            .Configure(serviceOptions =>
            {
                serviceOptions.Model = configuration.GetValue<string>($"IAcademy:ExternalServices:OpenAI:{OpenAIOptions.ModelKey}");
            });

        services.AddHttpClient<IOpenAIService, OpenAIService>(opt =>
        {
            opt.BaseAddress = new Uri(configuration.GetValue<string>("IAcademy:ExternalServices:OpenAI:BaseUrl"));
            opt.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(
                "Bearer",
                configuration.GetValue<string>("IAcademy:ExternalServices:OpenAI:SecretKey")
            );
        });

        return services;
    }
}