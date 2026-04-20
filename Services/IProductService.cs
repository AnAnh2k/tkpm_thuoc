using CNPM.Models;
using CNPM.Services.Dtos;

namespace CNPM.Services;

public interface IProductService
{
    Task<PagedResultDto<ProductListItemDto>> GetProductListAsync(string? sMaLoai, string? sTuKhoa, int iPage, int pageSize = 20);
    Task<OperationResultDto> AddProductAsync(TblSanPham sanPham);
    Task<OperationResultDto> EditProductAsync(TblSanPham sanPham);
    Task<OperationResultDto> DeleteProductAsync(string maSP);
    Task<List<DropdownItemDto>> GetLoaiSanPhamDropdownAsync();
    Task<List<DropdownItemDto>> GetNhaCungCapDropdownAsync();
}
