using CNPM.Services;
using CNPM.Services.Dtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CNPM.Controllers;

[Authorize]
public class WarehouseController : Controller
{
    private readonly IWarehouseService _warehouseService;

    public WarehouseController(IWarehouseService warehouseService)
    {
        _warehouseService = warehouseService;
    }

    [HttpGet]
    public IActionResult Index()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> CreateRequest([FromBody] CreatePhieuYeuCauDto request)
    {
        var result = await _warehouseService.CreatePhieuYeuCauAsync(request);
        return Json(new { success = result.Success, message = result.Message });
    }

    [HttpGet]
    public async Task<IActionResult> GetChiTietPhieuGiao(string maPgh)
    {
        var items = await _warehouseService.GetChiTietPhieuGiaoHangAsync(maPgh);
        return Json(items);
    }

    [HttpPost]
    public async Task<IActionResult> CreateImport([FromBody] CreatePhieuNhapDto request)
    {
        var result = await _warehouseService.CreatePhieuNhapAsync(request);
        return Json(new { success = result.Success, message = result.Message });
    }

    [HttpGet]
    public async Task<IActionResult> GetTonKhoHienTai()
    {
        var items = await _warehouseService.GetTonKhoHienTaiAsync();
        return Json(items);
    }

    [HttpPost]
    public async Task<IActionResult> CreateKiemKe([FromBody] CreateBienBanKiemKeDto request)
    {
        var result = await _warehouseService.CreateBienBanKiemKeAsync(request);
        return Json(new { success = result.Success, message = result.Message });
    }

    [HttpPost]
    public async Task<IActionResult> CreateHuy([FromBody] CreateBienBanHuyDto request)
    {
        var result = await _warehouseService.CreateBienBanHuyAsync(request);
        return Json(new { success = result.Success, message = result.Message });
    }
}
