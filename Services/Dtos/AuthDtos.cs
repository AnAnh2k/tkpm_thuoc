namespace CNPM.Services.Dtos;

public class LoginResultDto
{
    public bool Success { get; set; }
    public string? ErrorMessage { get; set; }
    public string UserName { get; set; } = string.Empty;
    public string MaTaiKhoan { get; set; } = string.Empty;
    public string? MaNhanVien { get; set; }
    public string Role { get; set; } = "Unknown";
}

public class RegisterResultDto
{
    public bool Success { get; set; }
    public string? ErrorMessage { get; set; }
}
