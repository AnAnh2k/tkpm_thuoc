using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ClosedXML.Excel;
using CNPM.Services;

namespace CNPM.Controllers;

[Authorize]
public class PhieuThuController : Controller
{
    private readonly IPhieuThuService _phieuThuService;

    public PhieuThuController(IPhieuThuService phieuThuService)
    {
        _phieuThuService = phieuThuService;
    }

    public IActionResult Index()
    {
        return View();
    }

    [HttpGet]
    public async Task<IActionResult> GetPhieuThuList(DateTime? dTuNgay, DateTime? dDenNgay, string? maPT, string? maKH, string? maNguoiLap, int iPage = 1)
    {
        var phieuThus = await _phieuThuService.GetPhieuThuListAsync(dTuNgay, dDenNgay, maPT, maKH, maNguoiLap, iPage, pageSize: 20);
        return Json(phieuThus);
    }

    [HttpGet]
    public async Task<IActionResult> ExportPhieuThuExcel(DateTime? dTuNgay, DateTime? dDenNgay, string? maPT, string? maKH, string? maNguoiLap)
    {
        var data = await _phieuThuService.GetPhieuThuExportListAsync(dTuNgay, dDenNgay, maPT, maKH, maNguoiLap);

        using var workbook = new XLWorkbook();
        var ws = workbook.Worksheets.Add("LichSuHoaDon");

        ws.Cell(1, 1).Value = "Mã phiếu thu";
        ws.Cell(1, 2).Value = "Ngày lập";
        ws.Cell(1, 3).Value = "Mã NV";
        ws.Cell(1, 4).Value = "Nhân viên";
        ws.Cell(1, 5).Value = "Mã KH";
        ws.Cell(1, 6).Value = "Khách hàng";
        ws.Cell(1, 7).Value = "Hình thức thanh toán";

        for (var i = 0; i < data.Count; i++)
        {
            var row = i + 2;
            var item = data[i];
            ws.Cell(row, 1).Value = item.PkSMaPt;
            ws.Cell(row, 2).Value = item.DTgLap;
            ws.Cell(row, 3).Value = item.FkSMaNv;
            ws.Cell(row, 4).Value = item.NhanVien;
            ws.Cell(row, 5).Value = item.FkSMaKh;
            ws.Cell(row, 6).Value = item.KhachHang;
            ws.Cell(row, 7).Value = item.SHinhThucTt;
        }

        ws.Columns().AdjustToContents();

        using var stream = new MemoryStream();
        workbook.SaveAs(stream);
        stream.Position = 0;

        var fileName = $"LichSuHoaDon_{DateTime.Now:yyyyMMddHHmmss}.xlsx";
        return File(stream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
    }

    [HttpGet]
    public async Task<IActionResult> GetChiTietPhieuThu(string maPT)
    {
        var result = await _phieuThuService.GetChiTietPhieuThuAsync(maPT);
        if (result == null)
        {
            return NotFound(new { message = "Không tìm thấy phiếu thu!" });
        }

        return Json(result);
    }
}