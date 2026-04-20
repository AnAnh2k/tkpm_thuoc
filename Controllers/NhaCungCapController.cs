using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using CNPM.Models;
using CNPM.Services;

namespace CNPM.Controllers;

[Authorize]
public class NhaCungCapController : Controller
{
    private readonly INhaCungCapService _nhaCungCapService;

    public NhaCungCapController(INhaCungCapService nhaCungCapService)
    {
        _nhaCungCapService = nhaCungCapService;
    }

    public IActionResult Index()
    {
        return View();
    }

    [HttpGet]
    public async Task<IActionResult> GetNhaCungCapList()
    {
        var nhaCungCaps = await _nhaCungCapService.GetNhaCungCapListAsync();
        return Json(nhaCungCaps);
    }

    [HttpPost]
    public async Task<IActionResult> AddNhaCungCap([FromBody] TblNhaCungCap nhaCungCap)
    {
        if (!ModelState.IsValid)
        {
            return Json(new { success = false, message = "Dữ liệu không hợp lệ!" });
        }

        var result = await _nhaCungCapService.AddNhaCungCapAsync(nhaCungCap);
        return Json(new { success = result.Success, message = result.Message });
    }

    [HttpPost]
    public async Task<IActionResult> EditNhaCungCap([FromBody] TblNhaCungCap nhaCungCap)
    {
        if (!ModelState.IsValid)
        {
            return Json(new { success = false, message = "Dữ liệu không hợp lệ!" });
        }

        var result = await _nhaCungCapService.EditNhaCungCapAsync(nhaCungCap);
        return Json(new { success = result.Success, message = result.Message });
    }

    [HttpPost]
    public async Task<IActionResult> DeleteNhaCungCap(string maNCC)
    {
        var result = await _nhaCungCapService.DeleteNhaCungCapAsync(maNCC);
        return Json(new { success = result.Success, message = result.Message });
    }
}