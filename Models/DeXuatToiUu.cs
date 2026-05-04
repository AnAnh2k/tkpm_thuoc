using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CNPM.Models
{
    [Table("tbl_DeXuatToiUu")]
    public class DeXuatToiUu
    {
        [Key]
        [StringLength(20)]
        public string ma_de_xuat { get; set; }

        public DateTime ngay_tao { get; set; }

        public DateTime tu_ngay { get; set; }

        public DateTime den_ngay { get; set; }

        public string? tom_tat { get; set; }

        public string? danh_sach_hanh_dong { get; set; }

        public string? bieu_do_xu_huong { get; set; }

        [StringLength(50)]
        public string? trang_thai { get; set; }

        public bool is_baseline { get; set; }

        [StringLength(20)]
        public string? phien_ban { get; set; }

        public DateTime? ngay_xem_lai { get; set; }

        [StringLength(50)]
        public string? quyet_dinh { get; set; }

        public string? ly_do { get; set; }

        [StringLength(20)]
        public string? ma_nv_thuc_hien { get; set; }
    }
}
