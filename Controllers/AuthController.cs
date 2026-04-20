using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using CNPM.Models;
using CNPM.Services;
using Microsoft.EntityFrameworkCore;

namespace CNPM.Controllers;

public class AuthController : Controller
{
    private readonly IAuthService _authService;
    private readonly IAccountService _accountService;
    private readonly PharmacyDbContext _context;

    public AuthController(IAuthService authService, IAccountService accountService, PharmacyDbContext context)
    {
        _authService = authService;
        _accountService = accountService;
        _context = context;
    }

    [HttpGet]
    public IActionResult Login()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Login(string username, string password)
    {
        var loginResult = await _authService.LoginAsync(username, password);

        if (!loginResult.Success)
        {
            ViewBag.ErrorMessage = loginResult.ErrorMessage;
            return View();
        }

        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.Name, loginResult.UserName),
            new Claim("MaTK", loginResult.MaTaiKhoan),
            new Claim(ClaimTypes.Role, loginResult.Role),
            new Claim("FkSMaQuyen", loginResult.Role)
        };

        if (!string.IsNullOrWhiteSpace(loginResult.MaNhanVien))
        {
            claims.Add(new Claim("MaNV", loginResult.MaNhanVien));
        }

        var identity = new ClaimsIdentity(claims, "CookieAuth");
        var principal = new ClaimsPrincipal(identity);

        await HttpContext.SignInAsync("CookieAuth", principal);
        return RedirectToAction("Index", "Home");
    }

    [HttpGet]
    public async Task<IActionResult> Logout()
    {
        await HttpContext.SignOutAsync("CookieAuth");
        return RedirectToAction("Index", "Home");
    }

    [HttpGet]
    public IActionResult Register()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Register(string tenTK, string matKhau)
    {
        var registerResult = await _authService.RegisterAsync(tenTK, matKhau);

        if (!registerResult.Success)
        {
            ViewBag.ErrorMessage = registerResult.ErrorMessage;
            return View();
        }

        TempData["SuccessMessage"] = "Đăng ký tài khoản thành công! Vui lòng đăng nhập.";
        return RedirectToAction("Login");
    }

    [HttpPost]
    public async Task<IActionResult> AssignRole(string sMaTK, string sMaQuyen)
    {
        var result = await _accountService.AssignRoleAsync(sMaTK, sMaQuyen);
        return Json(new { success = result.Success, message = result.Message });
    }

    [Authorize]
    [HttpGet]
    public async Task<IActionResult> CurrentContext()
    {
        var maTK = User.FindFirst("MaTK")?.Value;
        var maNV = User.FindFirst("MaNV")?.Value;

        if (string.IsNullOrWhiteSpace(maNV) && !string.IsNullOrWhiteSpace(maTK))
        {
            maNV = await _context.TblNhanViens
                .AsNoTracking()
                .Where(nv => nv.FkSMaTk == maTK)
                .Select(nv => nv.PkSMaNv)
                .FirstOrDefaultAsync();
        }

        return Json(new { maTaiKhoan = maTK, maNhanVien = maNV });
    }

    [HttpGet]
    public IActionResult AccessDenied()
    {
        return View();
    }
}
