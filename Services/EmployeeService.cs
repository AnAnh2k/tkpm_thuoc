using System.Security.Cryptography;
using System.Text;
using CNPM.Infrastructure.Repositories;
using CNPM.Models;
using CNPM.Services.Dtos;
using Microsoft.EntityFrameworkCore;

namespace CNPM.Services;

public class EmployeeService : IEmployeeService
{
    private readonly PharmacyDbContext _context;
    private readonly IRepository<TblNhanVien> _nhanVienRepository;
    private readonly IRepository<TblTaiKhoan> _taiKhoanRepository;
    private readonly IRepository<TblChucVu> _chucVuRepository;

    public EmployeeService(
        PharmacyDbContext context,
        IRepository<TblNhanVien> nhanVienRepository,
        IRepository<TblTaiKhoan> taiKhoanRepository,
        IRepository<TblChucVu> chucVuRepository)
    {
        _context = context;
        _nhanVienRepository = nhanVienRepository;
        _taiKhoanRepository = taiKhoanRepository;
        _chucVuRepository = chucVuRepository;
    }

    public Task<List<EmployeeListItemDto>> GetEmployeeListAsync()
    {
        return _nhanVienRepository.Query()
            .AsNoTracking()
            .Include(nv => nv.FkSMaTkNavigation)
            .Include(nv => nv.FkSMaCvNavigation)
            .Select(nv => new EmployeeListItemDto
            {
                PkSMaNv = nv.PkSMaNv,
                SHoTen = nv.SHoTen,
                DNgaySinh = nv.DNgaySinh,
                SCccd = nv.SCccd,
                SSdt = nv.SSdt,
                DNgayVaoLam = nv.DNgayVaoLam,
                FkSMaTk = nv.FkSMaTk,
                TaiKhoan = nv.FkSMaTkNavigation != null ? nv.FkSMaTkNavigation.STenTk : null,
                FkSMaCv = nv.FkSMaCv,
                ChucVu = nv.FkSMaCvNavigation != null ? nv.FkSMaCvNavigation.STenCv : null,
                BTrangThai = nv.BTrangThai
            })
            .ToListAsync();
    }

    public async Task<OperationResultDto> AddEmployeeAsync(TblNhanVien nhanVien)
    {
        if (await _nhanVienRepository.AnyAsync(nv => nv.PkSMaNv == nhanVien.PkSMaNv))
        {
            return new OperationResultDto { Success = false, Message = "Mã nhân viên đã tồn tại!" };
        }

        await _nhanVienRepository.AddAsync(nhanVien);
        await _context.SaveChangesAsync();
        return new OperationResultDto { Success = true, Message = "Thêm nhân viên thành công!" };
    }

    public async Task<OperationResultDto> CreateEmployeeWithAccountAsync(EmployeeAccountCreateDto request)
    {
        if (string.IsNullOrWhiteSpace(request.STenTk) || string.IsNullOrWhiteSpace(request.SMk) || string.IsNullOrWhiteSpace(request.PkSMaNv))
        {
            return new OperationResultDto { Success = false, Message = "Thiếu thông tin bắt buộc." };
        }

        if (request.SMk.Length < 8)
        {
            return new OperationResultDto { Success = false, Message = "Mật khẩu phải có ít nhất 8 ký tự." };
        }

        if (await _taiKhoanRepository.AnyAsync(t => t.STenTk == request.STenTk))
        {
            return new OperationResultDto { Success = false, Message = "Tên đăng nhập đã được sử dụng." };
        }

        if (await _nhanVienRepository.AnyAsync(nv => nv.PkSMaNv == request.PkSMaNv))
        {
            return new OperationResultDto { Success = false, Message = "Mã nhân viên đã tồn tại!" };
        }

        await using var transaction = await _context.Database.BeginTransactionAsync();
        try
        {
            var maTk = await GenerateTaiKhoanCodeAsync();
            await _taiKhoanRepository.AddAsync(new TblTaiKhoan
            {
                PkSMaTk = maTk,
                STenTk = request.STenTk,
                SMk = HashPassword(request.SMk),
                FkSMaQuyen = request.FkSMaQuyen
            });

            await _nhanVienRepository.AddAsync(new TblNhanVien
            {
                PkSMaNv = request.PkSMaNv,
                FkSMaTk = maTk,
                SHoTen = request.SHoTen,
                DNgaySinh = request.DNgaySinh,
                SCccd = request.SCccd,
                SSdt = request.SSdt,
                DNgayVaoLam = request.DNgayVaoLam ?? DateTime.Now,
                FkSMaCv = request.FkSMaCv,
                BTrangThai = true
            });

            await _context.SaveChangesAsync();
            await transaction.CommitAsync();

            return new OperationResultDto { Success = true, Message = "Tạo nhân viên và tài khoản thành công." };
        }
        catch
        {
            await transaction.RollbackAsync();
            return new OperationResultDto { Success = false, Message = "Tạo nhân viên và tài khoản thất bại." };
        }
    }

