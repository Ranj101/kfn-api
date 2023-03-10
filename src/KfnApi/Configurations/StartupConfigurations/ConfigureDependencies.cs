using KfnApi.Abstractions;
using KfnApi.Services;

namespace KfnApi.Configurations.StartupConfigurations;

public partial class StartupConfigurations
{
    public static IServiceCollection ConfigureDependencies(this IServiceCollection services)
    {
        services.AddScoped<IUserService, UserService>();
        services.AddScoped<IRemoteUserService, RemoteUserService>();

        return services;
    }
}
