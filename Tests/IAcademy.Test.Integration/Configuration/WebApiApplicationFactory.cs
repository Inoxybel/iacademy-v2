using IAcademyAPI;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;

namespace IAcademy.Test.Integration.Configuration;

public class WebApiApplicationFactory : WebApplicationFactory<Program>
{
    protected override IHost CreateHost(IHostBuilder builder)
    {
        builder.ConfigureHostConfiguration(config =>
        {
            var integrationAppSettings = new ConfigurationBuilder()
                .AddJsonFile("appsettings.Integration.json")
                .Build();

            config.AddConfiguration(integrationAppSettings);

            var launchSettingsConfig = new ConfigurationBuilder()
                .AddJsonFile("Properties/launchSettings.json")
                .Build();

            var envVariables = launchSettingsConfig.GetSection("profiles:IAcademy.Test.Integration:environmentVariables").GetChildren();
            foreach (var item in envVariables)
            {
                Environment.SetEnvironmentVariable(item.Key, item.Value);
            }
        });
        return base.CreateHost(builder);
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(service =>
        {
            // service.AddServiceWithFaker<CLASSE>();
        });
    }
}