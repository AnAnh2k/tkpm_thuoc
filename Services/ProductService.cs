using CNPM.Infrastructure.Repositories;
using CNPM.Models;
using CNPM.Services.Dtos;
using Microsoft.EntityFrameworkCore;

namespace CNPM.Services;

public class ProductService : IProductService
{
    private readonly PharmacyDbContext _context;
    private readonly IRepository<TblSanPham> _sanPhamRepository;
    private readonly IRepository<TblCtphieuThu> _ctPhieuThuRepository;
    private readonly IRepository<TblCtphieuXuatKho> _ctPhieuXuatKhoRepository;
    private readonly IRepository<TblCtphieuNhapKho> _ctPhieuNhapKhoRepository;
    private readonly IRepository<TblLoaiSanPham> _loaiSanPhamRepository;
    private readonly IRepository<TblNhaCungCap> _nhaCungCapRepository;

    public ProductService(
        PharmacyDbContext context,
        IRepository<TblSanPham> sanPhamRepository,
        IRepository<TblCtphieuThu> ctPhieuThuRepository,
        IRepository<TblCtphieuXuatKho> ctPhieuXuatKhoRepository,
        IRepository<TblCtphieuNhapKho> ctPhieuNhapKhoRepository,
        IRepository<TblLoaiSanPham> loaiSanPhamRepository,
        IRepository<TblNhaCungCap> nhaCungCapRepository)
    {
        _context = context;
        _sanPhamRepository = sanPhamRepository;
        _ctPhieuThuRepository = ctPhieuThuRepository;
        _ctPhieuXuatKhoRepository = ctPhieuXuatKhoRepository;
        _ctPhieuNhapKhoRepository = ctPhieuNhapKhoRepository;
        _loaiSanPhamRepository = loaiSanPhamRepository;
        _nhaCungCapRepository = nhaCungCapRepository;
    }

    public async Task<PagedResultDto<ProductListItemDto>> GetProductListAsync(string? sMaLoai, string? sTuKhoa, int iPage, int pageSize = 20)
    {
        var today = DateOnly.FromDateTime(DateTime.Now);

        var query = _sanPhamRepository.Query()
            .AsNoTracking()
            .Include(sp => sp.FkSMaLoaiNavigation)
            .Include(sp => sp.FkSMaNccNavigation)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(sMaLoai))
        {
            query = query.Where(sp => sp.FkSMaLoai == sMaLoai);
        }

        if (!string.IsNullOrWhiteSpace(sTuKhoa))
        {
            var keyword = sTuKhoa.Trim();
            query = query.Where(sp =>
                sp.PkSMaSp.Contains(keyword) ||
                (sp.STenSp != null && sp.STenSp.Contains(keyword)));
        }

        var totalItems = await query.CountAsync();
        var safePage = iPage <= 0 ? 1 : iPage;
        var totalPages = totalItems == 0 ? 1 : (int)Math.Ceiling(totalItems / (double)pageSize);
        if (safePage > totalPages)
        {
            safePage = totalPages;
        }

        var sanPhams = await query
            .OrderBy(sp => sp.PkSMaSp)
            .Skip((safePage - 1) * pageSize)
            .Take(pageSize)
            .Select(sp => new ProductListItemDto
            {
                PkSMaSp = sp.PkSMaSp,
                STenSp = sp.STenSp,
                SDonViTinh = sp.SDonViTinh,
                SHanDung = sp.SHanDung,
                ISl = sp.ISl,
                FDonGiaBan = sp.FDonGiaBan,
                FkSMaLoai = sp.FkSMaLoai,
                FkSMaNcc = sp.FkSMaNcc,
                LoaiSanPham = sp.FkSMaLoaiNavigation != null ? sp.FkSMaLoaiNavigation.STenLoai : null,
                NhaCungCap = sp.FkSMaNccNavigation != null ? sp.FkSMaNccNavigation.STenNcc : null,
                CanhBaoTon = (sp.ISl ?? 0) == 0,
                CanhBaoHan = sp.SHanDung != null && sp.SHanDung.Value <= today.AddDays(30),
                RowClass = (sp.ISl ?? 0) == 0
                    ? "table-danger"
                    : (sp.SHanDung != null && sp.SHanDung.Value <= today.AddDays(30))
                        ? "table-warning"
                        : string.Empty
            })
            .ToListAsync();

