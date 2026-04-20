using CNPM.Services.Dtos;

namespace CNPM.Services;

public interface IAuthService
{
    Task<LoginResultDto> LoginAsync(string username, string password);
    Task<RegisterResultDto> RegisterAsync(string tenTK, string matKhau);
}
