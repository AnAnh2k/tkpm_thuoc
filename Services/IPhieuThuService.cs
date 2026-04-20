using CNPM.Services.Dtos;

namespace CNPM.Services;

public interface IPhieuThuService
{
    Task<PagedResultDto<PhieuThuListItemDto>> GetPhieuThuListAsync(DateTime? dTuNgay, DateTime? dDenNgay, string? maPT, string? maKH, string? maNguoiLap, int iPage, int pageSize = 20);
    Task<List<PhieuThuListItemDto>> GetPhieuThuExportListAsync(DateTime? dTuNgay, DateTime? dDenNgay, string? maPT, string? maKH, string? maNguoiLap);
    Task<PhieuThuDetailDto?> GetChiTietPhieuThuAsync(string maPT);
}
