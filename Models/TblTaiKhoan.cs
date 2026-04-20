using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace CNPM.Models;

[Table("tbl_TaiKhoan")]
public partial class TblTaiKhoan
{
    [Key]
    [Column("PK_sMaTK")]
    [StringLength(20)]
    [Unicode(false)]
    public string PkSMaTk { get; set; } = null!;

    [Column("sTenTK")]
    [StringLength(20)]
    [Unicode(false)]
    public string? STenTk { get; set; }

    [Column("sMK")]
    [StringLength(20)]
    [Unicode(false)]
    public string? SMk { get; set; }

    [Column("FK_sMaQuyen")]
    [StringLength(20)]
    [Unicode(false)]
    public string? FkSMaQuyen { get; set; }

    [ForeignKey("FkSMaQuyen")]
    [InverseProperty("TblTaiKhoans")]
    public virtual TblQuyen? FkSMaQuyenNavigation { get; set; }

    [InverseProperty("FkSMaTkNavigation")]
    public virtual ICollection<TblNhanVien> TblNhanViens { get; set; } = new List<TblNhanVien>();
}
