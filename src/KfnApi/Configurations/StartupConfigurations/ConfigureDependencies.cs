using KfnApi.Abstractions;
using KfnApi.Helpers.Authorization;
using KfnApi.Services;

namespace KfnApi.Configurations.StartupConfigurations;

public partial class StartupConfigurations
{
    public static IServiceCollection ConfigureDependencies(this IServiceCollection services)
    {
        services.AddScoped<IUserService, UserService>();
        services.AddScoped<IAuthContext, AuthContext>();
        services.AddScoped<IRemoteUserService, RemoteUserService>();

        services.AddSingleton<IRoleMap, RoleMap>();

        return services;
    }
}
