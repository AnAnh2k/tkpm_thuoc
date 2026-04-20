using CNPM.Infrastructure.Repositories;
using CNPM.Models;
using CNPM.Services.Dtos;
using ClosedXML.Excel;
using Microsoft.EntityFrameworkCore;

namespace CNPM.Services;

public class ReportService : IReportService
{
    private readonly PharmacyDbContext _context;
    private readonly IRepository<TblPhieuThu> _phieuThuRepository;
    private readonly IRepository<TblCtphieuThu> _ctPhieuThuRepository;
    private readonly IRepository<TblSanPham> _sanPhamRepository;
    private readonly IRepository<TblPhieuXuatKho> _phieuXuatKhoRepository;
    private readonly IRepository<TblCtphieuXuatKho> _ctPhieuXuatKhoRepository;
    private readonly IRepository<TblKhachHang> _khachHangRepository;
    private readonly IRepository<TblNhanVien> _nhanVienRepository;
    private readonly IRepository<TblPhieuChi> _phieuChiRepository;
    private readonly IRepository<TblPhieuNhapKho> _phieuNhapRepository;
    private readonly IRepository<TblCtphieuNhapKho> _ctPhieuNhapRepository;
    private readonly IRepository<TblBaoCaoThuChi> _baoCaoThuChiRepository;

    public ReportService(
        PharmacyDbContext context,
        IRepository<TblPhieuThu> phieuThuRepository,
        IRepository<TblCtphieuThu> ctPhieuThuRepository,
        IRepository<TblSanPham> sanPhamRepository,
        IRepository<TblPhieuXuatKho> phieuXuatKhoRepository,
        IRepository<TblCtphieuXuatKho> ctPhieuXuatKhoRepository,
        IRepository<TblKhachHang> khachHangRepository,
        IRepository<TblNhanVien> nhanVienRepository,
        IRepository<TblPhieuChi> phieuChiRepository,
        IRepository<TblPhieuNhapKho> phieuNhapRepository,
        IRepository<TblCtphieuNhapKho> ctPhieuNhapRepository,
        IRepository<TblBaoCaoThuChi> baoCaoThuChiRepository)
    {
        _context = context;
        _phieuThuRepository = phieuThuRepository;
        _ctPhieuThuRepository = ctPhieuThuRepository;
        _sanPhamRepository = sanPhamRepository;
        _phieuXuatKhoRepository = phieuXuatKhoRepository;
        _ctPhieuXuatKhoRepository = ctPhieuXuatKhoRepository;
        _khachHangRepository = khachHangRepository;
        _nhanVienRepository = nhanVienRepository;
        _phieuChiRepository = phieuChiRepository;
        _phieuNhapRepository = phieuNhapRepository;
        _ctPhieuNhapRepository = ctPhieuNhapRepository;
        _baoCaoThuChiRepository = baoCaoThuChiRepository;
    }

    public async Task<List<DoanhThuDto>> GetDoanhThuAsync(string? loaiThongKe, DateTime? tuNgay, DateTime? denNgay)
    {
        var from = tuNgay ?? DateTime.Now.AddDays(-30);
        var to = denNgay ?? DateTime.Now;

        var source = await _phieuThuRepository.Query()
            .AsNoTracking()
            .Where(pt => pt.DTgLap >= from && pt.DTgLap <= to)
            .Join(_ctPhieuThuRepository.Query(),
                pt => pt.PkSMaPt,
                ct => ct.PkFkSMaPt,
                (pt, ct) => new { pt.DTgLap, ct.ISl, ct.PkFkSMaSp })
            .Join(_sanPhamRepository.Query(),
                ct => ct.PkFkSMaSp,
                sp => sp.PkSMaSp,
                (ct, sp) => new
                {
                    DTgLap = ct.DTgLap ?? from,
                    ThanhTien = (ct.ISl ?? 0) * (sp.FDonGiaBan ?? 0)
                })
            .ToListAsync();

        loaiThongKe ??= "ngay";

        return loaiThongKe.ToLowerInvariant() switch
        {
            "thang" => source
                .GroupBy(x => new { x.DTgLap.Year, x.DTgLap.Month })
                .OrderBy(g => g.Key.Year)
                .ThenBy(g => g.Key.Month)
                .Select(g => new DoanhThuDto
                {
                    ThoiGian = $"{g.Key.Month}/{g.Key.Year}",
                    TongDoanhThu = g.Sum(x => x.ThanhTien)
                })
                .ToList(),
            "nam" => source
                .GroupBy(x => x.DTgLap.Year)
                .OrderBy(g => g.Key)
                .Select(g => new DoanhThuDto
                {
                    ThoiGian = g.Key.ToString(),
                    TongDoanhThu = g.Sum(x => x.ThanhTien)
                })
                .ToList(),
            "tuan" => source
                .GroupBy(x => new { x.DTgLap.Year, Week = ((x.DTgLap.DayOfYear - 1) / 7) + 1 })
                .OrderBy(g => g.Key.Year)
                .ThenBy(g => g.Key.Week)
                .Select(g => new DoanhThuDto
                {
                    ThoiGian = $"Tuần {g.Key.Week}/{g.Key.Year}",
                    TongDoanhThu = g.Sum(x => x.ThanhTien)
                })
                .ToList(),
            _ => source
                .GroupBy(x => x.DTgLap.Date)
                .OrderBy(g => g.Key)
                .Select(g => new DoanhThuDto
                {
                    ThoiGian = g.Key.ToString("yyyy-MM-dd"),
                    TongDoanhThu = g.Sum(x => x.ThanhTien)
                })
                .ToList()
        };
    }

