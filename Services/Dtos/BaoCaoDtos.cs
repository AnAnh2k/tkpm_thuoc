namespace CNPM.Services.Dtos;

public class DoanhThuDto
{
    public string ThoiGian { get; set; } = string.Empty;
    public double TongDoanhThu { get; set; }
}

public class XuatKhoChiTietDto
{
    public string STenSp { get; set; } = string.Empty;
    public int ISlx { get; set; }
}

public class XuatKhoDto
{
    public DateTime Ngay { get; set; }
    public int TongSoLuongXuat { get; set; }
    public List<XuatKhoChiTietDto> ChiTiet { get; set; } = [];
}

public class TonKhoDto
{
    public string PkSMaSp { get; set; } = string.Empty;
    public string? STenSp { get; set; }
    public int? ISl { get; set; }
    public DateOnly? SHanDung { get; set; }
    public string TrangThai { get; set; } = string.Empty;
}

public class ChiTietHoaDonDto
{
    public string MaSP { get; set; } = string.Empty;
    public string? TenSP { get; set; }
    public int SoLuong { get; set; }
    public double DonGia { get; set; }
    public double ThanhTien { get; set; }
    public bool KeDon { get; set; }
}

public class HoaDonKhachHangDto
{
    public string MaPT { get; set; } = string.Empty;
    public DateTime? NgayLap { get; set; }
    public string? HinhThucTT { get; set; }
    public double TongTien { get; set; }
    public List<ChiTietHoaDonDto> ChiTietHoaDon { get; set; } = [];
}

public class KhachHangMuaNhieuDto
{
    public string MaKH { get; set; } = string.Empty;
    public string? TenKH { get; set; }
    public string? SoDienThoai { get; set; }
    public List<HoaDonKhachHangDto> HoaDons { get; set; } = [];
}

public class HieuSuatNhanVienDto
{
    public string MaNV { get; set; } = string.Empty;
    public string? TenNV { get; set; }
    public int SoPhieuThu { get; set; }
    public int SoPhieuXuat { get; set; }
    public double TongDoanhThu { get; set; }
}
