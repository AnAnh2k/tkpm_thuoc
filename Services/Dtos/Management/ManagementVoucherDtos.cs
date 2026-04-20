namespace CNPM.Services.Dtos;

public class PhieuThuListItemDto
{
    public string PkSMaPt { get; set; } = string.Empty;
    public DateTime? DTgLap { get; set; }
    public string? FkSMaNv { get; set; }
    public string? NhanVien { get; set; }
    public string? FkSMaKh { get; set; }
    public string? KhachHang { get; set; }
    public string? SHinhThucTt { get; set; }
}

public class PhieuThuChiTietItemDto
{
    public string PkFkSMaSp { get; set; } = string.Empty;
    public string? SanPham { get; set; }
    public int? ISl { get; set; }
    public double? DonGiaBan { get; set; }
    public double ThanhTien { get; set; }
}

public class PhieuThuDetailDto
{
    public string MaPT { get; set; } = string.Empty;
    public string? KhachHang { get; set; }
    public DateTime? NgayLap { get; set; }
    public string? HinhThucTT { get; set; }
    public List<PhieuThuChiTietItemDto> ChiTiet { get; set; } = [];
}

public class PhieuXuatKhoListItemDto
{
    public string PkSMaPx { get; set; } = string.Empty;
    public DateTime? DTgLap { get; set; }
    public string? NhanVien { get; set; }
}

public class PhieuXuatKhoChiTietItemDto
{
    public string PkFkSMaSp { get; set; } = string.Empty;
    public string? SanPham { get; set; }
    public int? ISlyc { get; set; }
    public int? ISlx { get; set; }
    public string? SGhiChu { get; set; }
}

public class PhieuXuatKhoDetailDto
{
    public string MaPX { get; set; } = string.Empty;
    public string? NhanVien { get; set; }
    public DateTime? NgayLap { get; set; }
    public List<PhieuXuatKhoChiTietItemDto> ChiTiet { get; set; } = [];
}
