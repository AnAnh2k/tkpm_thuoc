using System.ComponentModel.DataAnnotations;

namespace CNPM.Models;

public class KhachHangViewModel
{
    [Required(ErrorMessage = "Mã khách hàng là bắt buộc.")]
    [StringLength(20)]
    public string PkSMaKh { get; set; } = null!;

    [Required(ErrorMessage = "Tên khách hàng là bắt buộc.")]
    [StringLength(50)]
    public string? STenKh { get; set; }

    [Required(ErrorMessage = "Số điện thoại là bắt buộc.")]
    [RegularExpression(@"^0\d{9}$", ErrorMessage = "Số điện thoại phải có 10 số và bắt đầu bằng 0.")]
    public string? SSdt { get; set; }

    [StringLength(100)]
    public string? SDiaChi { get; set; }

    public DateTime? DNgaySinh { get; set; }
}
