using CNPM.Infrastructure.Repositories;
using CNPM.Models;
using Microsoft.EntityFrameworkCore;
using X.PagedList;
using X.PagedList.Extensions;

namespace CNPM.Services;

public class HomeService : IHomeService
{
    private readonly IRepository<TblSanPham> _sanPhamRepository;

    public HomeService(IRepository<TblSanPham> sanPhamRepository)
    {
        _sanPhamRepository = sanPhamRepository;
    }

    public IPagedList<TblSanPham> GetPagedSanPham(string? searchString, int pageNumber, int pageSize, string? filterType)
    {
        var sanPhams = _sanPhamRepository.Query().AsNoTracking();

        if (!string.IsNullOrWhiteSpace(searchString))
        {
            sanPhams = sanPhams.Where(sp => sp.STenSp != null && sp.STenSp.Contains(searchString));
        }

        if (!string.IsNullOrWhiteSpace(filterType))
        {
            if (filterType == "NoPrescription")
            {
                sanPhams = sanPhams.Where(sp => sp.FkSMaLoai == "LSP001");
            }
            else if (filterType == "Prescription")
            {
                sanPhams = sanPhams.Where(sp => sp.FkSMaLoai == "LSP002");
            }
        }

        return sanPhams
            .OrderBy(sp => sp.PkSMaSp)
            .ToPagedList(pageNumber, pageSize);
    }

    public TblSanPham? GetSanPhamDetails(string id)
    {
        return _sanPhamRepository.Query()
            .AsNoTracking()
            .Include(sp => sp.FkSMaNccNavigation)
            .FirstOrDefault(sp => sp.PkSMaSp == id);
    }
}
