using KfnApi.Helpers.Authentication;
using KfnApi.Helpers.Authorization.Policy;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Policy;

namespace KfnApi.Configurations.StartupConfigurations;

public static partial class StartupConfigurations
{
    public static IServiceCollection ConfigureAuthorization(this IServiceCollection service)
    {
        service.AddAuthorization(options => { options.InvokeHandlersAfterFailure = false; });

        service.AddSingleton<IAuthorizationPolicyProvider, PolicyProvider>()
               .AddSingleton<IAuthorizationMiddlewareResultHandler, AuthorizationResultHandler>()
               .AddScoped<IAuthorizationHandler, AuthorizationRequirementHandler>()
               .AddTransient<IPolicyEvaluator, UserPolicyEvaluator>();

        return service;
    }
}
