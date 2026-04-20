using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using CNPM.Models;
using CNPM.Services;

namespace CNPM.Controllers;

[Authorize]
public class AccountController : Controller
{
    private readonly IAccountService _accountService;

    public AccountController(IAccountService accountService)
    {
        _accountService = accountService;
    }

    public IActionResult Index()
    {
        return View();
    }

    [HttpGet]
    public async Task<IActionResult> GetAccountList()
    {
        var accounts = await _accountService.GetAccountListAsync();
        return Json(accounts);
    }

    [HttpPost]
    public async Task<IActionResult> AddAccount([FromBody] TblTaiKhoan taiKhoan)
    {
        if (!ModelState.IsValid)
        {
            return Json(new { success = false, message = "Dữ liệu không hợp lệ!" });
        }

        var result = await _accountService.AddAccountAsync(taiKhoan);
        return Json(new { success = result.Success, message = result.Message });
    }

    [HttpPost]
    public async Task<IActionResult> EditAccount([FromBody] TblTaiKhoan taiKhoan)
    {
        if (!ModelState.IsValid)
        {
            return Json(new { success = false, message = "Dữ liệu không hợp lệ!" });
        }

        var result = await _accountService.EditAccountAsync(taiKhoan);
        return Json(new { success = result.Success, message = result.Message });
    }

    [HttpPost]
    public async Task<IActionResult> DeleteAccount(string maTK)
    {
        var result = await _accountService.DeleteAccountAsync(maTK);
        return Json(new { success = result.Success, message = result.Message });
    }

    [HttpGet]
    public async Task<IActionResult> GetQuyenDropdown()
    {
        var quyens = await _accountService.GetQuyenDropdownAsync();
        return Json(quyens.Select(q => new { PkSMaQuyen = q.Id, STenQuyen = q.Name }));
    }
}
