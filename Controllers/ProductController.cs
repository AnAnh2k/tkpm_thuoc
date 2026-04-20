using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using CNPM.Models;
using CNPM.Services;

namespace CNPM.Controllers;

[Authorize]
public class ProductController : Controller
{
    private readonly IProductService _productService;

    public ProductController(IProductService productService)
    {
        _productService = productService;
    }

    public IActionResult Index()
    {
        return View();
    }

    /// <summary>
    /// Đáp ứng thiết kế 4.3.1.1 + 4.3.1.3: lọc theo loại, tìm kiếm theo mã/tên, phân trang 20 bản ghi.
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> GetProductList(string? sMaLoai, string? sTuKhoa, int iPage = 1)
    {
        var sanPhams = await _productService.GetProductListAsync(sMaLoai, sTuKhoa, iPage, pageSize: 20);
        return Json(sanPhams);
    }

    /// <summary>
    /// Thiết kế 4.3.1.2: kiểm tra mã trùng, tên không trống, đơn giá &gt; 0.
    /// </summary>
    [HttpPost]
    public async Task<IActionResult> AddProduct([FromBody] TblSanPham sanPham)
    {
        if (!ModelState.IsValid)
        {
            return Json(new { success = false, message = "Dữ liệu không hợp lệ!" });
        }

        var result = await _productService.AddProductAsync(sanPham);
        return Json(new { success = result.Success, message = result.Message });
    }

    /// <summary>
    /// Thiết kế 4.3.1.4: tên không trống, đơn giá &gt; 0.
    /// </summary>
    [HttpPost]
    public async Task<IActionResult> EditProduct([FromBody] TblSanPham sanPham)
    {
        if (!ModelState.IsValid)
        {
            return Json(new { success = false, message = "Dữ liệu không hợp lệ!" });
        }

        var result = await _productService.EditProductAsync(sanPham);
        return Json(new { success = result.Success, message = result.Message });
    }

    /// <summary>
    /// Thiết kế 4.3.1.5: không xóa nếu có giao dịch ở CT phiếu thu / xuất / nhập.
    /// </summary>
    [HttpPost]
    public async Task<IActionResult> DeleteProduct(string maSP)
    {
        var result = await _productService.DeleteProductAsync(maSP);
        return Json(new { success = result.Success, message = result.Message });
    }

    [HttpGet]
    public async Task<IActionResult> GetLoaiSanPhamDropdown()
    {
        var loaiSanPhams = await _productService.GetLoaiSanPhamDropdownAsync();
        return Json(loaiSanPhams.Select(x => new { PkSMaLoai = x.Id, STenLoai = x.Name }));
    }

    [HttpGet]
    public async Task<IActionResult> GetNhaCungCapDropdown()
    {
        var nhaCungCaps = await _productService.GetNhaCungCapDropdownAsync();
        return Json(nhaCungCaps.Select(x => new { PkSMaNcc = x.Id, STenNcc = x.Name }));
    }

    /// <summary>
    /// Thiết kế 4.3.1.3: tìm kiếm theo từ khóa + mã loại, trả JSON danh sách.
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> Search(string? sTuKhoa, string? fkSMaLoai)
    {
        var result = await _productService.GetProductListAsync(fkSMaLoai, sTuKhoa, iPage: 1, pageSize: 200);
        return Json(new
        {
            items = result.Items,
            message = result.Items.Count == 0 ? "Không tìm thấy sản phẩm phù hợp" : null
        });
    }
}
