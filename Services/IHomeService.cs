using CNPM.Models;
using X.PagedList;

namespace CNPM.Services;

public interface IHomeService
{
    IPagedList<TblSanPham> GetPagedSanPham(string? searchString, int pageNumber, int pageSize, string? filterType);
    TblSanPham? GetSanPhamDetails(string id);
}
