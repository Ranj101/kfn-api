using KfnApi.Abstractions;
using KfnApi.Helpers.Database;

namespace KfnApi.Services.Tasks;

public class UploadEraser : IHostedService, IDisposable
{
    private readonly ILogger<UploadEraser> _logger;
    private readonly IHostEnvironment _environment;
    private readonly IServiceProvider _serviceProvider;
    private Int128 _iteration;
    private Timer? _timer;

    public UploadEraser(IServiceProvider serviceProvider, IHostEnvironment environment, ILogger<UploadEraser> logger)
    {
        _logger = logger;
        _environment = environment;
        _serviceProvider = serviceProvider;
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        if(string.Equals(_environment.EnvironmentName, "Testing", StringComparison.OrdinalIgnoreCase))
            return Task.CompletedTask;

        _logger.LogInformation("Upload collection service initialized");

        _timer = new Timer(ClearUnlinkedUploads, null, TimeSpan.Zero, TimeSpan.FromMinutes(30));
        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Upload collection service shutting down");

        _timer!.Change(Timeout.Infinite, 0);
        return Task.CompletedTask;
    }

    public void Dispose()
    {
        _timer?.Dispose();
    }

    private async void ClearUnlinkedUploads(object? state)
    {
        using var scope = _serviceProvider.CreateScope();
        var databaseContext = scope.ServiceProvider.GetRequiredService<DatabaseContext>();
        var cloudService = scope.ServiceProvider.GetRequiredService<ICloudStorageService>();

        var uploads = databaseContext.Uploads
            .Where(u => u.Users!.Count == 0
                        && u.Producers!.Count == 0
                        && u.Products!.Count == 0
                        && u.ApprovalForms!.Count == 0)
            .ToList();

        foreach (var upload in uploads)
        {
            await cloudService.DeleteObjectAsync(upload.Key.ToString());
            databaseContext.Uploads.Remove(upload);
            await databaseContext.SaveChangesAsync();
        }

        var totalMBytes = uploads.Aggregate(0.0, (current, upload) => current + upload.Size / 1024f) / 1024f;
        _logger.LogInformation("({Iteration}) Erased: {TotalSize} MB of unlinked uploads", ++_iteration, totalMBytes.ToString("0.00"));
    }
}