    public async Task<List<XuatKhoDto>> GetXuatKhoAsync(DateTime? tuNgay, DateTime? denNgay)
    {
        var from = tuNgay ?? DateTime.Now.AddDays(-30);
        var to = denNgay ?? DateTime.Now;

        var source = await _phieuXuatKhoRepository.Query()
            .AsNoTracking()
            .Where(px => px.DTgLap >= from && px.DTgLap <= to)
            .Join(_ctPhieuXuatKhoRepository.Query(),
                px => px.PkSMaPx,
                ct => ct.PkFkSMaPx,
                (px, ct) => new { px.DTgLap, ct.ISlx, ct.PkFkSMaSp })
            .Join(_sanPhamRepository.Query(),
                ct => ct.PkFkSMaSp,
                sp => sp.PkSMaSp,
                (ct, sp) => new
                {
                    Ngay = (ct.DTgLap ?? from).Date,
                    ISlx = ct.ISlx ?? 0,
                    STenSp = sp.STenSp ?? string.Empty
                })
            .ToListAsync();

        return source
            .GroupBy(x => x.Ngay)
            .OrderBy(g => g.Key)
            .Select(g => new XuatKhoDto
            {
                Ngay = g.Key,
                TongSoLuongXuat = g.Sum(x => x.ISlx),
                ChiTiet = g.Select(x => new XuatKhoChiTietDto { STenSp = x.STenSp, ISlx = x.ISlx }).ToList()
            })
            .ToList();
    }

    public Task<List<TonKhoDto>> GetTonKhoAsync()
    {
        var today = DateOnly.FromDateTime(DateTime.Now);

        return _sanPhamRepository.Query()
            .AsNoTracking()
            .OrderBy(sp => sp.STenSp)
            .Select(sp => new TonKhoDto
            {
                PkSMaSp = sp.PkSMaSp,
                STenSp = sp.STenSp,
                ISl = sp.ISl,
                SHanDung = sp.SHanDung,
                TrangThai = sp.SHanDung.HasValue && sp.SHanDung.Value < today
                    ? "Hết hạn"
                    : (sp.ISl ?? 0) <= 10 ? "Sắp hết" : "Bình thường"
            })
            .ToListAsync();
    }

    public async Task<List<KhachHangMuaNhieuDto>> GetKhachHangMuaNhieuAsync()
    {
        var khachHangs = await _khachHangRepository.Query()
            .AsNoTracking()
            .Include(kh => kh.TblPhieuThus)
                .ThenInclude(pt => pt.TblCtphieuThus)
                    .ThenInclude(ct => ct.PkFkSMaSpNavigation)
            .OrderBy(kh => kh.STenKh)
            .ToListAsync();

        return khachHangs.Select(kh =>
        {
            CustomerDataProtection.DecryptCustomer(kh);

            return new KhachHangMuaNhieuDto
            {
                MaKH = kh.PkSMaKh,
                TenKH = kh.STenKh,
                SoDienThoai = kh.SSdt,
                HoaDons = kh.TblPhieuThus.Select(pt => new HoaDonKhachHangDto
                {
                    MaPT = pt.PkSMaPt,
                    NgayLap = pt.DTgLap,
                    HinhThucTT = pt.SHinhThucTt,
                    TongTien = pt.TblCtphieuThus.Sum(ct => (ct.ISl ?? 0) * (ct.PkFkSMaSpNavigation.FDonGiaBan ?? 0)),
                    ChiTietHoaDon = pt.TblCtphieuThus.Select(ct => new ChiTietHoaDonDto
                    {
                        MaSP = ct.PkFkSMaSp,
                        TenSP = ct.PkFkSMaSpNavigation.STenSp,
                        SoLuong = ct.ISl ?? 0,
                        DonGia = ct.PkFkSMaSpNavigation.FDonGiaBan ?? 0,
                        ThanhTien = (ct.ISl ?? 0) * (ct.PkFkSMaSpNavigation.FDonGiaBan ?? 0),
                        KeDon = ct.PkFkSMaSpNavigation.FkSMaLoai == "LSP002"
                    }).ToList()
                }).ToList()
            };
        }).ToList();
    }

