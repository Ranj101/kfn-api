using KfnApi.Configurations.StartupConfigurations;
using KfnApi.Helpers.Database;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);
var services = builder.Services;
var configuration = builder.Configuration;

// Run database migrations if --migrate is passed as an argument.
if (args.Contains("--migrate"))
{
    builder.Services.ConfigureDatabase(builder.Configuration);
    var migrationRunner = builder.Build();
    var logger = migrationRunner.Logger;
    logger.LogInformation("migration initiated");
    try
    {
        await using var scope = migrationRunner.Services.CreateAsyncScope();
        var db = scope.ServiceProvider.GetRequiredService<DatabaseContext>();
        await db.Database.MigrateAsync();
        logger.LogInformation("migration successful");
    }
    catch (Exception ex)
    {
        logger.LogError(ex, "migration failed");
    }

    return;
}

services.AddControllers();
services.AddEndpointsApiExplorer();

// Configure services.
services.ConfigureSwagger()
        .ConfigureControllers()
        .ConfigureDependencies()
        .ConfigureAuthorization();

services.ConfigureCache(configuration)
        .ConfigureDatabase(configuration)
        .ConfigureCloudStorage(configuration)
        .ConfigureAuthentication(configuration);

var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseSwagger();
app.UseSwaggerUI();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.Run();

#pragma warning disable CA1050
public sealed partial class Program { }
