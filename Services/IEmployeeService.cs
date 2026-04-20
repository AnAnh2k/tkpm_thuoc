using CNPM.Models;
using CNPM.Services.Dtos;

namespace CNPM.Services;

public interface IEmployeeService
{
    Task<List<EmployeeListItemDto>> GetEmployeeListAsync();
    Task<OperationResultDto> AddEmployeeAsync(TblNhanVien nhanVien);
    Task<OperationResultDto> CreateEmployeeWithAccountAsync(EmployeeAccountCreateDto request);
    Task<OperationResultDto> EditEmployeeAsync(TblNhanVien nhanVien);
    Task<OperationResultDto> DeleteEmployeeAsync(string maNV);
    Task<EmployeeDropdownDataDto> GetDropdownDataAsync();
}
