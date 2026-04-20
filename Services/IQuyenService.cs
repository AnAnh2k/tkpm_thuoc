using CNPM.Models;
using CNPM.Services.Dtos;

namespace CNPM.Services;

public interface IQuyenService
{
    Task<List<DropdownItemDto>> GetQuyenListAsync();
    Task<OperationResultDto> AddQuyenAsync(TblQuyen quyen);
    Task<OperationResultDto> EditQuyenAsync(TblQuyen quyen);
    Task<OperationResultDto> DeleteQuyenAsync(string maQuyen);
}
