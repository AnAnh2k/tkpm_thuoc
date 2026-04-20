using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using CNPM.Services;

namespace CNPM.Controllers;

[Authorize]
public class ReportController : Controller
{
    private readonly IReportService _reportService;

    public ReportController(IReportService reportService)
    {
        _reportService = reportService;
    }

    public IActionResult Index()
    {
        return View();
    }

    [HttpGet]
    public async Task<IActionResult> GetDoanhThu(string loaiThongKe, DateTime? tuNgay, DateTime? denNgay)
    {
        var result = await _reportService.GetDoanhThuAsync(loaiThongKe, tuNgay, denNgay);
        return Json(result);
    }

    [HttpGet]
    public async Task<IActionResult> GetXuatKho(DateTime? tuNgay, DateTime? denNgay)
    {
        var result = await _reportService.GetXuatKhoAsync(tuNgay, denNgay);
        return Json(result);
    }

    [HttpGet]
    public async Task<IActionResult> GetTonKho()
    {
        var result = await _reportService.GetTonKhoAsync();
        return Json(result);
    }

    [HttpGet]
    public async Task<IActionResult> GetKhachHangMuaNhieu()
    {
        var result = await _reportService.GetKhachHangMuaNhieuAsync();
        return Json(result);
    }

    [HttpGet]
    public async Task<IActionResult> GetHieuSuatNhanVien(DateTime? tuNgay, DateTime? denNgay)
    {
        var result = await _reportService.GetHieuSuatNhanVienAsync(tuNgay, denNgay);
        return Json(result);
    }

    [HttpPost]
    public async Task<IActionResult> GenerateThuChiTongHop(DateTime dTgBatDau, DateTime dTgKetThuc, string? maNv)
    {
        var result = await _reportService.GenerateThuChiTongHopAsync(dTgBatDau, dTgKetThuc, maNv);
        return Json(result);
    }

    [HttpGet]
    public async Task<IActionResult> ExportThuChiTongHopExcel(DateTime dTgBatDau, DateTime dTgKetThuc, string? maNv)
    {
        var bytes = await _reportService.ExportThuChiTongHopExcelAsync(dTgBatDau, dTgKetThuc, maNv);
        var fileName = $"BaoCaoThuChi_{DateTime.Now:yyyyMMddHHmmss}.xlsx";
        return File(bytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
    }
}
