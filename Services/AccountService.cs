using CNPM.Infrastructure.Repositories;
using CNPM.Models;
using CNPM.Services.Dtos;
using Microsoft.EntityFrameworkCore;

namespace CNPM.Services;

public class AccountService : IAccountService
{
    private readonly PharmacyDbContext _context;
    private readonly IRepository<TblTaiKhoan> _taiKhoanRepository;
    private readonly IRepository<TblQuyen> _quyenRepository;

    public AccountService(
        PharmacyDbContext context,
        IRepository<TblTaiKhoan> taiKhoanRepository,
        IRepository<TblQuyen> quyenRepository)
    {
        _context = context;
        _taiKhoanRepository = taiKhoanRepository;
        _quyenRepository = quyenRepository;
    }

    public Task<List<AccountListItemDto>> GetAccountListAsync()
    {
        return _taiKhoanRepository.Query()
            .AsNoTracking()
            .Include(tk => tk.FkSMaQuyenNavigation)
            .Select(tk => new AccountListItemDto
            {
                PkSMaTk = tk.PkSMaTk,
                STenTk = tk.STenTk,
                SMk = tk.SMk,
                FkSMaQuyen = tk.FkSMaQuyen,
                Quyen = tk.FkSMaQuyenNavigation != null ? tk.FkSMaQuyenNavigation.STenQuyen : null
            })
            .ToListAsync();
    }

    public async Task<OperationResultDto> AddAccountAsync(TblTaiKhoan taiKhoan)
    {
        if (await _taiKhoanRepository.AnyAsync(tk => tk.PkSMaTk == taiKhoan.PkSMaTk))
        {
            return new OperationResultDto { Success = false, Message = "Mã tài khoản đã tồn tại!" };
        }

        await _taiKhoanRepository.AddAsync(taiKhoan);
        await _context.SaveChangesAsync();

        return new OperationResultDto { Success = true, Message = "Thêm tài khoản thành công!" };
    }

    public async Task<OperationResultDto> EditAccountAsync(TblTaiKhoan taiKhoan)
    {
        var existingTaiKhoan = await _taiKhoanRepository.FirstOrDefaultAsync(tk => tk.PkSMaTk == taiKhoan.PkSMaTk);
        if (existingTaiKhoan == null)
        {
            return new OperationResultDto { Success = false, Message = "Không tìm thấy tài khoản!" };
        }

        existingTaiKhoan.STenTk = taiKhoan.STenTk;
        existingTaiKhoan.SMk = taiKhoan.SMk;
        existingTaiKhoan.FkSMaQuyen = taiKhoan.FkSMaQuyen;

        await _context.SaveChangesAsync();

        return new OperationResultDto { Success = true, Message = "Sửa tài khoản thành công!" };
    }

    public async Task<OperationResultDto> DeleteAccountAsync(string maTK)
    {
        var taiKhoan = await _taiKhoanRepository.FirstOrDefaultAsync(tk => tk.PkSMaTk == maTK);
        if (taiKhoan == null)
        {
            return new OperationResultDto { Success = false, Message = "Không tìm thấy tài khoản!" };
        }

        _context.TblTaiKhoans.Remove(taiKhoan);
        await _context.SaveChangesAsync();

        return new OperationResultDto { Success = true, Message = "Xóa tài khoản thành công!" };
    }

    public Task<List<DropdownItemDto>> GetQuyenDropdownAsync()
    {
        return _quyenRepository.Query()
            .AsNoTracking()
            .Select(q => new DropdownItemDto
            {
                Id = q.PkSMaQuyen,
                Name = q.STenQuyen
            })
            .ToListAsync();
    }

    public async Task<OperationResultDto> AssignRoleAsync(string maTK, string maQuyen)
    {
        var taiKhoan = await _taiKhoanRepository.FirstOrDefaultAsync(tk => tk.PkSMaTk == maTK);
        if (taiKhoan == null)
        {
            return new OperationResultDto { Success = false, Message = "Không tìm thấy tài khoản!" };
        }

        if (!await _quyenRepository.AnyAsync(q => q.PkSMaQuyen == maQuyen))
        {
            return new OperationResultDto { Success = false, Message = "Không tìm thấy quyền cần gán!" };
        }

        taiKhoan.FkSMaQuyen = maQuyen;
        await _context.SaveChangesAsync();

        return new OperationResultDto { Success = true, Message = "Phân quyền thành công." };
    }
}
