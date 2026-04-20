using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using CNPM.Services;
using CNPM.Services.Dtos;

namespace CNPM.Controllers;

[Authorize]
public class SaleController : Controller
{
    private readonly ISaleService _saleService;

    public SaleController(ISaleService saleService)
    {
        _saleService = saleService;
    }

    public IActionResult Index()
    {
        return View();
    }

    [HttpGet]
    public async Task<IActionResult> Search(string query)
    {
        var khachHangs = await _saleService.SearchCustomers(query);
        return Json(khachHangs);
    }

    [HttpGet]
    public async Task<IActionResult> SearchProduct(string query)
    {
        var sanPhams = await _saleService.SearchProductAsync(query);
        return Json(sanPhams);
    }

    [HttpPost]
    public async Task<IActionResult> XuatKhoVaLapHoaDon(
        string? maKH,
        Dictionary<string, int> sanPhamsXuat,
        string hinhThucTt,
        string? ghiChu,
        string? maKhuyenMai,
        string? loaiGiamGia,
        double giaTriGiamGia = 0)
    {
        var maNguoiLap = User.FindFirst("MaNV")?.Value ?? "NV001";
        var maQuyenNguoiLap = User.FindFirst("FkSMaQuyen")?.Value ?? User.FindFirst(ClaimTypes.Role)?.Value;

        var result = await _saleService.XuatKhoVaLapHoaDonAsync(
            maKH,
            sanPhamsXuat,
            hinhThucTt,
            ghiChu,
            maNguoiLap,
            maKhuyenMai,
            loaiGiamGia,
            giaTriGiamGia,
            maQuyenNguoiLap);

        return Json(new
        {
            success = result.Success,
            message = result.Message,
            maPT = result.MaPT,
            maKhuyenMai = result.MaKhuyenMai,
            khuyenMaiPhanTram = result.KhuyenMaiPhanTram,
            loaiGiamGia = result.LoaiGiamGia,
            giaTriGiamGia = result.GiaTriGiamGia,
            tienGiamThuCong = result.TienGiamThuCong,
            tongTienTamTinh = result.TongTienTamTinh,
            tongTienGiam = result.TongTienGiam,
            tongThanhToan = result.TongThanhToan
        });
    }

    public async Task<IActionResult> HoaDon(
        string maPT,
        string? maKhuyenMai,
        double khuyenMaiPhanTram = 0,
        string? loaiGiamGia = null,
        double giaTriGiamGia = 0,
        double tienGiamThuCong = 0,
        double tongTienTamTinh = 0,
        double tongTienGiam = 0,
        double tongThanhToan = 0)
    {
        if (string.IsNullOrEmpty(maPT))
        {
            return NotFound("Mã phiếu thu không hợp lệ!");
        }

        var phieuThu = await _saleService.GetHoaDonAsync(maPT);
        if (phieuThu == null)
        {
            return NotFound("Không tìm thấy hóa đơn!");
        }

        var tongTamTinhTuHoaDon = phieuThu.TblCtphieuThus.Sum(ct => (ct.ISl ?? 0) * (ct.PkFkSMaSpNavigation?.FDonGiaBan ?? 0d));
        var tongTamTinhFinal = tongTienTamTinh > 0 ? tongTienTamTinh : tongTamTinhTuHoaDon;

        var tongTienGiamFinal = tongTienGiam > 0
            ? tongTienGiam
            : (khuyenMaiPhanTram > 0 || tienGiamThuCong > 0
                ? (tongTamTinhFinal * khuyenMaiPhanTram / 100d) + tienGiamThuCong
                : 0);

        var tongThanhToanFinal = tongThanhToan > 0 ? tongThanhToan : Math.Max(0, tongTamTinhFinal - tongTienGiamFinal);

        return View(new HoaDonViewDto
        {
            PhieuThu = phieuThu,
            MaKhuyenMai = maKhuyenMai,
            KhuyenMaiPhanTram = khuyenMaiPhanTram,
            LoaiGiamGia = loaiGiamGia,
            GiaTriGiamGia = giaTriGiamGia,
            TienGiamThuCong = tienGiamThuCong,
            TongTienTamTinh = tongTamTinhFinal,
            TongTienGiam = tongTienGiamFinal,
            TongThanhToan = tongThanhToanFinal
        });
    }

    [HttpGet]
    public async Task<IActionResult> ChiTietHoaDon(
        string maPT,
        string? maKhuyenMai,
        double khuyenMaiPhanTram = 0,
        string? loaiGiamGia = null,
        double giaTriGiamGia = 0,
        double tienGiamThuCong = 0,
        double tongTienTamTinh = 0,
        double tongTienGiam = 0,
        double tongThanhToan = 0)
    {
        if (string.IsNullOrEmpty(maPT))
        {
            return NotFound("Mã phiếu thu không hợp lệ!");
        }

        var phieuThu = await _saleService.GetHoaDonAsync(maPT, includeNhanVien: true);
        if (phieuThu == null)
        {
            return NotFound("Không tìm thấy hóa đơn!");
        }

        var tongTamTinhTuHoaDon = phieuThu.TblCtphieuThus.Sum(ct => (ct.ISl ?? 0) * (ct.PkFkSMaSpNavigation?.FDonGiaBan ?? 0d));
        var tongTamTinhFinal = tongTienTamTinh > 0 ? tongTienTamTinh : tongTamTinhTuHoaDon;

        var tongTienGiamFinal = tongTienGiam > 0
            ? tongTienGiam
            : (khuyenMaiPhanTram > 0 || tienGiamThuCong > 0
                ? (tongTamTinhFinal * khuyenMaiPhanTram / 100d) + tienGiamThuCong
                : 0);

        var tongThanhToanFinal = tongThanhToan > 0 ? tongThanhToan : Math.Max(0, tongTamTinhFinal - tongTienGiamFinal);

        return View(new HoaDonViewDto
        {
            PhieuThu = phieuThu,
            MaKhuyenMai = maKhuyenMai,
            KhuyenMaiPhanTram = khuyenMaiPhanTram,
            LoaiGiamGia = loaiGiamGia,
            GiaTriGiamGia = giaTriGiamGia,
            TienGiamThuCong = tienGiamThuCong,
            TongTienTamTinh = tongTamTinhFinal,
            TongTienGiam = tongTienGiamFinal,
            TongThanhToan = tongThanhToanFinal
        });
    }

    [HttpGet]
    public IActionResult History()
    {
        return RedirectToAction("Index", "PhieuThu");
    }
}
