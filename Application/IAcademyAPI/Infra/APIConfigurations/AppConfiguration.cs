using CrossCutting.Constants;
using Domain.Infra;
using Infra;
using Infra.Cache;
using Infra.Cache.Interfaces;
using Infra.Cache.Options;
using Infra.Cache.Repositories;
using Infra.Options;
using Infra.Repositories;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Reflection;
using System.Text;

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
            c.SwaggerDoc("v1", new OpenApiInfo { Title = $"{AppConstants.AppName} API", Version = "v1" });

            c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                Description = "JWT Authorization header using the Bearer scheme. Example: \"Authorization: Bearer {token}\"",
                Name = "Authorization",
                In = ParameterLocation.Header,
                Type = SecuritySchemeType.ApiKey,
                Scheme = "Bearer"
            });

            c.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                        {
                            Type = ReferenceType.SecurityScheme,
                            Id = "Bearer"
                        }
                    },
                    Array.Empty<string>()
                }
            });

            c.EnableAnnotations();

            var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
            var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
            c.IncludeXmlComments(xmlPath);
        });

        return services;
    }

    public static IServiceCollection AddOptions(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<DatabaseInstanceOptions>(x =>
        {
            x.Name = configuration.GetValue<string>($"Mongo:{DatabaseInstanceOptions.DatabaseNameConfigKey}");
            x.ConnectionString = configuration.GetValue<string>($"Mongo:{DatabaseInstanceOptions.ConnectionStringConfigKey}");
            x.UserManagerConnectionString = configuration.GetValue<string>($"{DatabaseInstanceOptions.UserManagerContext}:Mongo:{DatabaseInstanceOptions.ConnectionStringConfigKey}");
            x.UserManagerDBName = configuration.GetValue<string>($"{DatabaseInstanceOptions.UserManagerContext}:Mongo:{DatabaseInstanceOptions.DatabaseNameConfigKey}");
        });

        ConfigureJWT(services, configuration);

        services.Configure<CacheOptions>(x =>
        {
            x.ExpirationSeconds = configuration.GetValue<int>($"Redis:ExpirationInSeconds", 60);
        });

        return services;
    }

    public static IServiceCollection AddRepositories(this IServiceCollection services)
    {  
        services.AddScoped<ISummaryRepository, SummaryRepository>();
        services.AddScoped<IContentRepository, ContentRepository>();
        services.AddScoped<ICompanyRepository, CompanyRepository>();
        services.AddScoped<IExerciseRepository, ExerciseRepository>();
        services.AddScoped<ICorrectionRepository, CorrectionRepository>();
        services.AddScoped<IConfigurationRepository, ConfigurationRepository>();
        services.AddScoped<IChatCompletionsRepository, ChatCompletionsRepository>();

        services.AddSingleton<DbContext>();

        return services;
    }

    public static IServiceCollection AddCacheRepositories(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddSingleton<IRedisConnectionManager, RedisConnectionManager>(c => new RedisConnectionManager(
           configuration["Redis:ConnectionString"])
        );

        services.Decorate<ISummaryRepository, SummaryCacheRepository>();
        services.Decorate<ICompanyRepository, CompanyCacheRepository>();
        services.Decorate<IConfigurationRepository, ConfigurationCacheRepository>();

        return services;
    }

    private static void ConfigureJWT(IServiceCollection services, IConfiguration configuration)
    {
        var key = Encoding.ASCII.GetBytes(configuration.GetValue<string>("JwtSettings:SecretKey"));

        services.AddAuthentication(x =>
        {
            x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        })
        .AddJwtBearer(x =>
        {
            x.RequireHttpsMetadata = false;
            x.SaveToken = true;
            x.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ValidateIssuer = false,
                ValidateAudience = false
            };
        });
    }
}