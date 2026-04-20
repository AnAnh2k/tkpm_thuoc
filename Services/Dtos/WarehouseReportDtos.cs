namespace CNPM.Services.Dtos;

public class WarehouseItemInputDto
{
    public string FkSMaSp { get; set; } = string.Empty;
    public int ISl { get; set; }
    public string? SGhiChu { get; set; }
    public string? SLyDo { get; set; }
}

public class CreatePhieuYeuCauDto
{
    public string FkSMaNguoiLap { get; set; } = string.Empty;
    public List<WarehouseItemInputDto> Items { get; set; } = [];
}

public class CreatePhieuNhapDto
{
    public string FkSMaPgh { get; set; } = string.Empty;
    public string FkSMaNguoiLap { get; set; } = string.Empty;
    public List<WarehouseItemInputDto> Items { get; set; } = [];
}

public class CreateBienBanKiemKeDto
{
    public string FkSMaNguoiLap { get; set; } = string.Empty;
    public string? SDiaDiemKiem { get; set; }
    public DateTime DTgBatDau { get; set; }
    public DateTime DTgKetThuc { get; set; }
    public List<WarehouseItemInputDto> Items { get; set; } = [];
    public List<string> ThanhVienIds { get; set; } = [];
    public bool DieuChinhTonKho { get; set; }
}

public class CreateBienBanHuyDto
{
    public string FkSMaNguoiLap { get; set; } = string.Empty;
    public string? SDiaDiemHuy { get; set; }
    public DateTime DTgBatDau { get; set; }
    public DateTime DTgKetThuc { get; set; }
    public string? SPhuongThucHuy { get; set; }
    public List<WarehouseItemInputDto> Items { get; set; } = [];
    public List<string> ThanhVienIds { get; set; } = [];
}

public class ThuChiTongHopDto
{
    public string MaBaoCao { get; set; } = string.Empty;
    public DateTime DTgBatDau { get; set; }
    public DateTime DTgKetThuc { get; set; }
    public double TongThu { get; set; }
    public double TongChi { get; set; }
    public double LoiNhuan { get; set; }
    public List<ThuChiTheoSanPhamDto> ChiTietTheoSanPham { get; set; } = [];
}

public class ThuChiTheoSanPhamDto
{
    public string MaSp { get; set; } = string.Empty;
    public string? TenSp { get; set; }
    public int SoLuong { get; set; }
    public double ThanhTien { get; set; }
}

public class PhieuGiaoHangItemDto
{
    public string MaSp { get; set; } = string.Empty;
    public string? TenSp { get; set; }
    public int SoLuongGiao { get; set; }
}

public class TonKhoSnapshotDto
{
    public string MaSp { get; set; } = string.Empty;
    public string? TenSp { get; set; }
    public int SoLuongHeThong { get; set; }
}
