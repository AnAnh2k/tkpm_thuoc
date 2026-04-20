using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using CNPM.Models;
using CNPM.Services;

namespace CNPM.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly IHomeService _homeService;

    public HomeController(ILogger<HomeController> logger, IHomeService homeService)
    {
        _logger = logger;
        _homeService = homeService;
    }

    public IActionResult Index(string searchString, int? page, string filterType = null)
    {
        var pageSize = 12;
        var pageNumber = page ?? 1;

        var pagedSanPhams = _homeService.GetPagedSanPham(searchString, pageNumber, pageSize, filterType);

        ViewBag.SearchString = searchString;
        ViewBag.FilterType = filterType;
        return View(pagedSanPhams);
    }

    public IActionResult Details(string id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var sanPham = _homeService.GetSanPhamDetails(id);

        if (sanPham == null)
        {
            return NotFound();
        }

        return View(sanPham);
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}