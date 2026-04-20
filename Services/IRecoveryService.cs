namespace CNPM.Services;

public interface IRecoveryService
{
    Task<string> BackupNowAsync(CancellationToken cancellationToken = default);
    Task<(bool Success, string Message)> RestoreLatestAsync(CancellationToken cancellationToken = default);
    Task<DateTime?> GetLatestBackupTimeUtcAsync(CancellationToken cancellationToken = default);
}
