using System.Security.Cryptography;
using System.Text;
using CNPM.Infrastructure.Repositories;
using CNPM.Models;
using CNPM.Services.Dtos;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;

namespace CNPM.Services;

public class AuthService : IAuthService
{
    private const int MaxFailedAttempts = 5;
    private static readonly TimeSpan LockoutDuration = TimeSpan.FromMinutes(15);

    private readonly PharmacyDbContext _context;
    private readonly IRepository<TblTaiKhoan> _taiKhoanRepository;
    private readonly IMemoryCache _memoryCache;

    public AuthService(
        PharmacyDbContext context,
        IRepository<TblTaiKhoan> taiKhoanRepository,
        IMemoryCache memoryCache)
    {
        _context = context;
        _taiKhoanRepository = taiKhoanRepository;
        _memoryCache = memoryCache;
    }

    public async Task<LoginResultDto> LoginAsync(string username, string password)
    {
        if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
        {
            return new LoginResultDto { Success = false, ErrorMessage = "Vui lòng nhập tên đăng nhập và mật khẩu." };
        }

        var lockKey = $"auth-lock-{username.Trim().ToLowerInvariant()}";
        var lockState = _memoryCache.Get<LoginLockState>(lockKey) ?? new LoginLockState();

        if (lockState.LockedUntilUtc.HasValue && lockState.LockedUntilUtc.Value > DateTimeOffset.UtcNow)
        {
            var remain = (int)Math.Ceiling((lockState.LockedUntilUtc.Value - DateTimeOffset.UtcNow).TotalMinutes);
            return new LoginResultDto
            {
                Success = false,
                ErrorMessage = $"Tài khoản bị tạm khóa. Vui lòng thử lại sau {Math.Max(remain, 1)} phút."
            };
        }

        var taiKhoan = await _taiKhoanRepository.Query()
            .Include(tk => tk.FkSMaQuyenNavigation)
            .Include(tk => tk.TblNhanViens)
            .FirstOrDefaultAsync(tk => tk.STenTk == username);

        if (taiKhoan == null || !VerifyPassword(password, taiKhoan.SMk))
        {
            lockState.FailedCount++;
            if (lockState.FailedCount >= MaxFailedAttempts)
            {
                lockState.LockedUntilUtc = DateTimeOffset.UtcNow.Add(LockoutDuration);
                lockState.FailedCount = 0;
            }

            _memoryCache.Set(lockKey, lockState, TimeSpan.FromHours(1));

            return new LoginResultDto
            {
                Success = false,
                ErrorMessage = lockState.LockedUntilUtc.HasValue && lockState.LockedUntilUtc > DateTimeOffset.UtcNow
                    ? "Tài khoản bị tạm khóa 15 phút do đăng nhập sai quá nhiều lần."
                    : "Thông tin đăng nhập không đúng."
            };
        }

        if (taiKhoan.TblNhanViens.Any() && !taiKhoan.TblNhanViens.Any(nv => nv.BTrangThai == true))
        {
            return new LoginResultDto
            {
                Success = false,
                ErrorMessage = "Tài khoản không còn hoạt động."
            };
        }

        if (taiKhoan.SMk == password)
        {
            taiKhoan.SMk = HashPassword(password);
            await _context.SaveChangesAsync();
        }

        _memoryCache.Remove(lockKey);

        return new LoginResultDto
        {
            Success = true,
            UserName = taiKhoan.STenTk ?? string.Empty,
            MaTaiKhoan = taiKhoan.PkSMaTk,
            MaNhanVien = taiKhoan.TblNhanViens.Select(nv => nv.PkSMaNv).FirstOrDefault(),
            Role = taiKhoan.FkSMaQuyen ?? "Unknown"
        };
    }

    public async Task<RegisterResultDto> RegisterAsync(string tenTK, string matKhau)
    {
        if (string.IsNullOrWhiteSpace(tenTK) || string.IsNullOrWhiteSpace(matKhau))
        {
            return new RegisterResultDto
            {
                Success = false,
                ErrorMessage = "Vui lòng điền đầy đủ tên tài khoản và mật khẩu!"
            };
        }

        if (matKhau.Length < 8)
        {
            return new RegisterResultDto
            {
                Success = false,
                ErrorMessage = "Mật khẩu phải có ít nhất 8 ký tự."
            };
        }

        if (await _taiKhoanRepository.AnyAsync(tk => tk.STenTk == tenTK))
        {
            return new RegisterResultDto
            {
                Success = false,
                ErrorMessage = "Tên tài khoản đã được sử dụng!"
            };
        }

        string maTK;
        var count = 1;
        do
        {
            maTK = $"TK{count:000}";
            count++;
        } while (await _taiKhoanRepository.AnyAsync(tk => tk.PkSMaTk == maTK));

        await _taiKhoanRepository.AddAsync(new TblTaiKhoan
        {
            PkSMaTk = maTK,
            STenTk = tenTK,
            SMk = HashPassword(matKhau),
            FkSMaQuyen = null
        });

        await _context.SaveChangesAsync();

        return new RegisterResultDto { Success = true };
    }

    private static string HashPassword(string plainText)
    {
        using var sha = SHA256.Create();
        var bytes = sha.ComputeHash(Encoding.UTF8.GetBytes(plainText));
        var fullHex = Convert.ToHexString(bytes);
        return fullHex[..20];
    }

    private static bool VerifyPassword(string inputPassword, string? storedPassword)
    {
        if (string.IsNullOrEmpty(storedPassword))
        {
            return false;
        }

        var inputHash = HashPassword(inputPassword);
        return string.Equals(storedPassword, inputHash, StringComparison.OrdinalIgnoreCase)
               || string.Equals(storedPassword, inputPassword, StringComparison.Ordinal);
    }

    private sealed class LoginLockState
    {
        public int FailedCount { get; set; }
        public DateTimeOffset? LockedUntilUtc { get; set; }
    }
}