    public async Task<OperationResultDto> EditEmployeeAsync(TblNhanVien nhanVien)
    {
        var existingNhanVien = await _nhanVienRepository.FirstOrDefaultAsync(nv => nv.PkSMaNv == nhanVien.PkSMaNv);
        if (existingNhanVien == null)
        {
            return new OperationResultDto { Success = false, Message = "Không tìm thấy nhân viên!" };
        }

        existingNhanVien.SHoTen = nhanVien.SHoTen;
        existingNhanVien.DNgaySinh = nhanVien.DNgaySinh;
        existingNhanVien.SCccd = nhanVien.SCccd;
        existingNhanVien.SSdt = nhanVien.SSdt;
        existingNhanVien.DNgayVaoLam = nhanVien.DNgayVaoLam;
        existingNhanVien.FkSMaTk = nhanVien.FkSMaTk;
        existingNhanVien.FkSMaCv = nhanVien.FkSMaCv;
        existingNhanVien.BTrangThai = nhanVien.BTrangThai;

        await _context.SaveChangesAsync();
        return new OperationResultDto { Success = true, Message = "Sửa nhân viên thành công!" };
    }

    public async Task<OperationResultDto> DeleteEmployeeAsync(string maNV)
    {
        var nhanVien = await _nhanVienRepository.FirstOrDefaultAsync(nv => nv.PkSMaNv == maNV);
        if (nhanVien == null)
        {
            return new OperationResultDto { Success = false, Message = "Không tìm thấy nhân viên!" };
        }

        _context.TblNhanViens.Remove(nhanVien);
        await _context.SaveChangesAsync();
        return new OperationResultDto { Success = true, Message = "Xóa nhân viên thành công!" };
    }

    public async Task<EmployeeDropdownDataDto> GetDropdownDataAsync()
    {
        var accounts = await _taiKhoanRepository.Query()
            .AsNoTracking()
            .Select(tk => new AccountDropdownItemDto
            {
                PkSMaTk = tk.PkSMaTk,
                STenTk = tk.STenTk
            })
            .ToListAsync();

        var chucVus = await _chucVuRepository.Query()
            .AsNoTracking()
            .Select(cv => new ChucVuDropdownItemDto
            {
                PkSMaCv = cv.PkSMaCv,
                STenCv = cv.STenCv
            })
            .ToListAsync();

        return new EmployeeDropdownDataDto
        {
            Accounts = accounts,
            ChucVus = chucVus
        };
    }

    private async Task<string> GenerateTaiKhoanCodeAsync()
    {
        var count = 1;
        string maTk;
        do
        {
            maTk = $"TK{count:000}";
            count++;
        } while (await _taiKhoanRepository.AnyAsync(tk => tk.PkSMaTk == maTk));

        return maTk;
    }

    private static string HashPassword(string plainText)
    {
        using var sha = SHA256.Create();
        var bytes = sha.ComputeHash(Encoding.UTF8.GetBytes(plainText));
        var fullHex = Convert.ToHexString(bytes);
        return fullHex[..20];
    }
}