    public async Task<List<HieuSuatNhanVienDto>> GetHieuSuatNhanVienAsync(DateTime? tuNgay, DateTime? denNgay)
    {
        var from = tuNgay ?? DateTime.Now.AddDays(-30);
        var to = denNgay ?? DateTime.Now;

        var nhanViens = await _nhanVienRepository.Query().AsNoTracking().ToListAsync();

        var phieuThus = await _phieuThuRepository.Query()
            .AsNoTracking()
            .Where(pt => pt.DTgLap >= from && pt.DTgLap <= to)
            .ToListAsync();

        var phieuXuatKhos = await _phieuXuatKhoRepository.Query()
            .AsNoTracking()
            .Where(px => px.DTgLap >= from && px.DTgLap <= to)
            .ToListAsync();

        var ctPhieuThus = await _ctPhieuThuRepository.Query().AsNoTracking().ToListAsync();
        var sanPhams = await _sanPhamRepository.Query().AsNoTracking().ToListAsync();

        return nhanViens
            .Select(nv =>
            {
                var phieuThuTheoNv = phieuThus.Where(pt => pt.FkSMaNv == nv.PkSMaNv).ToList();
                var tongDoanhThu = phieuThuTheoNv
                    .Join(ctPhieuThus,
                        pt => pt.PkSMaPt,
                        ct => ct.PkFkSMaPt,
                        (pt, ct) => ct)
                    .Join(sanPhams,
                        ct => ct.PkFkSMaSp,
                        sp => sp.PkSMaSp,
                        (ct, sp) => (ct.ISl ?? 0) * (sp.FDonGiaBan ?? 0))
                    .Sum();

                return new HieuSuatNhanVienDto
                {
                    MaNV = nv.PkSMaNv,
                    TenNV = nv.SHoTen,
                    SoPhieuThu = phieuThuTheoNv.Count,
                    SoPhieuXuat = phieuXuatKhos.Count(px => px.FkSMaNv == nv.PkSMaNv),
                    TongDoanhThu = tongDoanhThu
                };
            })
            .OrderByDescending(x => x.TongDoanhThu)
            .ToList();
    }

