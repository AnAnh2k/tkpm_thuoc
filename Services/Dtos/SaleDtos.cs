namespace CNPM.Services.Dtos;

public class CustomerSearchDto
{
    public string PkSMaKh { get; set; } = string.Empty;
    public string? STenKh { get; set; }
    public string? SSdt { get; set; }
}

public class ProductSearchDto
{
    public string PkSMaSp { get; set; } = string.Empty;
    public string? STenSp { get; set; }
    public int? ISl { get; set; }
    public double? FDonGiaBan { get; set; }
    public bool KeDon { get; set; }
}

public class XuatKhoVaLapHoaDonResultDto
{
    public bool Success { get; set; }
    public string? Message { get; set; }
    public string? MaPT { get; set; }
    public string? MaKhuyenMai { get; set; }
    public double KhuyenMaiPhanTram { get; set; }
    public string? LoaiGiamGia { get; set; }
    public double GiaTriGiamGia { get; set; }
    public double TienGiamThuCong { get; set; }
    public double TongTienTamTinh { get; set; }
    public double TongTienGiam { get; set; }
    public double TongThanhToan { get; set; }
}

public class HoaDonViewDto
{
    public required CNPM.Models.TblPhieuThu PhieuThu { get; set; }
    public string? MaKhuyenMai { get; set; }
    public double KhuyenMaiPhanTram { get; set; }
    public string? LoaiGiamGia { get; set; }
    public double GiaTriGiamGia { get; set; }
    public double TienGiamThuCong { get; set; }
    public double TongTienTamTinh { get; set; }
    public double TongTienGiam { get; set; }
    public double TongThanhToan { get; set; }
}
