using System.Text.Json.Serialization;
using Domain.Services;
using IAcademyAPI.Infra.APIConfigurations;
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
                    .Select("IAcademy:Mongo:*")
                    .Select("IAcademy:ExternalServices:*")
                    .ConfigureRefresh(refreshOptions =>
                    {
                        refreshOptions.SetCacheExpiration(TimeSpan.FromMinutes(1));
                        refreshOptions.Register("IAcademy", true);
                    });
            });
        }

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

        services
            .AddControllers()
            .ConfigureApiBehaviorOptions(options =>
            {
                options.SuppressMapClientErrors = false;
            })
            .AddJsonOptions(options =>
            {
                options.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
            });
            
        services.Configure<ApiBehaviorOptions>(options =>
        {
            options.SuppressModelStateInvalidFilter = true;
        });

        services
            .AddSwagger()
            .AddOptions(configuration)
            .AddOpenAIService(configuration)
            .AddRepositories()
            .AddHealthChecks()
            .AddMongoDb(configuration["IAcademy:MongoDB:ConnectionString"], name: "health-check-mongodb");
    }

    public static void ConfigureApp(WebApplication app, IConfiguration configuration)
    {

        app.UseSwagger();
        app.UseSwaggerUI(c =>
        {
            c.SwaggerEndpoint("/swagger/v1/swagger.json", "V1");
            c.RoutePrefix = "swagger";
            c.DocExpansion(Swashbuckle.AspNetCore.SwaggerUI.DocExpansion.List);
        });


        //app.UseHttpsRedirection();
        app.UseAuthorization();
        app.MapControllers();
    }
}
