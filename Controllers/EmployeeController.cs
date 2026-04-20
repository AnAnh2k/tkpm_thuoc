using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using CNPM.Models;
using CNPM.Services;
using CNPM.Services.Dtos;

namespace CNPM.Controllers;

[Authorize]
public class EmployeeController : Controller
{
    private readonly IEmployeeService _employeeService;

    public EmployeeController(IEmployeeService employeeService)
    {
        _employeeService = employeeService;
    }

    public IActionResult Index()
    {
        return View();
    }

    [HttpGet]
    public async Task<IActionResult> GetEmployeeList()
    {
        var nhanViens = await _employeeService.GetEmployeeListAsync();
        return Json(nhanViens);
    }

    [HttpPost]
    public async Task<IActionResult> AddEmployee([FromBody] TblNhanVien nhanVien)
    {
        if (!ModelState.IsValid)
        {
            return Json(new { success = false, message = "Dữ liệu không hợp lệ!" });
        }

        var result = await _employeeService.AddEmployeeAsync(nhanVien);
        return Json(new { success = result.Success, message = result.Message });
    }

    [HttpPost]
    public async Task<IActionResult> CreateEmployeeWithAccount([FromBody] EmployeeAccountCreateDto request)
    {
        var result = await _employeeService.CreateEmployeeWithAccountAsync(request);
        return Json(new { success = result.Success, message = result.Message });
    }

    [HttpPost]
    public async Task<IActionResult> EditEmployee([FromBody] TblNhanVien nhanVien)
    {
        if (!ModelState.IsValid)
        {
            return Json(new { success = false, message = "Dữ liệu không hợp lệ!" });
        }

        var result = await _employeeService.EditEmployeeAsync(nhanVien);
        return Json(new { success = result.Success, message = result.Message });
    }

    [HttpPost]
    public async Task<IActionResult> DeleteEmployee(string maNV)
    {
        var result = await _employeeService.DeleteEmployeeAsync(maNV);
        return Json(new { success = result.Success, message = result.Message });
    }

    [HttpGet]
    public async Task<IActionResult> GetDropdownData()
    {
        var data = await _employeeService.GetDropdownDataAsync();
        return Json(new { accounts = data.Accounts, chucVus = data.ChucVus });
    }
}
