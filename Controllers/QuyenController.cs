using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using CNPM.Models;
using CNPM.Services;

namespace CNPM.Controllers;

[Authorize]
public class QuyenController : Controller
{
    private readonly IQuyenService _quyenService;

    public QuyenController(IQuyenService quyenService)
    {
        _quyenService = quyenService;
    }

    public IActionResult Index()
    {
        return View();
    }

    [HttpGet]
    public async Task<IActionResult> GetQuyenList()
    {
        var quyens = await _quyenService.GetQuyenListAsync();
        return Json(quyens.Select(q => new { PkSMaQuyen = q.Id, STenQuyen = q.Name }));
    }

    [HttpPost]
    public async Task<IActionResult> AddQuyen([FromBody] TblQuyen quyen)
    {
        if (!ModelState.IsValid)
        {
            return Json(new { success = false, message = "Dữ liệu không hợp lệ!" });
        }

        var result = await _quyenService.AddQuyenAsync(quyen);
        return Json(new { success = result.Success, message = result.Message });
    }

    [HttpPost]
    public async Task<IActionResult> EditQuyen([FromBody] TblQuyen quyen)
    {
        if (!ModelState.IsValid)
        {
            return Json(new { success = false, message = "Dữ liệu không hợp lệ!" });
        }

        var result = await _quyenService.EditQuyenAsync(quyen);
        return Json(new { success = result.Success, message = result.Message });
    }

    [HttpPost]
    public async Task<IActionResult> DeleteQuyen(string maQuyen)
    {
        var result = await _quyenService.DeleteQuyenAsync(maQuyen);
        return Json(new { success = result.Success, message = result.Message });
    }
}