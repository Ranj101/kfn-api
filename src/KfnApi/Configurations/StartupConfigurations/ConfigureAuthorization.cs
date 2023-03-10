using KfnApi.Helpers.Authorization.Policy;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;

namespace KfnApi.Configurations.StartupConfigurations;

public static partial class StartupConfigurations
{
    public static IServiceCollection ConfigureAuthorization(this IServiceCollection service)
    {
        service.AddAuthorization(options =>
        {
            var builder = new AuthorizationPolicyBuilder(JwtBearerDefaults.AuthenticationScheme, Constants.AuthScheme);
            builder = builder.RequireAuthenticatedUser();
            options.DefaultPolicy = builder.Build();
        });

        service.AddSingleton<IAuthorizationPolicyProvider, PolicyProvider>()
               .AddScoped<IAuthorizationHandler, AuthorizationRequirementHandler>();

        return service;
    }
}
