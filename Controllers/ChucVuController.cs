using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using CNPM.Models;
using CNPM.Services;

namespace CNPM.Controllers;

[Authorize]
public class ChucVuController : Controller
{
    private readonly IChucVuService _chucVuService;

    public ChucVuController(IChucVuService chucVuService)
    {
        _chucVuService = chucVuService;
    }

    public IActionResult Index()
    {
        return View();
    }

    [HttpGet]
    public async Task<IActionResult> GetChucVuList()
    {
        var chucVus = await _chucVuService.GetChucVuListAsync();
        return Json(chucVus.Select(cv => new { PkSMaCv = cv.Id, STenCv = cv.Name }));
    }

    [HttpPost]
    public async Task<IActionResult> AddChucVu([FromBody] TblChucVu chucVu)
    {
        if (!ModelState.IsValid)
        {
            return Json(new { success = false, message = "Dữ liệu không hợp lệ!" });
        }

        var result = await _chucVuService.AddChucVuAsync(chucVu);
        return Json(new { success = result.Success, message = result.Message });
    }

    [HttpPost]
    public async Task<IActionResult> EditChucVu([FromBody] TblChucVu chucVu)
    {
        if (!ModelState.IsValid)
        {
            return Json(new { success = false, message = "Dữ liệu không hợp lệ!" });
        }

        var result = await _chucVuService.EditChucVuAsync(chucVu);
        return Json(new { success = result.Success, message = result.Message });
    }

    [HttpPost]
    public async Task<IActionResult> DeleteChucVu(string maCV)
    {
        var result = await _chucVuService.DeleteChucVuAsync(maCV);
        return Json(new { success = result.Success, message = result.Message });
    }
}