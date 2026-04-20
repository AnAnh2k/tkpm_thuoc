using CNPM.Infrastructure.Repositories;
using CNPM.Models;
using CNPM.Services.Dtos;
using Microsoft.EntityFrameworkCore;

namespace CNPM.Services;

public class SaleService : ISaleService
{
    private static readonly Dictionary<string, double> KhuyenMaiMacDinhs = new(StringComparer.OrdinalIgnoreCase)
    {
        ["KM5"] = 5,
        ["KM10"] = 10,
        ["KM15"] = 15
    };

    private readonly PharmacyDbContext _context;
    private readonly IRepository<TblKhachHang> _khachHangRepository;
    private readonly IRepository<TblSanPham> _sanPhamRepository;
    private readonly IRepository<TblNhanVien> _nhanVienRepository;
    private readonly IRepository<TblPhieuXuatKho> _phieuXuatKhoRepository;
    private readonly IRepository<TblCtphieuXuatKho> _ctPhieuXuatKhoRepository;
    private readonly IRepository<TblPhieuThu> _phieuThuRepository;
    private readonly IRepository<TblCtphieuThu> _ctPhieuThuRepository;

    public SaleService(
        PharmacyDbContext context,
        IRepository<TblKhachHang> khachHangRepository,
        IRepository<TblSanPham> sanPhamRepository,
        IRepository<TblNhanVien> nhanVienRepository,
        IRepository<TblPhieuXuatKho> phieuXuatKhoRepository,
        IRepository<TblCtphieuXuatKho> ctPhieuXuatKhoRepository,
        IRepository<TblPhieuThu> phieuThuRepository,
        IRepository<TblCtphieuThu> ctPhieuThuRepository)
    {
        _context = context;
        _khachHangRepository = khachHangRepository;
        _sanPhamRepository = sanPhamRepository;
        _nhanVienRepository = nhanVienRepository;
        _phieuXuatKhoRepository = phieuXuatKhoRepository;
        _ctPhieuXuatKhoRepository = ctPhieuXuatKhoRepository;
        _phieuThuRepository = phieuThuRepository;
        _ctPhieuThuRepository = ctPhieuThuRepository;
    }

    public async Task<List<CustomerSearchDto>> SearchCustomers(string query)
    {
        query ??= string.Empty;
        var keyword = query.Trim();

        var customers = await _khachHangRepository.Query()
            .AsNoTracking()
            .OrderBy(kh => kh.PkSMaKh)
            .ToListAsync();

        var result = customers.Select(kh =>
        {
            CustomerDataProtection.DecryptCustomer(kh);
            return new CustomerSearchDto
            {
                PkSMaKh = kh.PkSMaKh,
                STenKh = kh.STenKh,
                SSdt = kh.SSdt
            };
        });

        if (!string.IsNullOrWhiteSpace(keyword))
        {
            result = result.Where(kh => kh.PkSMaKh.Contains(keyword, StringComparison.OrdinalIgnoreCase)
                || (!string.IsNullOrWhiteSpace(kh.STenKh) && kh.STenKh.Contains(keyword, StringComparison.OrdinalIgnoreCase))
                || (!string.IsNullOrWhiteSpace(kh.SSdt) && kh.SSdt.Contains(keyword, StringComparison.OrdinalIgnoreCase)));
        }

        return result.Take(10).ToList();
    }

    public Task<List<ProductSearchDto>> SearchProductAsync(string query)
    {
        query ??= string.Empty;
        var today = DateOnly.FromDateTime(DateTime.Today);

        return _sanPhamRepository.Query()
            .AsNoTracking()
            .Where(sp => (sp.PkSMaSp.Contains(query) || (sp.STenSp != null && sp.STenSp.Contains(query)))
                         && (!sp.SHanDung.HasValue || sp.SHanDung.Value >= today)
                         && (sp.ISl ?? 0) > 0)
            .OrderBy(sp => sp.SHanDung ?? DateOnly.MaxValue)
            .ThenBy(sp => sp.STenSp)
            .Select(sp => new ProductSearchDto
            {
                PkSMaSp = sp.PkSMaSp,
                STenSp = sp.STenSp,
                ISl = sp.ISl,
                FDonGiaBan = sp.FDonGiaBan,
                KeDon = sp.FkSMaLoai == "LSP002"
            })
            .Take(10)
            .ToListAsync();
    }

