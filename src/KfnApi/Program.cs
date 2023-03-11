using KfnApi.Configurations.StartupConfigurations;

var builder = WebApplication.CreateBuilder(args);
var services = builder.Services;
var configuration = builder.Configuration;

services.AddControllers();
services.AddEndpointsApiExplorer();

services.ConfigureSwagger()
        .ConfigureDependencies()
        .ConfigureAuthorization();

services.ConfigureCache(configuration)
        .ConfigureDatabase(configuration)
        .ConfigureAuthentication(configuration);

services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.WithOrigins("https://localhost:7227")
            .AllowAnyMethod()
            .AllowAnyHeader();
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors();
app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
app.Run();
