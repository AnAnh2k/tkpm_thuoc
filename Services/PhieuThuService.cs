using CNPM.Infrastructure.Repositories;
using CNPM.Models;
using CNPM.Services.Dtos;
using Microsoft.EntityFrameworkCore;

namespace CNPM.Services;

public class PhieuThuService : IPhieuThuService
{
    private readonly IRepository<TblPhieuThu> _phieuThuRepository;
    private readonly IRepository<TblCtphieuThu> _ctPhieuThuRepository;

    public PhieuThuService(IRepository<TblPhieuThu> phieuThuRepository, IRepository<TblCtphieuThu> ctPhieuThuRepository)
    {
        _phieuThuRepository = phieuThuRepository;
        _ctPhieuThuRepository = ctPhieuThuRepository;
    }

    public async Task<PagedResultDto<PhieuThuListItemDto>> GetPhieuThuListAsync(DateTime? dTuNgay, DateTime? dDenNgay, string? maPT, string? maKH, string? maNguoiLap, int iPage, int pageSize = 20)
    {
        var query = BuildFilteredQuery(dTuNgay, dDenNgay, maPT, maKH, maNguoiLap);

        var totalItems = await query.CountAsync();
        var safePage = iPage <= 0 ? 1 : iPage;
        var totalPages = totalItems == 0 ? 1 : (int)Math.Ceiling(totalItems / (double)pageSize);
        if (safePage > totalPages)
        {
            safePage = totalPages;
        }

        var phieuThuRows = await query
            .OrderByDescending(pt => pt.DTgLap)
            .Skip((safePage - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        var items = MapToListItems(phieuThuRows);

        return new PagedResultDto<PhieuThuListItemDto>
        {
            Items = items,
            CurrentPage = safePage,
            PageSize = pageSize,
            TotalItems = totalItems,
            TotalPages = totalPages,
            Message = totalItems == 0 ? "Không có hóa đơn nào trong khoảng thời gian này" : null
        };
    }

    public async Task<List<PhieuThuListItemDto>> GetPhieuThuExportListAsync(DateTime? dTuNgay, DateTime? dDenNgay, string? maPT, string? maKH, string? maNguoiLap)
    {
        var rows = await BuildFilteredQuery(dTuNgay, dDenNgay, maPT, maKH, maNguoiLap)
            .OrderByDescending(pt => pt.DTgLap)
            .ToListAsync();

        return MapToListItems(rows);
    }

    public async Task<PhieuThuDetailDto?> GetChiTietPhieuThuAsync(string maPT)
    {
        var chiTiet = await _ctPhieuThuRepository.Query()
            .AsNoTracking()
            .Where(ct => ct.PkFkSMaPt == maPT)
            .Include(ct => ct.PkFkSMaSpNavigation)
            .Select(ct => new PhieuThuChiTietItemDto
            {
                PkFkSMaSp = ct.PkFkSMaSp,
                SanPham = ct.PkFkSMaSpNavigation.STenSp,
                ISl = ct.ISl,
                DonGiaBan = ct.PkFkSMaSpNavigation.FDonGiaBan,
                ThanhTien = (ct.ISl ?? 0) * (ct.PkFkSMaSpNavigation.FDonGiaBan ?? 0)
            })
            .ToListAsync();

        var phieuThu = await _phieuThuRepository.Query()
            .AsNoTracking()
            .Include(pt => pt.FkSMaKhNavigation)
            .FirstOrDefaultAsync(pt => pt.PkSMaPt == maPT);

        if (phieuThu == null)
        {
            return null;
        }

        if (phieuThu.FkSMaKhNavigation != null)
        {
            CustomerDataProtection.DecryptCustomer(phieuThu.FkSMaKhNavigation);
        }

        return new PhieuThuDetailDto
        {
            MaPT = phieuThu.PkSMaPt,
            KhachHang = phieuThu.FkSMaKhNavigation != null ? phieuThu.FkSMaKhNavigation.STenKh : null,
            NgayLap = phieuThu.DTgLap,
            HinhThucTT = phieuThu.SHinhThucTt,
            ChiTiet = chiTiet
        };
    }

    private IQueryable<TblPhieuThu> BuildFilteredQuery(DateTime? dTuNgay, DateTime? dDenNgay, string? maPT, string? maKH, string? maNguoiLap)
    {
        var query = _phieuThuRepository.Query()
            .AsNoTracking()
            .Include(pt => pt.FkSMaNvNavigation)
            .Include(pt => pt.FkSMaKhNavigation)
            .AsQueryable();

        var hasAnyFilter = dTuNgay.HasValue
            || dDenNgay.HasValue
            || !string.IsNullOrWhiteSpace(maPT)
            || !string.IsNullOrWhiteSpace(maKH)
            || !string.IsNullOrWhiteSpace(maNguoiLap);

        if (!hasAnyFilter)
        {
            var now = DateTime.Now;
            var startOfMonth = new DateTime(now.Year, now.Month, 1);
            var startOfNextMonth = startOfMonth.AddMonths(1);
            query = query.Where(pt => pt.DTgLap >= startOfMonth && pt.DTgLap < startOfNextMonth);
        }

        if (dTuNgay.HasValue)
        {
            query = query.Where(pt => pt.DTgLap >= dTuNgay.Value);
        }

        if (dDenNgay.HasValue)
        {
            var denNgayCuoi = dDenNgay.Value.Date.AddDays(1).AddTicks(-1);
            query = query.Where(pt => pt.DTgLap <= denNgayCuoi);
        }

        if (!string.IsNullOrWhiteSpace(maPT))
        {
            var keyword = maPT.Trim();
            query = query.Where(pt => pt.PkSMaPt.Contains(keyword));
        }

        if (!string.IsNullOrWhiteSpace(maKH))
        {
            query = query.Where(pt => pt.FkSMaKh == maKH);
        }

        if (!string.IsNullOrWhiteSpace(maNguoiLap))
        {
            query = query.Where(pt => pt.FkSMaNv == maNguoiLap);
        }

        return query;
    }

    private static List<PhieuThuListItemDto> MapToListItems(List<TblPhieuThu> rows)
    {
        return rows.Select(pt =>
        {
            if (pt.FkSMaKhNavigation != null)
            {
                CustomerDataProtection.DecryptCustomer(pt.FkSMaKhNavigation);
            }

            return new PhieuThuListItemDto
            {
                PkSMaPt = pt.PkSMaPt,
                DTgLap = pt.DTgLap,
                FkSMaNv = pt.FkSMaNv,
                NhanVien = pt.FkSMaNvNavigation != null ? pt.FkSMaNvNavigation.SHoTen : null,
                FkSMaKh = pt.FkSMaKh,
                KhachHang = pt.FkSMaKhNavigation != null ? pt.FkSMaKhNavigation.STenKh : null,
                SHinhThucTt = pt.SHinhThucTt
            };
        }).ToList();
    }
}
