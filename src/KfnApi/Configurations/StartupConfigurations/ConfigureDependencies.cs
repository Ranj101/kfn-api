using KfnApi.Abstractions;
using KfnApi.Helpers.Authorization;
using KfnApi.Services;
using KfnApi.Services.Tasks;

namespace KfnApi.Configurations.StartupConfigurations;

public partial class StartupConfigurations
{
    public static IServiceCollection ConfigureDependencies(this IServiceCollection services)
    {
        services.AddScoped<ISelfService, SelfService>();
        services.AddScoped<IUserService, UserService>();
        services.AddScoped<IAuthContext, AuthContext>();
        services.AddScoped<IOrderService, OrderService>();
        services.AddScoped<IUploadService, UploadService>();
        services.AddScoped<IReportService, ReportService>();
        services.AddScoped<IProductService, ProductService>();
        services.AddScoped<IProducerService, ProducerService>();
        services.AddScoped<IWorkflowService, WorkflowService>();
        services.AddScoped<IRemoteUserService, RemoteUserService>();
        services.AddScoped<IPermissionService, PermissionService>();
        services.AddScoped<IApprovalFormService, ApprovalFormService>();

        services.AddSingleton<WorkflowContext>();
        services.AddSingleton<IRoleMap, RoleMap>();
        services.AddSingleton<ICloudStorageService, CloudStorageService>();

        services.AddHostedService<UploadEraser>();

        return services;
    }
}