    public async Task<ThuChiTongHopDto> GenerateThuChiTongHopAsync(DateTime dTgBatDau, DateTime dTgKetThuc, string? maNv)
    {
        var revenueByProduct = await _phieuThuRepository.Query()
            .AsNoTracking()
            .Where(pt => pt.DTgLap >= dTgBatDau && pt.DTgLap <= dTgKetThuc)
            .Join(_ctPhieuThuRepository.Query(),
                pt => pt.PkSMaPt,
                ct => ct.PkFkSMaPt,
                (pt, ct) => new { ct.PkFkSMaSp, SoLuong = ct.ISl ?? 0 })
            .Join(_sanPhamRepository.Query(),
                ct => ct.PkFkSMaSp,
                sp => sp.PkSMaSp,
                (ct, sp) => new
                {
                    sp.PkSMaSp,
                    sp.STenSp,
                    SoLuong = ct.SoLuong,
                    ThanhTien = ct.SoLuong * (sp.FDonGiaBan ?? 0)
                })
            .ToListAsync();

        var tongThu = revenueByProduct.Sum(x => x.ThanhTien);

        var tongChi = await _phieuChiRepository.Query()
            .AsNoTracking()
            .Where(pc => pc.DTgLap >= dTgBatDau && pc.DTgLap <= dTgKetThuc)
            .Join(_phieuNhapRepository.Query(),
                pc => pc.FkSMaPn,
                pn => pn.PkSMaPn,
                (pc, pn) => pn)
            .Join(_ctPhieuNhapRepository.Query(),
                pn => pn.PkSMaPn,
                ct => ct.PkFkSMaPn,
                (pn, ct) => ct)
            .Join(_sanPhamRepository.Query(),
                ct => ct.PkFkSMaSp,
                sp => sp.PkSMaSp,
                (ct, sp) => (ct.ISl ?? 0) * (sp.FDonGiaBan ?? 0))
            .SumAsync();

        var chiTietTheoSanPham = revenueByProduct
            .GroupBy(x => new { x.PkSMaSp, x.STenSp })
            .Select(g => new ThuChiTheoSanPhamDto
            {
                MaSp = g.Key.PkSMaSp,
                TenSp = g.Key.STenSp,
                SoLuong = g.Sum(x => x.SoLuong),
                ThanhTien = g.Sum(x => x.ThanhTien)
            })
            .OrderByDescending(x => x.ThanhTien)
            .ToList();

        var maBaoCao = await GenerateBaoCaoCodeAsync();
        var report = new TblBaoCaoThuChi
        {
            PkSMaBc = maBaoCao,
            DTgLap = DateTime.Now,
            FkSMaNv = string.IsNullOrWhiteSpace(maNv) ? null : maNv,
            DTgBatDau = dTgBatDau,
            DTgKetThuc = dTgKetThuc,
            BTrangThai = true
        };

        var productIds = chiTietTheoSanPham.Select(x => x.MaSp).Distinct().ToList();
        var products = await _sanPhamRepository.Query().Where(sp => productIds.Contains(sp.PkSMaSp)).ToListAsync();
        report.PkFkSMaSps = products;

        await _baoCaoThuChiRepository.AddAsync(report);
        await _context.SaveChangesAsync();

        return new ThuChiTongHopDto
        {
            MaBaoCao = maBaoCao,
            DTgBatDau = dTgBatDau,
            DTgKetThuc = dTgKetThuc,
            TongThu = tongThu,
            TongChi = tongChi,
            LoiNhuan = tongThu - tongChi,
            ChiTietTheoSanPham = chiTietTheoSanPham
        };
    }

    public async Task<byte[]> ExportThuChiTongHopExcelAsync(DateTime dTgBatDau, DateTime dTgKetThuc, string? maNv)
    {
        var report = await GenerateThuChiTongHopAsync(dTgBatDau, dTgKetThuc, maNv);

        using var workbook = new XLWorkbook();
        var ws = workbook.Worksheets.Add("BaoCaoThuChi");

        ws.Cell(1, 1).Value = "Mã báo cáo";
        ws.Cell(1, 2).Value = report.MaBaoCao;
        ws.Cell(2, 1).Value = "Từ ngày";
        ws.Cell(2, 2).Value = report.DTgBatDau;
        ws.Cell(3, 1).Value = "Đến ngày";
        ws.Cell(3, 2).Value = report.DTgKetThuc;
        ws.Cell(4, 1).Value = "Tổng thu";
        ws.Cell(4, 2).Value = report.TongThu;
        ws.Cell(5, 1).Value = "Tổng chi";
        ws.Cell(5, 2).Value = report.TongChi;
        ws.Cell(6, 1).Value = "Lợi nhuận";
        ws.Cell(6, 2).Value = report.LoiNhuan;

        ws.Cell(8, 1).Value = "Mã SP";
        ws.Cell(8, 2).Value = "Tên SP";
        ws.Cell(8, 3).Value = "Số lượng";
        ws.Cell(8, 4).Value = "Thành tiền";

        var row = 9;
        foreach (var item in report.ChiTietTheoSanPham)
        {
            ws.Cell(row, 1).Value = item.MaSp;
            ws.Cell(row, 2).Value = item.TenSp;
            ws.Cell(row, 3).Value = item.SoLuong;
            ws.Cell(row, 4).Value = item.ThanhTien;
            row++;
        }

        ws.Columns().AdjustToContents();

        using var stream = new MemoryStream();
        workbook.SaveAs(stream);
        return stream.ToArray();
    }

    private async Task<string> GenerateBaoCaoCodeAsync()
    {
        while (true)
        {
            var code = $"BC{DateTime.Now:yyyyMMddHHmmssfff}";
            if (!await _baoCaoThuChiRepository.AnyAsync(x => x.PkSMaBc == code))
            {
                return code;
            }

            await Task.Delay(2);
        }
    }
}
