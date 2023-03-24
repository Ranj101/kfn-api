using KfnApi.Abstractions;
using KfnApi.Helpers.Authorization;
using KfnApi.Services;

namespace KfnApi.Configurations.StartupConfigurations;

public partial class StartupConfigurations
{
    public static IServiceCollection ConfigureDependencies(this IServiceCollection services)
    {
        services.AddScoped<ISelfService, SelfService>();
        services.AddScoped<IUserService, UserService>();
        services.AddScoped<IAuthContext, AuthContext>();
        services.AddScoped<IReportService, ReportService>();
        services.AddScoped<IProducerService, ProducerService>();
        services.AddScoped<IRemoteUserService, RemoteUserService>();
        services.AddScoped<IPermissionService, PermissionService>();

        services.AddSingleton<WorkflowContext>();
        services.AddSingleton<IRoleMap, RoleMap>();

        return services;
    }
}
