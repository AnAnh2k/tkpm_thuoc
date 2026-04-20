using CNPM.Models;
using CNPM.Services.Dtos;

namespace CNPM.Services;

public interface INhaCungCapService
{
    Task<List<NhaCungCapListItemDto>> GetNhaCungCapListAsync();
    Task<OperationResultDto> AddNhaCungCapAsync(TblNhaCungCap nhaCungCap);
    Task<OperationResultDto> EditNhaCungCapAsync(TblNhaCungCap nhaCungCap);
    Task<OperationResultDto> DeleteNhaCungCapAsync(string maNCC);
}
