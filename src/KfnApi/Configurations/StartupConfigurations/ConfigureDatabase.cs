using KfnApi.Helpers.Database;
using KfnApi.Models.Settings;
using Microsoft.EntityFrameworkCore;

namespace KfnApi.Configurations.StartupConfigurations;

public static partial class StartupConfigurations
{
    public static IServiceCollection ConfigureDatabase(this IServiceCollection services, IConfiguration configuration)
    {
        configuration = configuration.GetRequiredSection(PostgresOptions.SectionName);

        services
            .AddOptions<PostgresOptions>()
            .Bind(configuration)
            .ValidateDataAnnotations()
            .ValidateOnStart();

        var postgresOptions =configuration.Get<PostgresOptions>();

        services.AddDbContext<DatabaseContext>(options =>
            {
                options.UseNpgsql(postgresOptions!.ConnectionString, o =>
                    o.UseQuerySplittingBehavior(QuerySplittingBehavior.SplitQuery));
            });

        return services;
    }
}
