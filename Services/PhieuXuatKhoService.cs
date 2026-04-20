using CNPM.Infrastructure.Repositories;
using CNPM.Models;
using CNPM.Services.Dtos;
using Microsoft.EntityFrameworkCore;

namespace CNPM.Services;

public class PhieuXuatKhoService : IPhieuXuatKhoService
{
    private readonly IRepository<TblPhieuXuatKho> _phieuXuatKhoRepository;
    private readonly IRepository<TblCtphieuXuatKho> _ctPhieuXuatKhoRepository;

    public PhieuXuatKhoService(IRepository<TblPhieuXuatKho> phieuXuatKhoRepository, IRepository<TblCtphieuXuatKho> ctPhieuXuatKhoRepository)
    {
        _phieuXuatKhoRepository = phieuXuatKhoRepository;
        _ctPhieuXuatKhoRepository = ctPhieuXuatKhoRepository;
    }

    public Task<List<PhieuXuatKhoListItemDto>> GetPhieuXuatKhoListAsync()
    {
        return _phieuXuatKhoRepository.Query()
            .AsNoTracking()
            .Include(px => px.FkSMaNvNavigation)
            .Select(px => new PhieuXuatKhoListItemDto
            {
                PkSMaPx = px.PkSMaPx,
                DTgLap = px.DTgLap,
                NhanVien = px.FkSMaNvNavigation != null ? px.FkSMaNvNavigation.SHoTen : null
            })
            .ToListAsync();
    }

    public async Task<PhieuXuatKhoDetailDto?> GetChiTietPhieuXuatKhoAsync(string maPX)
    {
        var chiTiet = await _ctPhieuXuatKhoRepository.Query()
            .AsNoTracking()
            .Where(ct => ct.PkFkSMaPx == maPX)
            .Include(ct => ct.PkFkSMaSpNavigation)
            .Select(ct => new PhieuXuatKhoChiTietItemDto
            {
                PkFkSMaSp = ct.PkFkSMaSp,
                SanPham = ct.PkFkSMaSpNavigation.STenSp,
                ISlyc = ct.ISlyc,
                ISlx = ct.ISlx,
                SGhiChu = ct.SGhiChu
            })
            .ToListAsync();

        var phieuXuatKho = await _phieuXuatKhoRepository.Query()
            .AsNoTracking()
            .Include(px => px.FkSMaNvNavigation)
            .FirstOrDefaultAsync(px => px.PkSMaPx == maPX);

        if (phieuXuatKho == null)
        {
            return null;
        }

        return new PhieuXuatKhoDetailDto
        {
            MaPX = phieuXuatKho.PkSMaPx,
            NhanVien = phieuXuatKho.FkSMaNvNavigation != null ? phieuXuatKho.FkSMaNvNavigation.SHoTen : null,
            NgayLap = phieuXuatKho.DTgLap,
            ChiTiet = chiTiet
        };
    }
}