        return new PagedResultDto<ProductListItemDto>
        {
            Items = sanPhams,
            CurrentPage = safePage,
            PageSize = pageSize,
            TotalItems = totalItems,
            TotalPages = totalPages,
            Message = totalItems == 0 ? "Không có sản phẩm nào phù hợp" : null
        };
    }

    public async Task<OperationResultDto> AddProductAsync(TblSanPham sanPham)
    {
        if (string.IsNullOrWhiteSpace(sanPham.STenSp))
        {
            return new OperationResultDto { Success = false, Message = "Tên sản phẩm không được để trống." };
        }

        if (sanPham.FDonGiaBan is null or <= 0)
        {
            return new OperationResultDto { Success = false, Message = "Đơn giá bán phải lớn hơn 0." };
        }

        if (await _sanPhamRepository.AnyAsync(sp => sp.PkSMaSp == sanPham.PkSMaSp))
        {
            return new OperationResultDto { Success = false, Message = "Mã sản phẩm đã tồn tại, vui lòng kiểm tra lại." };
        }

        await _sanPhamRepository.AddAsync(sanPham);

        try
        {
            await _context.SaveChangesAsync();
        }
        catch
        {
            return new OperationResultDto { Success = false, Message = "Thêm thất bại, vui lòng thử lại." };
        }

        return new OperationResultDto { Success = true, Message = "Thêm sản phẩm thành công!" };
    }

    public async Task<OperationResultDto> EditProductAsync(TblSanPham sanPham)
    {
        if (string.IsNullOrWhiteSpace(sanPham.STenSp))
        {
            return new OperationResultDto { Success = false, Message = "Tên sản phẩm không được để trống." };
        }

        if (sanPham.FDonGiaBan is null or <= 0)
        {
            return new OperationResultDto { Success = false, Message = "Đơn giá bán phải lớn hơn 0." };
        }

        var existingSanPham = await _sanPhamRepository.FirstOrDefaultAsync(sp => sp.PkSMaSp == sanPham.PkSMaSp);
        if (existingSanPham == null)
        {
            return new OperationResultDto { Success = false, Message = "Không tìm thấy sản phẩm!" };
        }

        existingSanPham.STenSp = sanPham.STenSp;
        existingSanPham.SDonViTinh = sanPham.SDonViTinh;
        existingSanPham.SHanDung = sanPham.SHanDung;
        existingSanPham.ISl = sanPham.ISl;
        existingSanPham.FDonGiaBan = sanPham.FDonGiaBan;
        existingSanPham.FkSMaLoai = sanPham.FkSMaLoai;
        existingSanPham.FkSMaNcc = sanPham.FkSMaNcc;

        try
        {
            await _context.SaveChangesAsync();
        }
        catch
        {
            return new OperationResultDto { Success = false, Message = "Cập nhật thất bại." };
        }

        return new OperationResultDto { Success = true, Message = "Cập nhật thành công!" };
    }

    public async Task<OperationResultDto> DeleteProductAsync(string maSP)
    {
        var sanPham = await _sanPhamRepository.FirstOrDefaultAsync(sp => sp.PkSMaSp == maSP);
        if (sanPham == null)
        {
            return new OperationResultDto { Success = false, Message = "Không tìm thấy sản phẩm!" };
        }

        var coPhieuThu = await _ctPhieuThuRepository.AnyAsync(ct => ct.PkFkSMaSp == maSP);
        var coPhieuXuat = await _ctPhieuXuatKhoRepository.AnyAsync(ct => ct.PkFkSMaSp == maSP);
        var coPhieuNhap = await _ctPhieuNhapKhoRepository.AnyAsync(ct => ct.PkFkSMaSp == maSP);

        if (coPhieuThu || coPhieuXuat || coPhieuNhap)
        {
            return new OperationResultDto
            {
                Success = false,
                Message = "Không thể xóa — sản phẩm đang có lịch sử giao dịch. Hệ thống sẽ chỉ lưu lại thông tin."
            };
        }

        _context.TblSanPhams.Remove(sanPham);
        await _context.SaveChangesAsync();

        return new OperationResultDto { Success = true, Message = "Xóa thành công!" };
    }

    public Task<List<DropdownItemDto>> GetLoaiSanPhamDropdownAsync()
    {
        return _loaiSanPhamRepository.Query()
            .AsNoTracking()
            .Select(l => new DropdownItemDto
            {
                Id = l.PkSMaLoai,
                Name = l.STenLoai
            })
            .ToListAsync();
    }

    public Task<List<DropdownItemDto>> GetNhaCungCapDropdownAsync()
    {
        return _nhaCungCapRepository.Query()
            .AsNoTracking()
            .Select(n => new DropdownItemDto
            {
                Id = n.PkSMaNcc,
                Name = n.STenNcc
            })
            .ToListAsync();
    }
}
