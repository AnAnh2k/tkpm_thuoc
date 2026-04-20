using CNPM.Models;
using CNPM.Services.Dtos;

namespace CNPM.Services;

public interface IAccountService
{
    Task<List<AccountListItemDto>> GetAccountListAsync();
    Task<OperationResultDto> AddAccountAsync(TblTaiKhoan taiKhoan);
    Task<OperationResultDto> EditAccountAsync(TblTaiKhoan taiKhoan);
    Task<OperationResultDto> DeleteAccountAsync(string maTK);
    Task<OperationResultDto> AssignRoleAsync(string maTK, string maQuyen);
    Task<List<DropdownItemDto>> GetQuyenDropdownAsync();
}
