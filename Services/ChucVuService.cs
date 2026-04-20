using CNPM.Infrastructure.Repositories;
using CNPM.Models;
using CNPM.Services.Dtos;
using Microsoft.EntityFrameworkCore;

namespace CNPM.Services;

public class ChucVuService : IChucVuService
{
    private readonly PharmacyDbContext _context;
    private readonly IRepository<TblChucVu> _chucVuRepository;

    public ChucVuService(PharmacyDbContext context, IRepository<TblChucVu> chucVuRepository)
    {
        _context = context;
        _chucVuRepository = chucVuRepository;
    }

    public Task<List<DropdownItemDto>> GetChucVuListAsync()
    {
        return _chucVuRepository.Query()
            .AsNoTracking()
            .Select(cv => new DropdownItemDto
            {
                Id = cv.PkSMaCv,
                Name = cv.STenCv
            })
            .ToListAsync();
    }

    public async Task<OperationResultDto> AddChucVuAsync(TblChucVu chucVu)
    {
        if (await _chucVuRepository.AnyAsync(cv => cv.PkSMaCv == chucVu.PkSMaCv))
        {
            return new OperationResultDto { Success = false, Message = "Mã chức vụ đã tồn tại!" };
        }

        await _chucVuRepository.AddAsync(chucVu);
        await _context.SaveChangesAsync();
        return new OperationResultDto { Success = true, Message = "Thêm chức vụ thành công!" };
    }

    public async Task<OperationResultDto> EditChucVuAsync(TblChucVu chucVu)
    {
        var existingChucVu = await _chucVuRepository.FirstOrDefaultAsync(cv => cv.PkSMaCv == chucVu.PkSMaCv);
        if (existingChucVu == null)
        {
            return new OperationResultDto { Success = false, Message = "Không tìm thấy chức vụ!" };
        }

        existingChucVu.STenCv = chucVu.STenCv;
        await _context.SaveChangesAsync();
        return new OperationResultDto { Success = true, Message = "Sửa chức vụ thành công!" };
    }

    public async Task<OperationResultDto> DeleteChucVuAsync(string maCV)
    {
        var chucVu = await _chucVuRepository.FirstOrDefaultAsync(cv => cv.PkSMaCv == maCV);
        if (chucVu == null)
        {
            return new OperationResultDto { Success = false, Message = "Không tìm thấy chức vụ!" };
        }

        _context.TblChucVus.Remove(chucVu);
        await _context.SaveChangesAsync();
        return new OperationResultDto { Success = true, Message = "Xóa chức vụ thành công!" };
    }
}
