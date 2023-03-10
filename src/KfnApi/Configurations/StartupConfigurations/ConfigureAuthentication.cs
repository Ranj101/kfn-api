using KfnApi.Helpers.Authentication;
using KfnApi.Models.Settings;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Polly;
using Polly.Extensions.Http;

namespace KfnApi.Configurations.StartupConfigurations;

public static partial class StartupConfigurations
{
    public static IServiceCollection ConfigureAuthentication(this IServiceCollection services, IConfiguration configuration)
    {
        configuration = configuration.GetRequiredSection(AuthOptions.SectionName);

        services
            .AddOptions<AuthOptions>()
            .Bind(configuration)
            .ValidateDataAnnotations()
            .ValidateOnStart();

        var authOptions = configuration.Get<AuthOptions>();

        services
            .AddIdentityAuthentication(authOptions!)
            .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, options =>
            {
                options.Authority = authOptions!.AuthorityUrl;
                options.Audience = authOptions.Audience;
            });

        return services;
    }

    private static IServiceCollection AddIdentityAuthentication(this IServiceCollection services, AuthOptions authOptions)
    {
        services.AddHttpClient(AuthDefaults.IdentityClient, client =>
                client.BaseAddress = new Uri(authOptions.AuthorityUrl))
        .AddPolicyHandler(HttpPolicyExtensions
                .HandleTransientHttpError()
                .WaitAndRetryAsync(3, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt))));

        services.AddScoped<MachineToMachineBearerTokenHandler>();

        services.AddHttpClient(AuthDefaults.IdentityWrapperClient, client =>
                client.BaseAddress = new Uri(authOptions.WrapperUrl))
            .AddHttpMessageHandler<MachineToMachineBearerTokenHandler>()
            .AddPolicyHandler(HttpPolicyExtensions
                    .HandleTransientHttpError()
                    .WaitAndRetryAsync(3, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt))));

        return services;
    }
}
