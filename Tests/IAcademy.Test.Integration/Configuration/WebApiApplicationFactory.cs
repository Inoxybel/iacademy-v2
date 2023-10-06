using IAcademyAPI;
using Infra.Options;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace IAcademy.Test.Integration.Configuration;

public class WebApiApplicationFactory : WebApplicationFactory<Program>
{
    protected override IHost CreateHost(IHostBuilder builder)
    {
        builder.ConfigureHostConfiguration(config =>
        {
            var launchSettingsConfig = new ConfigurationBuilder()
                .AddJsonFile("Properties/launchSettings.json")
                .Build();

            Environment.SetEnvironmentVariable("ASPNETCORE_ENVIRONMENT", "Development");
        });

        return base.CreateHost(builder);
    }

    public static void ConfigureDatabaseInstanceOptions(IServiceCollection services)
    {
        services.Configure<DatabaseInstanceOptions>(options =>
        {
            options.ConnectionString = "mongodb://localhost:27017";
            options.Name = "IAcademyDB";
            options.UserManagerDBName = "IAcademyUsersDB";
            options.UserManagerConnectionString = "mongodb://localhost:27017";
        });
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            ConfigureDatabaseInstanceOptions(services);
            // services.AddServiceWithFaker<CLASSE>();
        });
    }
}