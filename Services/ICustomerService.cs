using CNPM.Models;
using CNPM.Services.Dtos;

namespace CNPM.Services;

public interface ICustomerService
{
    Task<List<CustomerListItemDto>> GetCustomerListAsync();
    Task<List<CustomerListItemDto>> SearchCustomers(string? sTuKhoa);
    Task<OperationResultDto> AddCustomer(TblKhachHang khachHang);
    Task<OperationResultDto> EditCustomer(TblKhachHang khachHang);
    Task<OperationResultDto> DeleteCustomer(string maKH);
    Task<List<PurchaseHistoryItemDto>> GetPurchaseHistoryAsync(string maKH);
}
