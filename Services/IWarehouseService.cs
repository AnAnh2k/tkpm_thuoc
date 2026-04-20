using CNPM.Services.Dtos;

namespace CNPM.Services;

public interface IWarehouseService
{
    Task<OperationResultDto> CreatePhieuYeuCauAsync(CreatePhieuYeuCauDto request);
    Task<OperationResultDto> CreatePhieuNhapAsync(CreatePhieuNhapDto request);
    Task<OperationResultDto> CreateBienBanKiemKeAsync(CreateBienBanKiemKeDto request);
    Task<OperationResultDto> CreateBienBanHuyAsync(CreateBienBanHuyDto request);
    Task<List<PhieuGiaoHangItemDto>> GetChiTietPhieuGiaoHangAsync(string maPgh);
    Task<List<TonKhoSnapshotDto>> GetTonKhoHienTaiAsync();
}
