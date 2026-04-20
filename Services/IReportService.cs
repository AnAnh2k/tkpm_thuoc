using CNPM.Services.Dtos;

namespace CNPM.Services;

public interface IReportService
{
    Task<List<DoanhThuDto>> GetDoanhThuAsync(string? loaiThongKe, DateTime? tuNgay, DateTime? denNgay);
    Task<List<XuatKhoDto>> GetXuatKhoAsync(DateTime? tuNgay, DateTime? denNgay);
    Task<List<TonKhoDto>> GetTonKhoAsync();
    Task<List<KhachHangMuaNhieuDto>> GetKhachHangMuaNhieuAsync();
    Task<List<HieuSuatNhanVienDto>> GetHieuSuatNhanVienAsync(DateTime? tuNgay, DateTime? denNgay);
    Task<ThuChiTongHopDto> GenerateThuChiTongHopAsync(DateTime dTgBatDau, DateTime dTgKetThuc, string? maNv);
    Task<byte[]> ExportThuChiTongHopExcelAsync(DateTime dTgBatDau, DateTime dTgKetThuc, string? maNv);
}
