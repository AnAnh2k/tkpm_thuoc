using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using CNPM.Models;
using CNPM.Services;

namespace CNPM.Controllers;

[Authorize]
public class CustomerController : Controller
{
    private readonly ICustomerService _customerService;

    public CustomerController(ICustomerService customerService)
    {
        _customerService = customerService;
    }

    public IActionResult Index()
    {
        return View();
    }

    [HttpGet]
    public async Task<IActionResult> GetCustomerList()
    {
        var khachHangs = await _customerService.GetCustomerListAsync();
        return Json(khachHangs);
    }

    [HttpGet]
    public async Task<IActionResult> Search(string? sTuKhoa)
    {
        var khachHangs = await _customerService.SearchCustomers(sTuKhoa);
        return Json(khachHangs);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] KhachHangViewModel model)
    {
        var errors = new List<string>();

        if (!ModelState.IsValid)
        {
            errors.AddRange(ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage));
        }

        if (errors.Any())
        {
            return Json(new { success = false, message = string.Join(" ", errors) });
        }

        var khachHang = new TblKhachHang
        {
            PkSMaKh = model.PkSMaKh,
            STenKh = model.STenKh,
            SSdt = model.SSdt,
            SDiaChi = model.SDiaChi,
            DNgaySinh = model.DNgaySinh
        };

        var result = await _customerService.AddCustomer(khachHang);
        return Json(new { success = result.Success, message = result.Message });
    }

    [HttpPost]
    public async Task<IActionResult> EditCustomer([FromBody] KhachHangViewModel model)
    {
        if (!ModelState.IsValid)
        {
            var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage);
            return Json(new { success = false, message = string.Join(" ", errors) });
        }

        var khachHang = new TblKhachHang
        {
            PkSMaKh = model.PkSMaKh,
            STenKh = model.STenKh,
            SSdt = model.SSdt,
            SDiaChi = model.SDiaChi,
            DNgaySinh = model.DNgaySinh
        };

        var result = await _customerService.EditCustomer(khachHang);
        return Json(new { success = result.Success, message = result.Message });
    }

    [HttpPost]
    public async Task<IActionResult> DeleteCustomer(string maKH)
    {
        var result = await _customerService.DeleteCustomer(maKH);
        return Json(new { success = result.Success, message = result.Message });
    }

    [HttpGet]
    public async Task<IActionResult> GetPurchaseHistory(string maKH)
    {
        var history = await _customerService.GetPurchaseHistoryAsync(maKH);
        return Json(history);
    }
}
