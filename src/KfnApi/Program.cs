using KfnApi.Configurations.StartupConfigurations;
using KfnApi.Helpers.Database;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);
var services = builder.Services;
var configuration = builder.Configuration;

var migrationConfig = configuration.GetRequiredSection("RunMigration").Value;

// Run database migrations if runMigration is true.
if (bool.TryParse(migrationConfig, out var runMigration) && runMigration)
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

services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.AllowAnyOrigin()
            .AllowAnyMethod()
            .AllowAnyHeader();
    });
});

var app = builder.Build();

app.UseCors();
// Configure the HTTP request pipeline.
app.UseSwagger();
app.UseSwaggerUI();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.Run();

#pragma warning disable CA1050
public sealed partial class Program { }
