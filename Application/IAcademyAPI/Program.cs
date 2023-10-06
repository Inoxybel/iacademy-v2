using System.Text.Json.Serialization;
using CrossCutting.Constants;
using Domain.Services;
using Domain.Validators;
using FluentValidation;
using FluentValidation.AspNetCore;
using IAcademyAPI.Infra.APIConfigurations;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Service;
using Service.Integrations.OpenAI.Configuration;

namespace IAcademyAPI;

public class Program
{
    protected Program(){}

    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        if (builder.Configuration["ASPNETCORE_ENVIRONMENT"] != "Development")
        {
            builder.Configuration.AddAzureAppConfiguration(options =>
            {
                options.Connect(Environment.GetEnvironmentVariable("AppConfigConnectionString"))
                    .TrimKeyPrefix($"{AppConstants.AppName}:")
                    .Select($"{AppConstants.AppName}:Mongo:*")
                    .Select($"{AppConstants.AppName}:Redis:*")
                    .Select($"{AppConstants.AppName}:UserManager:Mongo:*")
                    .Select($"{AppConstants.AppName}:ExternalServices:*")
                    .Select($"{AppConstants.AppName}:JwtSettings:*")                   
                    .ConfigureRefresh(refreshOptions =>
                    {
                        refreshOptions.SetCacheExpiration(TimeSpan.FromMinutes(1));
                        refreshOptions.Register($"{AppConstants.AppName}", true);
                    });
            });
        }

        builder.WebHost.ConfigureKestrel(options =>
        {
            options.Limits.MaxRequestBodySize = null;
        });

        ConfigureServices(builder.Services, builder.Configuration);

        var app = builder.Build();

        ConfigureApp(app, builder.Configuration);

        app.Run();
    }

    public static void ConfigureServices(IServiceCollection services, IConfiguration configuration)
    {
        MongoConfiguration.RegisterConfigurations();

        services.AddScoped<ISummaryService, SummaryService>();
        services.AddScoped<IContentService, ContentService>();
        services.AddScoped<IExerciseService, ExerciseService>();
        services.AddScoped<IGeneratorService, GeneratorService>();
        services.AddScoped<ICorrectionService, CorrectionService>();
        services.AddScoped<IConfigurationService, ConfigurationService>();
        services.AddScoped<IChatCompletionsService, ChatCompletionsService>();
        services.AddScoped<ICompanyService, CompanyService>();

        services.Configure<ApiBehaviorOptions>(options =>
        {
            options.SuppressModelStateInvalidFilter = true;
        });

        services
            .AddFluentValidationAutoValidation()
            .AddValidatorsFromAssemblyContaining<SummaryCreationRequestValidator>();

        services
            .AddControllers()
            .ConfigureApiBehaviorOptions(options =>
            {
                options.SuppressMapClientErrors = false;
                options.SuppressModelStateInvalidFilter = true;
            })
            .AddJsonOptions(options =>
            {
                options.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
                options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
            });

        services
            .AddAuthorization(options =>
            {
                options.DefaultPolicy = new AuthorizationPolicyBuilder(JwtBearerDefaults.AuthenticationScheme)
                    .RequireAuthenticatedUser()
                    .Build();
            });

        services
            .AddSwagger()
            .AddOptions(configuration)
            .AddOpenAIService(configuration)
            .AddRepositories()
            .AddCacheRepositories(configuration)
            .AddHealthChecks()
            .AddMongoDb(configuration["MongoDB:ConnectionString"], name: "health-check-mongodb");
    }

    public static void ConfigureApp(WebApplication app, IConfiguration configuration)
    {

        app.UseSwagger();
        app.UseSwaggerUI(c =>
        {
            c.SwaggerEndpoint("/swagger/v1/swagger.json", "V1");
            c.RoutePrefix = "swagger";
            c.DocExpansion(Swashbuckle.AspNetCore.SwaggerUI.DocExpansion.List);
            c.OAuthUseBasicAuthenticationWithAccessCodeGrant();
        });

        app.UseAuthentication();
        app.UseAuthorization();
        app.MapControllers();
    }
}