    public async Task<XuatKhoVaLapHoaDonResultDto> XuatKhoVaLapHoaDonAsync(
        string? maKH,
        Dictionary<string, int> sanPhamsXuat,
        string hinhThucTt,
        string? ghiChu,
        string maNguoiLap,
        string? maKhuyenMai,
        string? loaiGiamGia,
        double giaTriGiamGia,
        string? maQuyenNguoiLap)
    {
        if (sanPhamsXuat == null || sanPhamsXuat.Count == 0)
        {
            return new XuatKhoVaLapHoaDonResultDto { Success = false, Message = "Danh sách sản phẩm xuất không hợp lệ!" };
        }

        if (sanPhamsXuat.Any(x => string.IsNullOrWhiteSpace(x.Key) || x.Value <= 0))
        {
            return new XuatKhoVaLapHoaDonResultDto { Success = false, Message = "Số lượng xuất phải lớn hơn 0." };
        }

        if (giaTriGiamGia < 0)
        {
            return new XuatKhoVaLapHoaDonResultDto { Success = false, Message = "Giá trị giảm giá không hợp lệ." };
        }

        var kmPhanTram = 0d;
        if (!string.IsNullOrWhiteSpace(maKhuyenMai))
        {
            if (!KhuyenMaiMacDinhs.TryGetValue(maKhuyenMai.Trim(), out kmPhanTram))
            {
                return new XuatKhoVaLapHoaDonResultDto { Success = false, Message = "Mã khuyến mãi không hợp lệ." };
            }
        }

        await using var transaction = await _context.Database.BeginTransactionAsync();

        try
        {
            var now = DateTime.Now;
            var maPX = $"PX{now:yyyyMMddHHmmss}";
            var maPT = $"PT{now:yyyyMMddHHmmss}";

            if (!await _nhanVienRepository.AnyAsync(nv => nv.PkSMaNv == maNguoiLap))
            {
                return new XuatKhoVaLapHoaDonResultDto { Success = false, Message = "Không tìm thấy nhân viên lập hóa đơn!" };
            }

            if (!string.IsNullOrWhiteSpace(maKH) && !await _khachHangRepository.AnyAsync(kh => kh.PkSMaKh == maKH))
            {
                return new XuatKhoVaLapHoaDonResultDto { Success = false, Message = "Khách hàng không tồn tại!" };
            }

            if (hinhThucTt != "Tiền mặt" && hinhThucTt != "Chuyển khoản")
            {
                return new XuatKhoVaLapHoaDonResultDto { Success = false, Message = "Hình thức thanh toán không hợp lệ!" };
            }

            await _phieuXuatKhoRepository.AddAsync(new TblPhieuXuatKho
            {
                PkSMaPx = maPX,
                DTgLap = now,
                FkSMaNv = maNguoiLap
            });

            var today = DateOnly.FromDateTime(DateTime.Today);
            var tongTamTinh = 0d;
            var banHangTheoSanPham = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);

            foreach (var item in sanPhamsXuat)
            {
                var maSPYeuCau = item.Key;
                var soLuongConLai = item.Value;

                var sanPhamYeuCau = await _sanPhamRepository.FirstOrDefaultAsync(sp => sp.PkSMaSp == maSPYeuCau);
                if (sanPhamYeuCau == null)
                {
                    return new XuatKhoVaLapHoaDonResultDto { Success = false, Message = $"Không tìm thấy sản phẩm {maSPYeuCau}." };
                }

                var nhomTenSanPham = sanPhamYeuCau.STenSp;
                var ungVienFefo = await _sanPhamRepository.Query()
                    .Where(sp => sp.STenSp == nhomTenSanPham
                                 && (sp.ISl ?? 0) > 0
                                 && (!sp.SHanDung.HasValue || sp.SHanDung.Value >= today))
                    .OrderBy(sp => sp.SHanDung ?? DateOnly.MaxValue)
                    .ThenBy(sp => sp.PkSMaSp)
                    .ToListAsync();

                var tongTonKhaDung = ungVienFefo.Sum(sp => sp.ISl ?? 0);
                if (tongTonKhaDung < soLuongConLai)
                {
                    return new XuatKhoVaLapHoaDonResultDto
                    {
                        Success = false,
                        Message = $"Sản phẩm {sanPhamYeuCau.STenSp ?? maSPYeuCau} không đủ tồn kho!"
                    };
                }

                foreach (var sanPhamFefo in ungVienFefo)
                {
                    if (soLuongConLai <= 0)
                    {
                        break;
                    }

                    var tonHienTai = sanPhamFefo.ISl ?? 0;
                    if (tonHienTai <= 0)
                    {
                        continue;
                    }

                    var slXuat = Math.Min(tonHienTai, soLuongConLai);

                    await _ctPhieuXuatKhoRepository.AddAsync(new TblCtphieuXuatKho
                    {
                        PkFkSMaPx = maPX,
                        PkFkSMaSp = sanPhamFefo.PkSMaSp,
                        ISlyc = slXuat,
                        ISlx = slXuat,
                        SGhiChu = string.IsNullOrWhiteSpace(ghiChu) ? null : ghiChu
                    });

                    tongTamTinh += slXuat * (sanPhamFefo.FDonGiaBan ?? 0d);
                    sanPhamFefo.ISl = tonHienTai - slXuat;
                    _sanPhamRepository.Update(sanPhamFefo);

                    if (banHangTheoSanPham.TryGetValue(sanPhamFefo.PkSMaSp, out var daCo))
                    {
                        banHangTheoSanPham[sanPhamFefo.PkSMaSp] = daCo + slXuat;
                    }
                    else
                    {
                        banHangTheoSanPham[sanPhamFefo.PkSMaSp] = slXuat;
                    }

                    soLuongConLai -= slXuat;
                }
            }

            await _phieuThuRepository.AddAsync(new TblPhieuThu
            {
                PkSMaPt = maPT,
                DTgLap = now,
                FkSMaNv = maNguoiLap,
                FkSMaKh = string.IsNullOrWhiteSpace(maKH) ? null : maKH,
                SHinhThucTt = hinhThucTt
            });

            _ctPhieuThuRepository.AddRange(banHangTheoSanPham.Select(item => new TblCtphieuThu
            {
                PkFkSMaPt = maPT,
                PkFkSMaSp = item.Key,
                ISl = item.Value
            }));

            var loaiGiamGiaNormalized = (loaiGiamGia ?? string.Empty).Trim().ToUpperInvariant();
            var laNhanVienThuong = string.Equals(maQuyenNguoiLap, "Q001", StringComparison.OrdinalIgnoreCase);

            var tienGiamTuKm = tongTamTinh * kmPhanTram / 100d;
            var tienGiamThuCong = 0d;

            if (loaiGiamGiaNormalized is "PHANTRAM" or "%")
            {
                if (giaTriGiamGia > 100)
                {
                    return new XuatKhoVaLapHoaDonResultDto { Success = false, Message = "Giảm giá theo phần trăm phải nằm trong khoảng 0 - 100%." };
                }

                if (laNhanVienThuong && giaTriGiamGia > 50)
                {
                    return new XuatKhoVaLapHoaDonResultDto { Success = false, Message = "Nhân viên thường chỉ được giảm tối đa 50%." };
                }

                tienGiamThuCong = tongTamTinh * giaTriGiamGia / 100d;
            }
            else if (loaiGiamGiaNormalized is "VND" or "TIEN" or "TIENMAT")
            {
                if (giaTriGiamGia > tongTamTinh)
                {
                    return new XuatKhoVaLapHoaDonResultDto { Success = false, Message = "Giá trị giảm không được lớn hơn tổng tiền." };
                }

                if (laNhanVienThuong && tongTamTinh > 0 && (giaTriGiamGia / tongTamTinh * 100d) > 50)
                {
                    return new XuatKhoVaLapHoaDonResultDto { Success = false, Message = "Nhân viên thường chỉ được giảm tối đa 50%." };
                }

                tienGiamThuCong = giaTriGiamGia;
            }
            else
            {
                loaiGiamGiaNormalized = string.Empty;
                giaTriGiamGia = 0;
            }

            var tongTienGiam = Math.Min(tongTamTinh, tienGiamTuKm + tienGiamThuCong);
            var tongThanhToan = tongTamTinh - tongTienGiam;

            await _context.SaveChangesAsync();
            await transaction.CommitAsync();

            return new XuatKhoVaLapHoaDonResultDto
            {
                Success = true,
                MaPT = maPT,
                Message = "Thanh toán thành công",
                MaKhuyenMai = string.IsNullOrWhiteSpace(maKhuyenMai) ? null : maKhuyenMai.Trim().ToUpperInvariant(),
                KhuyenMaiPhanTram = kmPhanTram,
                LoaiGiamGia = string.IsNullOrWhiteSpace(loaiGiamGiaNormalized) ? null : loaiGiamGiaNormalized,
                GiaTriGiamGia = giaTriGiamGia,
                TienGiamThuCong = tienGiamThuCong,
                TongTienTamTinh = tongTamTinh,
                TongTienGiam = tongTienGiam,
                TongThanhToan = tongThanhToan
            };
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync();
            return new XuatKhoVaLapHoaDonResultDto
            {
                Success = false,
                Message = ex.Message + " | Inner Exception: " + (ex.InnerException?.Message ?? "No inner exception")
            };
        }
    }

    public async Task<TblPhieuThu?> GetHoaDonAsync(string maPT, bool includeNhanVien = false)
    {
        IQueryable<TblPhieuThu> query = _phieuThuRepository.Query()
            .AsNoTracking()
            .Include(pt => pt.TblCtphieuThus)
            .ThenInclude(ct => ct.PkFkSMaSpNavigation)
            .Include(pt => pt.FkSMaKhNavigation);

        if (includeNhanVien)
        {
            query = query.Include(pt => pt.FkSMaNvNavigation);
        }

        var phieuThu = await query.FirstOrDefaultAsync(pt => pt.PkSMaPt == maPT);
        if (phieuThu?.FkSMaKhNavigation != null)
        {
            CustomerDataProtection.DecryptCustomer(phieuThu.FkSMaKhNavigation);
        }

        return phieuThu;
    }
}
