using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CNPM.Models
{
    [Table("tbl_NhatKyHeThong")]
    public class NhatKyHeThong
    {
        [Key]
        [StringLength(50)]
        public string ma_log { get; set; }

        [StringLength(20)]
        public string? ma_de_xuat { get; set; }

        [StringLength(20)]
        public string? ma_nv { get; set; }

        public DateTime thoi_gian { get; set; }

        public string? hanh_dong { get; set; }

        public string? chi_tiet { get; set; }
    }
}
