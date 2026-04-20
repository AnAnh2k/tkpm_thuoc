using System.Diagnostics;
using CNPM.Models;
using CNPM.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CNPM.Controllers;

/// <summary>
/// Endpoint phục vụ đo lường NFR (hiệu năng truy vấn CSDL) cho Chương 5 — không thay thế kiểm thử tải chuyên nghiệp.
/// </summary>
[AllowAnonymous]
public class NfrMetricsController : Controller
{
    private readonly PharmacyDbContext _context;
    private readonly IRecoveryService _recoveryService;

    public NfrMetricsController(PharmacyDbContext context, IRecoveryService recoveryService)
    {
        _context = context;
        _recoveryService = recoveryService;
    }

    /// <summary>
    /// Đo thời gian một truy vấn đơn giản tới SQL Server (độ trễ đọc CSDL).
    /// </summary>
    [HttpGet]
    [Route("metrics/db-probe")]
    public async Task<IActionResult> DbProbe()
    {
        var sw = Stopwatch.StartNew();
        _ = await _context.TblSanPhams.AsNoTracking().Take(1).CountAsync();
        sw.Stop();
        return Json(new
        {
            databaseRoundTripMs = sw.ElapsedMilliseconds,
            measuredAtUtc = DateTime.UtcNow
        });
    }

    [HttpGet]
    [Route("metrics/recovery/status")]
    public async Task<IActionResult> RecoveryStatus()
    {
        var latestBackupUtc = await _recoveryService.GetLatestBackupTimeUtcAsync();
        return Json(new
        {
            targetRtoHours = 2,
            backupIntervalHours = 2,
            latestBackupUtc
        });
    }

    [Authorize]
    [HttpPost]
    [Route("metrics/recovery/backup-now")]
    public async Task<IActionResult> BackupNow()
    {
        var fileName = await _recoveryService.BackupNowAsync();
        return Json(new
        {
            success = true,
            message = "Đã tạo bản sao lưu dữ liệu.",
            fileName
        });
    }

    [Authorize]
    [HttpPost]
    [Route("metrics/recovery/restore-last")]
    public async Task<IActionResult> RestoreLastBackup()
    {
        var result = await _recoveryService.RestoreLatestAsync();
        return Json(new
        {
            success = result.Success,
            message = result.Message
        });
    }
}
