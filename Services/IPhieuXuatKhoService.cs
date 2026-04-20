using CNPM.Services.Dtos;

namespace CNPM.Services;

public interface IPhieuXuatKhoService
{
    Task<List<PhieuXuatKhoListItemDto>> GetPhieuXuatKhoListAsync();
    Task<PhieuXuatKhoDetailDto?> GetChiTietPhieuXuatKhoAsync(string maPX);
}
