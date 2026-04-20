using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using CNPM.Services;

namespace CNPM.Controllers;

[Authorize]
public class PhieuXuatKhoController : Controller
{
    private readonly IPhieuXuatKhoService _phieuXuatKhoService;

    public PhieuXuatKhoController(IPhieuXuatKhoService phieuXuatKhoService)
    {
        _phieuXuatKhoService = phieuXuatKhoService;
    }

    public IActionResult Index()
    {
        return View();
    }

    [HttpGet]
    public async Task<IActionResult> GetPhieuXuatKhoList()
    {
        var phieuXuatKhos = await _phieuXuatKhoService.GetPhieuXuatKhoListAsync();
        return Json(phieuXuatKhos);
    }

    [HttpGet]
    public async Task<IActionResult> GetChiTietPhieuXuatKho(string maPX)
    {
        var result = await _phieuXuatKhoService.GetChiTietPhieuXuatKhoAsync(maPX);
        if (result == null)
        {
            return NotFound(new { message = "Không tìm thấy phiếu xuất kho!" });
        }

        return Json(result);
    }
}