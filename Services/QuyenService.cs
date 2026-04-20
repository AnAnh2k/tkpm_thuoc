using CNPM.Infrastructure.Repositories;
using CNPM.Models;
using CNPM.Services.Dtos;
using Microsoft.EntityFrameworkCore;

namespace CNPM.Services;

public class QuyenService : IQuyenService
{
    private readonly PharmacyDbContext _context;
    private readonly IRepository<TblQuyen> _quyenRepository;

    public QuyenService(PharmacyDbContext context, IRepository<TblQuyen> quyenRepository)
    {
        _context = context;
        _quyenRepository = quyenRepository;
    }

    public Task<List<DropdownItemDto>> GetQuyenListAsync()
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

    public async Task<OperationResultDto> AddQuyenAsync(TblQuyen quyen)
    {
        if (await _quyenRepository.AnyAsync(q => q.PkSMaQuyen == quyen.PkSMaQuyen))
        {
            return new OperationResultDto { Success = false, Message = "Mã quyền đã tồn tại!" };
        }

        await _quyenRepository.AddAsync(quyen);
        await _context.SaveChangesAsync();
        return new OperationResultDto { Success = true, Message = "Thêm quyền thành công!" };
    }

    public async Task<OperationResultDto> EditQuyenAsync(TblQuyen quyen)
    {
        var existingQuyen = await _quyenRepository.FirstOrDefaultAsync(q => q.PkSMaQuyen == quyen.PkSMaQuyen);
        if (existingQuyen == null)
        {
            return new OperationResultDto { Success = false, Message = "Không tìm thấy quyền!" };
        }

        existingQuyen.STenQuyen = quyen.STenQuyen;
        await _context.SaveChangesAsync();
        return new OperationResultDto { Success = true, Message = "Sửa quyền thành công!" };
    }

    public async Task<OperationResultDto> DeleteQuyenAsync(string maQuyen)
    {
        var quyen = await _quyenRepository.FirstOrDefaultAsync(q => q.PkSMaQuyen == maQuyen);
        if (quyen == null)
        {
            return new OperationResultDto { Success = false, Message = "Không tìm thấy quyền!" };
        }

        _context.TblQuyens.Remove(quyen);
        await _context.SaveChangesAsync();
        return new OperationResultDto { Success = true, Message = "Xóa quyền thành công!" };
    }
}
