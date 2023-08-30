using System.Text.Json.Serialization;
using Azure.Identity;
using IAcademyAPI.Infra.APIConfigurations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.OpenApi.Models;

namespace IAcademyAPI;

public static class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Host.ConfigureAppConfiguration((hostingContext, config) =>
        {
            var settings = config.Build();

            if (settings["ASPNETCORE_ENVIRONMENT"] != "Development")
            {
                config.AddAzureAppConfiguration(options =>
                {
                    options.Connect(settings["AppConfigConnectionString"]);
                    options.ConfigureKeyVault(opt =>
                        {
                            opt.SetCredential(new DefaultAzureCredential());
                        })
                        .TrimKeyPrefix("IAcademy")
                        .Select("IAcademy:Mongo:*")
                        .Select("IAcademy:ExternalServices:*")
                        .ConfigureRefresh(config =>
                        {
                            config.SetCacheExpiration(TimeSpan.FromMinutes(1));
                            config.Register("IAcademy", true);
                        });
                });
            }
        });

        ConfigureServices(builder.Services, builder.Configuration);

        var app = builder.Build();

        ConfigureApp(app, builder.Configuration);

        app.Run();
    }

    public static void ConfigureServices(IServiceCollection services, IConfiguration configuration)
    {
        MongoConfiguration.RegisterConfigurations();

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

        services
            .AddSwagger()
            .AddHealthChecks()
            .AddMongoDb(configuration["IAcademy:MongoDB:ConnectionString"], name: "health-check-mongodb");
    }

    public static void ConfigureApp(WebApplication app, IConfiguration configuration)
    {
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "V1");
                c.RoutePrefix = "swagger";
                c.DocExpansion(Swashbuckle.AspNetCore.SwaggerUI.DocExpansion.List);
            });
        }

        app.UseHttpsRedirection();
        app.UseAuthorization();
        app.MapControllers();
    }

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
}
