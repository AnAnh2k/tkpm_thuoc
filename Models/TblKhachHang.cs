using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace CNPM.Models;

[Table("tbl_KhachHang")]
public partial class TblKhachHang
{
    [Key]
    [Column("PK_sMaKH")]
    [StringLength(20)]
    [Unicode(false)]
    [Required(ErrorMessage = "Mã khách hàng là bắt buộc.")]
    public string PkSMaKh { get; set; } = null!;

    [Column("sTenKH")]
    [StringLength(50)]
    [Required(ErrorMessage = "Tên khách hàng là bắt buộc.")]
    public string? STenKh { get; set; }

    [Column("sSDT")]
    [StringLength(20)]
    [Unicode(false)]
    [Required(ErrorMessage = "Số điện thoại là bắt buộc.")]
    [RegularExpression(@"^0\d{9}$", ErrorMessage = "Số điện thoại phải có 10 số và bắt đầu bằng 0.")]
    public string? SSdt { get; set; }

    [Column("sDiaChi")]
    [StringLength(100)]
    public string? SDiaChi { get; set; }

    [Column("dNgaySinh")]
    public DateTime? DNgaySinh { get; set; }

    [InverseProperty("FkSMaKhNavigation")]
    public virtual ICollection<TblPhieuThu> TblPhieuThus { get; set; } = new List<TblPhieuThu>();
}