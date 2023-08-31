using System.Text.Json.Serialization;
using Azure.Identity;
using IAcademyAPI.Infra.APIConfigurations;

namespace IAcademyAPI;

public class Program
{
    protected Program(){}

    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Host.ConfigureAppConfiguration((hostingContext, config) =>
        {
            var settings = config.Build();

            var x = settings["ASPNETCORE_ENVIRONMENT"];

            if (settings["ASPNETCORE_ENVIRONMENT"] != "Development")
            {
                config.AddAzureAppConfiguration(options =>
                {
                    options.Connect(settings["c"]);
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
            .AddOptions(configuration)
            .AddRepositories()
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
}
