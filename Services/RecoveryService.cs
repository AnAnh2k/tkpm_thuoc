using System.Text.Json;
using CNPM.Models;
using Microsoft.EntityFrameworkCore;

namespace CNPM.Services;

public class RecoveryService : IRecoveryService
{
    private readonly PharmacyDbContext _context;
    private readonly IWebHostEnvironment _environment;

    public RecoveryService(PharmacyDbContext context, IWebHostEnvironment environment)
    {
        _context = context;
        _environment = environment;
    }

    public async Task<string> BackupNowAsync(CancellationToken cancellationToken = default)
    {
        var backupDir = GetBackupDirectory();
        Directory.CreateDirectory(backupDir);

        var snapshot = new RecoverySnapshot
        {
            CreatedAtUtc = DateTime.UtcNow,
            SanPhams = await _context.TblSanPhams
                .AsNoTracking()
                .Select(sp => new SanPhamSnapshot
                {
                    PkSMaSp = sp.PkSMaSp,
                    STenSp = sp.STenSp,
                    SDonViTinh = sp.SDonViTinh,
                    SHanDung = sp.SHanDung,
                    ISl = sp.ISl,
                    FDonGiaBan = sp.FDonGiaBan,
                    FkSMaLoai = sp.FkSMaLoai,
                    FkSMaNcc = sp.FkSMaNcc
                })
                .ToListAsync(cancellationToken),
            KhachHangs = await _context.TblKhachHangs
                .AsNoTracking()
                .Select(kh => new KhachHangSnapshot
                {
                    PkSMaKh = kh.PkSMaKh,
                    STenKh = kh.STenKh,
                    SSdt = kh.SSdt
                })
                .ToListAsync(cancellationToken)
        };

        var fileName = $"snapshot_{DateTime.UtcNow:yyyyMMdd_HHmmss}.json";
        var filePath = Path.Combine(backupDir, fileName);

        await using var stream = File.Create(filePath);
        await JsonSerializer.SerializeAsync(stream, snapshot, new JsonSerializerOptions { WriteIndented = true }, cancellationToken);
        return fileName;
    }

    public async Task<(bool Success, string Message)> RestoreLatestAsync(CancellationToken cancellationToken = default)
    {
        var latestFile = GetLatestBackupFilePath();
        if (latestFile == null)
        {
            return (false, "Không tìm thấy bản sao lưu để khôi phục.");
        }

        await using var stream = File.OpenRead(latestFile);
        var snapshot = await JsonSerializer.DeserializeAsync<RecoverySnapshot>(stream, cancellationToken: cancellationToken);
        if (snapshot == null)
        {
            return (false, "Không đọc được dữ liệu sao lưu.");
        }

        foreach (var item in snapshot.KhachHangs)
        {
            var existing = await _context.TblKhachHangs.FirstOrDefaultAsync(x => x.PkSMaKh == item.PkSMaKh, cancellationToken);
            if (existing == null)
            {
                _context.TblKhachHangs.Add(new TblKhachHang
                {
                    PkSMaKh = item.PkSMaKh,
                    STenKh = item.STenKh,
                    SSdt = item.SSdt
                });
                continue;
            }

            existing.STenKh = item.STenKh;
            existing.SSdt = item.SSdt;
        }

        foreach (var item in snapshot.SanPhams)
        {
            var existing = await _context.TblSanPhams.FirstOrDefaultAsync(x => x.PkSMaSp == item.PkSMaSp, cancellationToken);
            if (existing == null)
            {
                _context.TblSanPhams.Add(new TblSanPham
                {
                    PkSMaSp = item.PkSMaSp,
                    STenSp = item.STenSp,
                    SDonViTinh = item.SDonViTinh,
                    SHanDung = item.SHanDung,
                    ISl = item.ISl,
                    FDonGiaBan = item.FDonGiaBan,
                    FkSMaLoai = item.FkSMaLoai,
                    FkSMaNcc = item.FkSMaNcc
                });
                continue;
            }

            existing.STenSp = item.STenSp;
            existing.SDonViTinh = item.SDonViTinh;
            existing.SHanDung = item.SHanDung;
            existing.ISl = item.ISl;
            existing.FDonGiaBan = item.FDonGiaBan;
            existing.FkSMaLoai = item.FkSMaLoai;
            existing.FkSMaNcc = item.FkSMaNcc;
        }

        await _context.SaveChangesAsync(cancellationToken);
        return (true, $"Khôi phục dữ liệu từ bản sao lưu: {Path.GetFileName(latestFile)}");
    }

    public Task<DateTime?> GetLatestBackupTimeUtcAsync(CancellationToken cancellationToken = default)
    {
        var latestFile = GetLatestBackupFilePath();
        if (latestFile == null)
        {
            return Task.FromResult<DateTime?>(null);
        }

        var time = File.GetCreationTimeUtc(latestFile);
        return Task.FromResult<DateTime?>(time);
    }

    private string? GetLatestBackupFilePath()
    {
        var backupDir = GetBackupDirectory();
        if (!Directory.Exists(backupDir))
        {
            return null;
        }

        return Directory.GetFiles(backupDir, "snapshot_*.json")
            .OrderByDescending(File.GetCreationTimeUtc)
            .FirstOrDefault();
    }

    private string GetBackupDirectory()
    {
        return Path.Combine(_environment.ContentRootPath, "App_Data", "backups");
    }

    private sealed class RecoverySnapshot
    {
        public DateTime CreatedAtUtc { get; set; }
        public List<SanPhamSnapshot> SanPhams { get; set; } = [];
        public List<KhachHangSnapshot> KhachHangs { get; set; } = [];
    }

    private sealed class SanPhamSnapshot
    {
        public string PkSMaSp { get; set; } = string.Empty;
        public string? STenSp { get; set; }
        public string? SDonViTinh { get; set; }
        public DateOnly? SHanDung { get; set; }
        public int? ISl { get; set; }
        public double? FDonGiaBan { get; set; }
        public string? FkSMaLoai { get; set; }
        public string? FkSMaNcc { get; set; }
    }

    private sealed class KhachHangSnapshot
    {
        public string PkSMaKh { get; set; } = string.Empty;
        public string? STenKh { get; set; }
        public string? SSdt { get; set; }
    }
}
