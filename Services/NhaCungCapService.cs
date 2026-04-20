using CNPM.Infrastructure.Repositories;
using CNPM.Models;
using CNPM.Services.Dtos;
using Microsoft.EntityFrameworkCore;

namespace CNPM.Services;

public class NhaCungCapService : INhaCungCapService
{
    private readonly PharmacyDbContext _context;
    private readonly IRepository<TblNhaCungCap> _nhaCungCapRepository;

    public NhaCungCapService(PharmacyDbContext context, IRepository<TblNhaCungCap> nhaCungCapRepository)
    {
        _context = context;
        _nhaCungCapRepository = nhaCungCapRepository;
    }

    public Task<List<NhaCungCapListItemDto>> GetNhaCungCapListAsync()
    {
        return _nhaCungCapRepository.Query()
            .AsNoTracking()
            .Select(ncc => new NhaCungCapListItemDto
            {
                PkSMaNcc = ncc.PkSMaNcc,
                STenNcc = ncc.STenNcc,
                SDiaChi = ncc.SDiaChi,
                SSdt = ncc.SSdt,
                SSoTk = ncc.SSoTk
            })
            .ToListAsync();
    }

    public async Task<OperationResultDto> AddNhaCungCapAsync(TblNhaCungCap nhaCungCap)
    {
        if (await _nhaCungCapRepository.AnyAsync(ncc => ncc.PkSMaNcc == nhaCungCap.PkSMaNcc))
        {
            return new OperationResultDto { Success = false, Message = "Mã nhà cung cấp đã tồn tại!" };
        }

        await _nhaCungCapRepository.AddAsync(nhaCungCap);
        await _context.SaveChangesAsync();

        return new OperationResultDto { Success = true, Message = "Thêm nhà cung cấp thành công!" };
    }

    public async Task<OperationResultDto> EditNhaCungCapAsync(TblNhaCungCap nhaCungCap)
    {
        var existingNhaCungCap = await _nhaCungCapRepository.FirstOrDefaultAsync(ncc => ncc.PkSMaNcc == nhaCungCap.PkSMaNcc);
        if (existingNhaCungCap == null)
        {
            return new OperationResultDto { Success = false, Message = "Không tìm thấy nhà cung cấp!" };
        }

        existingNhaCungCap.STenNcc = nhaCungCap.STenNcc;
        existingNhaCungCap.SDiaChi = nhaCungCap.SDiaChi;
        existingNhaCungCap.SSdt = nhaCungCap.SSdt;
        existingNhaCungCap.SSoTk = nhaCungCap.SSoTk;

        await _context.SaveChangesAsync();

        return new OperationResultDto { Success = true, Message = "Sửa nhà cung cấp thành công!" };
    }

    public async Task<OperationResultDto> DeleteNhaCungCapAsync(string maNCC)
    {
        var nhaCungCap = await _nhaCungCapRepository.FirstOrDefaultAsync(ncc => ncc.PkSMaNcc == maNCC);
        if (nhaCungCap == null)
        {
            return new OperationResultDto { Success = false, Message = "Không tìm thấy nhà cung cấp!" };
        }

        _context.TblNhaCungCaps.Remove(nhaCungCap);
        await _context.SaveChangesAsync();

        return new OperationResultDto { Success = true, Message = "Xóa nhà cung cấp thành công!" };
    }
}
