using IntegrationTests.Abstractions;
using IntegrationTests.Auth;
using IntegrationTests.Models;
using KfnApi;
using KfnApi.Helpers.Authentication;
using KfnApi.Helpers.Database;
using KfnApi.Models.Settings;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using TokenHandler = IntegrationTests.Auth.TokenHandler;

namespace IntegrationTests;

public class TestApplicationFactory : WebApplicationFactory<Program>
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder
            .UseEnvironment("Testing")
            .UseTestServer()
            .ConfigureTestServices(ServiceConfigs);
    }

    private static void ServiceConfigs(IServiceCollection services)
    {
        RemoveDescriptors(services);

        var configuration = services.BuildServiceProvider().GetRequiredService<IConfiguration>();
        ConfigureDatabase(services, configuration);
        ConfigureAuthentication(services, configuration);

        var serviceProvider = services.BuildServiceProvider();
        using var scope = serviceProvider.CreateScope();

        using var databaseContext = scope.ServiceProvider.GetRequiredService<DatabaseContext>();
        databaseContext.Database.Migrate();
    }

    private static void ConfigureAuthentication(IServiceCollection services, IConfiguration configuration)
    {
        configuration = configuration.GetRequiredSection(TokenSettings.SectionName);

        services.AddOptions<TokenSettings>()
            .Bind(configuration)
            .ValidateDataAnnotations()
            .ValidateOnStart();

        var tokenSettings = configuration.Get<TokenSettings>()!;
        var signingConfig = new SigningConfigurations();

        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = tokenSettings.Issuer,
                    ValidAudience = tokenSettings.Audience,
                    IssuerSigningKey = signingConfig.GetTokenSecurityKey(tokenSettings.AccessTokenSecret),
                    ClockSkew = TimeSpan.Zero
                };
            })
            .AddScheme<AuthenticationSchemeOptions, UserAuthHandler>(Constants.AuthScheme, _ => {});

        services.AddSingleton<ITokenHandler, TokenHandler>();
    }

    private static void ConfigureDatabase(IServiceCollection services, IConfiguration configuration)
    {
        configuration = configuration.GetRequiredSection(PostgresOptions.SectionName);

        services
            .AddOptions<PostgresOptions>()
            .Bind(configuration)
            .ValidateDataAnnotations()
            .ValidateOnStart();

        var postgresOptions =configuration.Get<PostgresOptions>();
        var variableConnectionString = postgresOptions!.ConnectionString + "_" + Guid.NewGuid().ToString("N");

        services.AddDbContext<DatabaseContext>(options =>
        {
            options.UseNpgsql(variableConnectionString, o =>
                o.UseQuerySplittingBehavior(QuerySplittingBehavior.SplitQuery));
        });
    }

    private static void RemoveDescriptors(IServiceCollection services)
    {
        var descriptors = new List<ServiceDescriptor?>();

        var authDescriptors = services.Where(
            descriptor => descriptor.ServiceType.FullName != null &&
                          descriptor.ServiceType.FullName.Contains("Authentication"));

        descriptors.AddRange(authDescriptors);
        descriptors.Add(services.SingleOrDefault(d => d.ServiceType == typeof(DbContextOptions<DatabaseContext>)));
        foreach (var descriptor in descriptors.Where(descriptor => descriptor != null)) services.Remove(descriptor!);
    }
}
