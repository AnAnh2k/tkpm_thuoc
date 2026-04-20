using CNPM.Models;
using CNPM.Services.Dtos;

namespace CNPM.Services;

public interface ISaleService
{
    Task<List<CustomerSearchDto>> SearchCustomers(string query);
    Task<List<ProductSearchDto>> SearchProductAsync(string query);
    Task<XuatKhoVaLapHoaDonResultDto> XuatKhoVaLapHoaDonAsync(
        string? maKH,
        Dictionary<string, int> sanPhamsXuat,
        string hinhThucTt,
        string? ghiChu,
        string maNguoiLap,
        string? maKhuyenMai,
        string? loaiGiamGia,
        double giaTriGiamGia,
        string? maQuyenNguoiLap);
    Task<TblPhieuThu?> GetHoaDonAsync(string maPT, bool includeNhanVien = false);
}
