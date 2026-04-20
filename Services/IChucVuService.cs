using CNPM.Models;
using CNPM.Services.Dtos;

namespace CNPM.Services;

public interface IChucVuService
{
    Task<List<DropdownItemDto>> GetChucVuListAsync();
    Task<OperationResultDto> AddChucVuAsync(TblChucVu chucVu);
    Task<OperationResultDto> EditChucVuAsync(TblChucVu chucVu);
    Task<OperationResultDto> DeleteChucVuAsync(string maCV);
}
