using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace CNPM.Models;

[Table("tbl_PhieuThu")]
public partial class TblPhieuThu
{
    [Key]
    [Column("PK_sMaPT")]
    [StringLength(20)]
    [Unicode(false)]
    public string PkSMaPt { get; set; } = null!;

    [Column("dTgLap", TypeName = "datetime")]
    public DateTime? DTgLap { get; set; }

    [Column("FK_sMaNV")]
    [StringLength(20)]
    [Unicode(false)]
    public string? FkSMaNv { get; set; }

    [Column("FK_sMaKH")]
    [StringLength(20)]
    [Unicode(false)]
    public string? FkSMaKh { get; set; }

    [Column("sHinhThucTT")]
    [StringLength(20)]
    public string? SHinhThucTt { get; set; }

    [ForeignKey("FkSMaKh")]
    [InverseProperty("TblPhieuThus")]
    public virtual TblKhachHang? FkSMaKhNavigation { get; set; }

    [ForeignKey("FkSMaNv")]
    [InverseProperty("TblPhieuThus")]
    public virtual TblNhanVien? FkSMaNvNavigation { get; set; }

    [InverseProperty("PkFkSMaPtNavigation")]
    public virtual ICollection<TblCtphieuThu> TblCtphieuThus { get; set; } = new List<TblCtphieuThu>();
}
