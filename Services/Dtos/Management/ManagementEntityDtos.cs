namespace CNPM.Services.Dtos;

public class CustomerListItemDto
{
    public string PkSMaKh { get; set; } = string.Empty;
    public string? STenKh { get; set; }
    public string? SSdt { get; set; }
    public string? SDiaChi { get; set; }
    public DateTime? DNgaySinh { get; set; }
}

public class PurchaseHistoryItemDto
{
    public string PkSMaPt { get; set; } = string.Empty;
    public DateTime? DTgLap { get; set; }
    public string? SHinhThucTt { get; set; }
    public List<PurchaseHistoryProductDto> Products { get; set; } = [];
}

public class PurchaseHistoryProductDto
{
    public string MaSanPham { get; set; } = string.Empty;
    public string? TenSanPham { get; set; }
    public int? SoLuong { get; set; }
    public double? DonGia { get; set; }
}

public class NhaCungCapListItemDto
{
    public string PkSMaNcc { get; set; } = string.Empty;
    public string? STenNcc { get; set; }
    public string? SDiaChi { get; set; }
    public string? SSdt { get; set; }
    public string? SSoTk { get; set; }
}

public class AccountListItemDto
{
    public string PkSMaTk { get; set; } = string.Empty;
    public string? STenTk { get; set; }
    public string? SMk { get; set; }
    public string? FkSMaQuyen { get; set; }
    public string? Quyen { get; set; }
}

public class ProductListItemDto
{
    public string PkSMaSp { get; set; } = string.Empty;
    public string? STenSp { get; set; }
    public string? SDonViTinh { get; set; }
    public DateOnly? SHanDung { get; set; }
    public int? ISl { get; set; }
    public double? FDonGiaBan { get; set; }
    public string? FkSMaLoai { get; set; }
    public string? FkSMaNcc { get; set; }
    public string? LoaiSanPham { get; set; }
    public string? NhaCungCap { get; set; }
    public bool CanhBaoTon { get; set; }
    public bool CanhBaoHan { get; set; }
    public string RowClass { get; set; } = string.Empty;
}

public class EmployeeListItemDto
{
    public string PkSMaNv { get; set; } = string.Empty;
    public string? SHoTen { get; set; }
    public DateTime? DNgaySinh { get; set; }
    public string? SCccd { get; set; }
    public string? SSdt { get; set; }
    public DateTime? DNgayVaoLam { get; set; }
    public string? FkSMaTk { get; set; }
    public string? TaiKhoan { get; set; }
    public string? FkSMaCv { get; set; }
    public string? ChucVu { get; set; }
    public bool? BTrangThai { get; set; }
}

public class AccountDropdownItemDto
{
    public string PkSMaTk { get; set; } = string.Empty;
    public string? STenTk { get; set; }
}

public class ChucVuDropdownItemDto
{
    public string PkSMaCv { get; set; } = string.Empty;
    public string? STenCv { get; set; }
}

public class EmployeeDropdownDataDto
{
    public List<AccountDropdownItemDto> Accounts { get; set; } = [];
    public List<ChucVuDropdownItemDto> ChucVus { get; set; } = [];
}

public class EmployeeAccountCreateDto
{
    public string PkSMaNv { get; set; } = string.Empty;
    public string? SHoTen { get; set; }
    public DateTime? DNgaySinh { get; set; }
    public string? SCccd { get; set; }
    public string? SSdt { get; set; }
    public DateTime? DNgayVaoLam { get; set; }
    public string? FkSMaCv { get; set; }
    public string STenTk { get; set; } = string.Empty;
    public string SMk { get; set; } = string.Empty;
    public string? FkSMaQuyen { get; set; }
}
