namespace CNPM.Services;

public class RecoveryBackgroundService : BackgroundService
{
    private static readonly TimeSpan BackupInterval = TimeSpan.FromHours(2);

    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<RecoveryBackgroundService> _logger;

    public RecoveryBackgroundService(IServiceScopeFactory scopeFactory, ILogger<RecoveryBackgroundService> logger)
    {
        _scopeFactory = scopeFactory;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await BackupNow(stoppingToken);

        using var timer = new PeriodicTimer(BackupInterval);
        while (!stoppingToken.IsCancellationRequested && await timer.WaitForNextTickAsync(stoppingToken))
        {
            await BackupNow(stoppingToken);
        }
    }

    private async Task BackupNow(CancellationToken cancellationToken)
    {
        try
        {
            using var scope = _scopeFactory.CreateScope();
            var recoveryService = scope.ServiceProvider.GetRequiredService<IRecoveryService>();
            var fileName = await recoveryService.BackupNowAsync(cancellationToken);
            _logger.LogInformation("Đã tạo backup dữ liệu: {FileName}", fileName);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Không thể tạo backup dữ liệu định kỳ.");
        }
    }
}
