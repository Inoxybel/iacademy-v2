using Domain.Infra;
using Infra;
using Infra.Options;
using Infra.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.OpenApi.Models;

namespace IAcademyAPI.Infra.APIConfigurations;

public static class AppConfiguration
{
    public static IServiceCollection AddSwagger(this IServiceCollection services)
    {
        services.AddApiVersioning(opt =>
        {
            opt.DefaultApiVersion = new ApiVersion(1, 0);
            opt.AssumeDefaultVersionWhenUnspecified = true;
            opt.ReportApiVersions = true;
        });

        services.AddVersionedApiExplorer(options =>
        {
            options.GroupNameFormat = "'v'VVV";
            options.SubstituteApiVersionInUrl = true;
        });

        services.AddSwaggerGen(c =>
        {
            c.SwaggerDoc("v1", new OpenApiInfo { Title = "IAcademy API", Version = "v1" });
        });

        return services;
    }

    public static IServiceCollection AddOptions(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<DatabaseInstanceOptions>(x =>
        {
            x.Name = configuration.GetValue<string>($"IAcademy:Mongo:{DatabaseInstanceOptions.DatabaseNameConfigKey}");
            x.ConnectionString = configuration.GetValue<string>($"IAcademy:Mongo:{DatabaseInstanceOptions.ConnectionStringConfigKey}");
        });

        return services;
    }

    public static IServiceCollection AddRepositories(this IServiceCollection services)
    {
        services.AddScoped<ISummaryRepository, SummaryRepository>();
        services.AddScoped<IContentRepository, ContentRepository>();
        services.AddScoped<IExerciseRepository, ExerciseRepository>();
        services.AddScoped<ICorrectionRepository, CorrectionRepository>();
        services.AddScoped<IConfigurationRepository, ConfigurationRepository>();

        services.AddSingleton<DbContext>();

        return services;
    }
}