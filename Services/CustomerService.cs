using CNPM.Infrastructure.Repositories;
using CNPM.Models;
using CNPM.Services.Dtos;
using Microsoft.EntityFrameworkCore;

namespace CNPM.Services;

public class CustomerService : ICustomerService
{
    private readonly PharmacyDbContext _context;
    private readonly IRepository<TblKhachHang> _khachHangRepository;

    public CustomerService(PharmacyDbContext context, IRepository<TblKhachHang> khachHangRepository)
    {
        _context = context;
        _khachHangRepository = khachHangRepository;
    }

    public async Task<List<CustomerListItemDto>> GetCustomerListAsync()
    {
        var customers = await _khachHangRepository.Query()
            .AsNoTracking()
            .OrderBy(kh => kh.PkSMaKh)
            .ToListAsync();

        return customers
            .Select(kh =>
            {
                CustomerDataProtection.DecryptCustomer(kh);
                return new CustomerListItemDto
                {
                    PkSMaKh = kh.PkSMaKh,
                    STenKh = kh.STenKh,
                    SSdt = kh.SSdt,
                    SDiaChi = kh.SDiaChi,
                    DNgaySinh = kh.DNgaySinh
                };
            })
            .ToList();
    }

    public async Task<List<CustomerListItemDto>> SearchCustomers(string? sTuKhoa)
    {
        var keyword = sTuKhoa?.Trim();

        var customers = await _khachHangRepository.Query()
            .AsNoTracking()
            .OrderBy(kh => kh.PkSMaKh)
            .ToListAsync();

        var decrypted = customers.Select(kh =>
        {
            CustomerDataProtection.DecryptCustomer(kh);
            return new CustomerListItemDto
            {
                PkSMaKh = kh.PkSMaKh,
                STenKh = kh.STenKh,
                SSdt = kh.SSdt,
                SDiaChi = kh.SDiaChi,
                DNgaySinh = kh.DNgaySinh
            };
        });

        if (string.IsNullOrWhiteSpace(keyword))
        {
            return decrypted.ToList();
        }

        return decrypted
            .Where(kh => kh.PkSMaKh.Contains(keyword, StringComparison.OrdinalIgnoreCase)
                         || (!string.IsNullOrWhiteSpace(kh.STenKh)
                             && kh.STenKh.Contains(keyword, StringComparison.OrdinalIgnoreCase))
                         || (!string.IsNullOrWhiteSpace(kh.SSdt)
                             && kh.SSdt.Contains(keyword, StringComparison.OrdinalIgnoreCase)))
            .ToList();
    }

    public async Task<OperationResultDto> AddCustomer(TblKhachHang khachHang)
    {
        try 
        {
            if (await _khachHangRepository.AnyAsync(kh => kh.PkSMaKh == khachHang.PkSMaKh))
            {
                return new OperationResultDto { Success = false, Message = "Mã khách hàng đã tồn tại" };
            }

            // Kiểm tra trùng SĐT
            var allCustomers = await _khachHangRepository.Query().ToListAsync();
            foreach (var c in allCustomers)
            {
                CustomerDataProtection.DecryptCustomer(c);
                if (c.SSdt == khachHang.SSdt)
                {
                    return new OperationResultDto { Success = false, Message = "Số điện thoại này đã được đăng ký!" };
                }
            }

            CustomerDataProtection.EncryptCustomer(khachHang);
            await _khachHangRepository.AddAsync(khachHang);
            await _context.SaveChangesAsync();

            return new OperationResultDto { Success = true, Message = "Thêm khách hàng thành công!" };
        }
        catch (Exception)
        {
            return new OperationResultDto { Success = false, Message = "Lỗi kết nối hệ thống, vui lòng thử lại!" };
        }
    }

    public async Task<OperationResultDto> EditCustomer(TblKhachHang khachHang)
    {
        try
        {
            var existingKhachHang = await _khachHangRepository.FirstOrDefaultAsync(kh => kh.PkSMaKh == khachHang.PkSMaKh);
            if (existingKhachHang == null)
            {
                return new OperationResultDto { Success = false, Message = "Không tìm thấy khách hàng!" };
            }

            // Kiểm tra trùng SĐT nếu có thay đổi
            var allCustomers = await _khachHangRepository.Query().Where(kh => kh.PkSMaKh != khachHang.PkSMaKh).ToListAsync();
            foreach (var c in allCustomers)
            {
                CustomerDataProtection.DecryptCustomer(c);
                if (c.SSdt == khachHang.SSdt)
                {
                    return new OperationResultDto { Success = false, Message = "Số điện thoại đã thuộc về khách hàng khác!" };
                }
            }

            existingKhachHang.STenKh = khachHang.STenKh;
            existingKhachHang.SSdt = khachHang.SSdt;
            existingKhachHang.SDiaChi = khachHang.SDiaChi;
            existingKhachHang.DNgaySinh = khachHang.DNgaySinh;
            CustomerDataProtection.EncryptCustomer(existingKhachHang);

            await _context.SaveChangesAsync();

            return new OperationResultDto { Success = true, Message = "Sửa khách hàng thành công!" };
        }
        catch (Exception)
        {
            return new OperationResultDto { Success = false, Message = "Lỗi hệ thống khi cập nhật dữ liệu!" };
        }
    }

    public async Task<OperationResultDto> DeleteCustomer(string maKH)
    {
        try
        {
            var khachHang = await _context.TblKhachHangs
                .Include(kh => kh.TblPhieuThus)
                .FirstOrDefaultAsync(kh => kh.PkSMaKh == maKH);

            if (khachHang == null)
            {
                return new OperationResultDto { Success = false, Message = "Không tìm thấy khách hàng!" };
            }

            // Kiểm tra lịch sử giao dịch (Khớp AF1 trong thiết kế)
            if (khachHang.TblPhieuThus.Any())
            {
                return new OperationResultDto { Success = false, Message = "Khách hàng có lịch sử mua hàng không thể xóa" };
            }

            // Nếu chưa có giao dịch -> Xóa vĩnh viễn (Hard Delete)
            _context.TblKhachHangs.Remove(khachHang);
            await _context.SaveChangesAsync();

            return new OperationResultDto { Success = true, Message = "Xóa khách hàng thành công!" };
        }
        catch (Exception)
        {
            return new OperationResultDto { Success = false, Message = "Lỗi hệ thống khi thực hiện xóa!" };
        }
    }

    public async Task<List<PurchaseHistoryItemDto>> GetPurchaseHistoryAsync(string maKH)
    {
        var phieuThus = await _context.TblPhieuThus
            .Where(pt => pt.FkSMaKh == maKH)
            .Include(pt => pt.TblCtphieuThus)
                .ThenInclude(ct => ct.PkFkSMaSpNavigation)
            .OrderByDescending(pt => pt.DTgLap)
            .AsNoTracking()
            .ToListAsync();

        return phieuThus.Select(pt => new PurchaseHistoryItemDto
        {
            PkSMaPt = pt.PkSMaPt,
            DTgLap = pt.DTgLap,
            SHinhThucTt = pt.SHinhThucTt,
            Products = pt.TblCtphieuThus.Select(ct => new PurchaseHistoryProductDto
            {
                MaSanPham = ct.PkFkSMaSp,
                TenSanPham = ct.PkFkSMaSpNavigation?.STenSp,
                SoLuong = ct.ISl,
                DonGia = ct.PkFkSMaSpNavigation?.FDonGiaBan
            }).ToList()
        }).ToList();
    }
}
