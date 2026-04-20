using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace CNPM.Models;

public partial class PharmacyDbContext : DbContext
{
    public PharmacyDbContext()
    {
    }

    public PharmacyDbContext(DbContextOptions<PharmacyDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<TblBaoCaoThuChi> TblBaoCaoThuChis { get; set; }

    public virtual DbSet<TblBienBanHuy> TblBienBanHuys { get; set; }

    public virtual DbSet<TblBienBanKiemKe> TblBienBanKiemKes { get; set; }

    public virtual DbSet<TblChucVu> TblChucVus { get; set; }

    public virtual DbSet<TblCtbienBanHuy> TblCtbienBanHuys { get; set; }

    public virtual DbSet<TblCtbienBanKiemKe> TblCtbienBanKiemKes { get; set; }

    public virtual DbSet<TblCthopDongCungCap> TblCthopDongCungCaps { get; set; }

    public virtual DbSet<TblCtphieuDatHang> TblCtphieuDatHangs { get; set; }

    public virtual DbSet<TblCtphieuGiaoHang> TblCtphieuGiaoHangs { get; set; }

    public virtual DbSet<TblCtphieuNhapKho> TblCtphieuNhapKhos { get; set; }

    public virtual DbSet<TblCtphieuThu> TblCtphieuThus { get; set; }

    public virtual DbSet<TblCtphieuXuatKho> TblCtphieuXuatKhos { get; set; }

    public virtual DbSet<TblCtphieuYeuCau> TblCtphieuYeuCaus { get; set; }

    public virtual DbSet<TblHopDongCungCap> TblHopDongCungCaps { get; set; }

    public virtual DbSet<TblKhachHang> TblKhachHangs { get; set; }

    public virtual DbSet<TblLoaiSanPham> TblLoaiSanPhams { get; set; }

    public virtual DbSet<TblNhaCungCap> TblNhaCungCaps { get; set; }

    public virtual DbSet<TblNhanVien> TblNhanViens { get; set; }

    public virtual DbSet<TblPhieuChi> TblPhieuChis { get; set; }

    public virtual DbSet<TblPhieuDatHang> TblPhieuDatHangs { get; set; }

    public virtual DbSet<TblPhieuGiaoHang> TblPhieuGiaoHangs { get; set; }

    public virtual DbSet<TblPhieuNhapKho> TblPhieuNhapKhos { get; set; }

    public virtual DbSet<TblPhieuThu> TblPhieuThus { get; set; }

    public virtual DbSet<TblPhieuXuatKho> TblPhieuXuatKhos { get; set; }

    public virtual DbSet<TblPhieuYeuCau> TblPhieuYeuCaus { get; set; }

    public virtual DbSet<TblQuyen> TblQuyens { get; set; }

    public virtual DbSet<TblSanPham> TblSanPhams { get; set; }

    public virtual DbSet<TblTaiKhoan> TblTaiKhoans { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<TblBaoCaoThuChi>(entity =>
        {
            entity.HasKey(e => e.PkSMaBc).HasName("PK__tbl_BaoC__DF6E59A563C250A1");

            entity.HasOne(d => d.FkSMaNvNavigation).WithMany(p => p.TblBaoCaoThuChis).HasConstraintName("FK__tbl_BaoCa__FK_sM__1BC821DD");

            entity.HasMany(d => d.PkFkSMaSps).WithMany(p => p.PkFkSMaBcs)
                .UsingEntity<Dictionary<string, object>>(
                    "TblCtbaoCaoThuChi",
                    r => r.HasOne<TblSanPham>().WithMany()
                        .HasForeignKey("PkFkSMaSp")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK__tbl_CTBao__PK_FK__1F98B2C1"),
                    l => l.HasOne<TblBaoCaoThuChi>().WithMany()
                        .HasForeignKey("PkFkSMaBc")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK__tbl_CTBao__PK_FK__1EA48E88"),
                    j =>
                    {
                        j.HasKey("PkFkSMaBc", "PkFkSMaSp").HasName("PK__tbl_CTBa__091FC2A60E2AC47B");
                        j.ToTable("tbl_CTBaoCaoThuChi");
                        j.IndexerProperty<string>("PkFkSMaBc")
                            .HasMaxLength(20)
                            .IsUnicode(false)
                            .HasColumnName("PK_FK_sMaBC");
                        j.IndexerProperty<string>("PkFkSMaSp")
                            .HasMaxLength(20)
                            .IsUnicode(false)
                            .HasColumnName("PK_FK_sMaSP");
                    });
        });



        modelBuilder.Entity<TblBienBanHuy>(entity =>
        {
            entity.HasKey(e => e.PkSMaBbh).HasName("PK__tbl_Bien__471DAAF591300568");

            entity.HasOne(d => d.FkSMaNvNavigation).WithMany(p => p.TblBienBanHuys).HasConstraintName("FK__tbl_BienB__FK_sM__06CD04F7");

            entity.HasMany(d => d.PkFkSMaNguoiHuys).WithMany(p => p.PkFkMaBbhs)
                .UsingEntity<Dictionary<string, object>>(
                    "TblThanhVienHuy",
                    r => r.HasOne<TblNhanVien>().WithMany()
                        .HasForeignKey("PkFkSMaNguoiHuy")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK__tbl_Thanh__PK_FK__0E6E26BF"),
                    l => l.HasOne<TblBienBanHuy>().WithMany()
                        .HasForeignKey("PkFkMaBbh")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK__tbl_Thanh__PK_FK__0D7A0286"),
                    j =>
                    {
                        j.HasKey("PkFkMaBbh", "PkFkSMaNguoiHuy").HasName("PK__tbl_Than__8ADFC24EADBF51EA");
                        j.ToTable("tbl_ThanhVienHuy");
                        j.IndexerProperty<string>("PkFkMaBbh")
                            .HasMaxLength(20)
                            .IsUnicode(false)
                            .HasColumnName("PK_FK_MaBBH");
                        j.IndexerProperty<string>("PkFkSMaNguoiHuy")
                            .HasMaxLength(20)
                            .IsUnicode(false)
                            .HasColumnName("PK_FK_sMaNguoiHuy");
                    });
        });

        modelBuilder.Entity<TblBienBanKiemKe>(entity =>
        {
            entity.HasKey(e => e.PkSMaBbk).HasName("PK__tbl_Bien__471DAAF09CB84A54");

            entity.HasOne(d => d.FkSMaNvNavigation).WithMany(p => p.TblBienBanKiemKes).HasConstraintName("FK__tbl_BienB__FK_sM__114A936A");

            entity.HasMany(d => d.PkFkSMaNguoiKiems).WithMany(p => p.PkFkSMaBbks)
                .UsingEntity<Dictionary<string, object>>(
                    "TblThanhVienKiemKe",
                    r => r.HasOne<TblNhanVien>().WithMany()
                        .HasForeignKey("PkFkSMaNguoiKiem")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK__tbl_Thanh__PK_FK__18EBB532"),
                    l => l.HasOne<TblBienBanKiemKe>().WithMany()
                        .HasForeignKey("PkFkSMaBbk")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK__tbl_Thanh__PK_FK__17F790F9"),
                    j =>
                    {
                        j.HasKey("PkFkSMaBbk", "PkFkSMaNguoiKiem").HasName("PK__tbl_Than__B30ABB7373E087D0");
                        j.ToTable("tbl_ThanhVienKiemKe");
                        j.IndexerProperty<string>("PkFkSMaBbk")
                            .HasMaxLength(20)
                            .IsUnicode(false)
                            .HasColumnName("PK_FK_sMaBBK");
                        j.IndexerProperty<string>("PkFkSMaNguoiKiem")
                            .HasMaxLength(20)
                            .IsUnicode(false)
                            .HasColumnName("PK_FK_sMaNguoiKiem");
                    });
        });

        modelBuilder.Entity<TblChucVu>(entity =>
        {
            entity.HasKey(e => e.PkSMaCv).HasName("PK__tbl_Chuc__DF6FC22C02D5DE3B");
        });

        modelBuilder.Entity<TblCtbienBanHuy>(entity =>
        {
            entity.HasKey(e => new { e.PkFkSMaBbh, e.PkFkSMaSp }).HasName("PK__tbl_CTBi__11043D0062DDA535");

            entity.HasOne(d => d.PkFkSMaBbhNavigation).WithMany(p => p.TblCtbienBanHuys)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__tbl_CTBie__PK_FK__09A971A2");

            entity.HasOne(d => d.PkFkSMaSpNavigation).WithMany(p => p.TblCtbienBanHuys)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__tbl_CTBie__PK_FK__0A9D95DB");
        });

        modelBuilder.Entity<TblCtbienBanKiemKe>(entity =>
        {
            entity.HasKey(e => new { e.PkFkSMaBbk, e.PkFkSMaSp }).HasName("PK__tbl_CTBi__11043D051975011B");

            entity.HasOne(d => d.PkFkSMaBbkNavigation).WithMany(p => p.TblCtbienBanKiemKes)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__tbl_CTBie__PK_FK__14270015");

            entity.HasOne(d => d.PkFkSMaSpNavigation).WithMany(p => p.TblCtbienBanKiemKes)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__tbl_CTBie__PK_FK__151B244E");
        });

        modelBuilder.Entity<TblCthopDongCungCap>(entity =>
        {
            entity.HasKey(e => new { e.PkFkSMaHd, e.PkFkSMaSp }).HasName("PK__tbl_CTHo__091C136FB51E64E6");

            entity.HasOne(d => d.PkFkSMaHdNavigation).WithMany(p => p.TblCthopDongCungCaps)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__tbl_CTHop__PK_FK__6D0D32F4");

            entity.HasOne(d => d.PkFkSMaSpNavigation).WithMany(p => p.TblCthopDongCungCaps)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__tbl_CTHop__PK_FK__6E01572D");
        });

        modelBuilder.Entity<TblCtphieuDatHang>(entity =>
        {
            entity.HasKey(e => new { e.PkFkSMaPdh, e.PkFkSMaSp }).HasName("PK__tbl_CTPh__0520BA68D00B7CC9");

            entity.HasOne(d => d.PkFkSMaPdhNavigation).WithMany(p => p.TblCtphieuDatHangs)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__tbl_CTPhi__PK_FK__5629CD9C");

            entity.HasOne(d => d.PkFkSMaSpNavigation).WithMany(p => p.TblCtphieuDatHangs)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__tbl_CTPhi__PK_FK__571DF1D5");
        });

        modelBuilder.Entity<TblCtphieuGiaoHang>(entity =>
        {
            entity.HasKey(e => new { e.PkFkSMaPgh, e.PkFkSMaSp }).HasName("PK__tbl_CTPh__0520918C69F32167");

            entity.HasOne(d => d.PkFkSMaPghNavigation).WithMany(p => p.TblCtphieuGiaoHangs)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__tbl_CTPhi__PK_FK__5DCAEF64");

            entity.HasOne(d => d.PkFkSMaSpNavigation).WithMany(p => p.TblCtphieuGiaoHangs)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__tbl_CTPhi__PK_FK__5EBF139D");
        });

        modelBuilder.Entity<TblCtphieuNhapKho>(entity =>
        {
            entity.HasKey(e => new { e.PkFkSMaPn, e.PkFkSMaSp }).HasName("PK__tbl_CTPh__091D5C4B78BE2206");

            entity.HasOne(d => d.PkFkSMaPnNavigation).WithMany(p => p.TblCtphieuNhapKhos)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__tbl_CTPhi__PK_FK__656C112C");

            entity.HasOne(d => d.PkFkSMaSpNavigation).WithMany(p => p.TblCtphieuNhapKhos)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__tbl_CTPhi__PK_FK__66603565");
        });

        modelBuilder.Entity<TblCtphieuThu>(entity =>
        {
            entity.HasKey(e => new { e.PkFkSMaPt, e.PkFkSMaSp }).HasName("PK__tbl_CTPh__091D5C3171E973FD");

            entity.HasOne(d => d.PkFkSMaPtNavigation).WithMany(p => p.TblCtphieuThus)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__tbl_CTPhi__PK_FK__02FC7413");

            entity.HasOne(d => d.PkFkSMaSpNavigation).WithMany(p => p.TblCtphieuThus)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__tbl_CTPhi__PK_FK__03F0984C");
        });

        modelBuilder.Entity<TblCtphieuXuatKho>(entity =>
        {
            entity.HasKey(e => new { e.PkFkSMaPx, e.PkFkSMaSp }).HasName("PK__tbl_CTPh__091D5C3573D4441B");

            entity.HasOne(d => d.PkFkSMaPxNavigation).WithMany(p => p.TblCtphieuXuatKhos)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__tbl_CTPhi__PK_FK__7B5B524B");

            entity.HasOne(d => d.PkFkSMaSpNavigation).WithMany(p => p.TblCtphieuXuatKhos)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__tbl_CTPhi__PK_FK__7C4F7684");
        });

        modelBuilder.Entity<TblCtphieuYeuCau>(entity =>
        {
            entity.HasKey(e => new { e.PkFkSMaPyc, e.PkFkSMaSp }).HasName("PK__tbl_CTPh__055C17A93C3EE7F8");

            entity.HasOne(d => d.PkFkSMaPycNavigation).WithMany(p => p.TblCtphieuYeuCaus)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__tbl_CTPhi__PK_FK__4E88ABD4");

            entity.HasOne(d => d.PkFkSMaSpNavigation).WithMany(p => p.TblCtphieuYeuCaus)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__tbl_CTPhi__PK_FK__4F7CD00D");
        });

        modelBuilder.Entity<TblHopDongCungCap>(entity =>
        {
            entity.HasKey(e => e.PkSMaHd).HasName("PK__tbl_HopD__DF6FAADA3738AE65");

            entity.HasOne(d => d.FkSMaNccNavigation).WithMany(p => p.TblHopDongCungCaps).HasConstraintName("FK__tbl_HopDo__FK_sM__693CA210");

            entity.HasOne(d => d.FkSMaNvNavigation).WithMany(p => p.TblHopDongCungCaps).HasConstraintName("FK__tbl_HopDo__FK_sM__6A30C649");
        });

        modelBuilder.Entity<TblKhachHang>(entity =>
        {
            entity.HasKey(e => e.PkSMaKh).HasName("PK__tbl_Khac__DF6E0330D00AA42B");
        });

        modelBuilder.Entity<TblLoaiSanPham>(entity =>
        {
            entity.HasKey(e => e.PkSMaLoai).HasName("PK__tbl_Loai__5D93B0465D2093CB");
        });

        modelBuilder.Entity<TblNhaCungCap>(entity =>
        {
            entity.HasKey(e => e.PkSMaNcc).HasName("PK__tbl_NhaC__7ADBE2FEB607D37E");
        });

        modelBuilder.Entity<TblNhanVien>(entity =>
        {
            entity.HasKey(e => e.PkSMaNv).HasName("PK__tbl_Nhan__DF6FFB014FB56926");

            entity.HasOne(d => d.FkSMaCvNavigation).WithMany(p => p.TblNhanViens).HasConstraintName("FK__tbl_NhanV__FK_sM__44FF419A");

            entity.HasOne(d => d.FkSMaTkNavigation).WithMany(p => p.TblNhanViens).HasConstraintName("FK__tbl_NhanV__FK_sM__440B1D61");
        });

        modelBuilder.Entity<TblPhieuChi>(entity =>
        {
            entity.HasKey(e => e.PkSMaPchi).HasName("PK__tbl_Phie__5351DBF3ABB44345");

            entity.HasOne(d => d.FkSMaNvNavigation).WithMany(p => p.TblPhieuChis).HasConstraintName("FK__tbl_Phieu__FK_sM__71D1E811");

            entity.HasOne(d => d.FkSMaPnNavigation).WithMany(p => p.TblPhieuChis).HasConstraintName("FK__tbl_Phieu__FK_sM__70DDC3D8");

            entity.HasMany(d => d.PkFkSMaSps).WithMany(p => p.PkFkSMaPchis)
                .UsingEntity<Dictionary<string, object>>(
                    "TblCtphieuChi",
                    r => r.HasOne<TblSanPham>().WithMany()
                        .HasForeignKey("PkFkSMaSp")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK__tbl_CTPhi__PK_FK__75A278F5"),
                    l => l.HasOne<TblPhieuChi>().WithMany()
                        .HasForeignKey("PkFkSMaPchi")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK__tbl_CTPhi__PK_FK__74AE54BC"),
                    j =>
                    {
                        j.HasKey("PkFkSMaPchi", "PkFkSMaSp").HasName("PK__tbl_CTPh__B22F9ECD5305DE24");
                        j.ToTable("tbl_CTPhieuChi");
                        j.IndexerProperty<string>("PkFkSMaPchi")
                            .HasMaxLength(20)
                            .IsUnicode(false)
                            .HasColumnName("PK_FK_sMaPChi");
                        j.IndexerProperty<string>("PkFkSMaSp")
                            .HasMaxLength(20)
                            .IsUnicode(false)
                            .HasColumnName("PK_FK_sMaSP");
                    });
        });

        modelBuilder.Entity<TblPhieuDatHang>(entity =>
        {
            entity.HasKey(e => e.PkSMaPdh).HasName("PK__tbl_Phie__7A58168C837175E1");

            entity.HasOne(d => d.FkSMaNvNavigation).WithMany(p => p.TblPhieuDatHangs).HasConstraintName("FK__tbl_Phieu__FK_sM__534D60F1");

            entity.HasOne(d => d.FkSMaPycNavigation).WithMany(p => p.TblPhieuDatHangs).HasConstraintName("FK__tbl_Phieu__FK_sM__52593CB8");
        });

        modelBuilder.Entity<TblPhieuGiaoHang>(entity =>
        {
            entity.HasKey(e => e.PkSMaPgh).HasName("PK__tbl_Phie__7A57CE2A2BC91673");

            entity.HasOne(d => d.FkSMaNvNavigation).WithMany(p => p.TblPhieuGiaoHangs).HasConstraintName("FK__tbl_Phieu__FK_sM__5AEE82B9");

            entity.HasOne(d => d.FkSMaPdhNavigation).WithMany(p => p.TblPhieuGiaoHangs).HasConstraintName("FK__tbl_Phieu__FK_sM__59FA5E80");
        });

        modelBuilder.Entity<TblPhieuNhapKho>(entity =>
        {
            entity.HasKey(e => e.PkSMaPn).HasName("PK__tbl_Phie__DF6FEBCEF653D342");

            entity.HasOne(d => d.FkSMaNvNavigation).WithMany(p => p.TblPhieuNhapKhos).HasConstraintName("FK__tbl_Phieu__FK_sM__628FA481");

            entity.HasOne(d => d.FkSMaPghNavigation).WithMany(p => p.TblPhieuNhapKhos).HasConstraintName("FK__tbl_Phieu__FK_sM__619B8048");
        });

        modelBuilder.Entity<TblPhieuThu>(entity =>
        {
            entity.HasKey(e => e.PkSMaPt).HasName("PK__tbl_Phie__DF6FEBC0E46C0696");

            entity.HasOne(d => d.FkSMaKhNavigation).WithMany(p => p.TblPhieuThus).HasConstraintName("FK__tbl_Phieu__FK_sM__00200768");

            entity.HasOne(d => d.FkSMaNvNavigation).WithMany(p => p.TblPhieuThus).HasConstraintName("FK__tbl_Phieu__FK_sM__7F2BE32F");
        });

        modelBuilder.Entity<TblPhieuXuatKho>(entity =>
        {
            entity.HasKey(e => e.PkSMaPx).HasName("PK__tbl_Phie__DF6FEBC4DF2AB2FE");

            entity.HasOne(d => d.FkSMaNvNavigation).WithMany(p => p.TblPhieuXuatKhos).HasConstraintName("FK__tbl_Phieu__FK_sM__787EE5A0");
        });

        modelBuilder.Entity<TblPhieuYeuCau>(entity =>
        {
            entity.HasKey(e => e.PkSMaPyc).HasName("PK__tbl_Phie__7A587871469C7019");

            entity.HasOne(d => d.FkSMaNvNavigation).WithMany(p => p.TblPhieuYeuCaus).HasConstraintName("FK__tbl_Phieu__FK_sM__4BAC3F29");
        });

        modelBuilder.Entity<TblQuyen>(entity =>
        {
            entity.HasKey(e => e.PkSMaQuyen).HasName("PK__tbl_Quye__2C0839D82775F366");
        });

        modelBuilder.Entity<TblSanPham>(entity =>
        {
            entity.HasKey(e => e.PkSMaSp).HasName("PK__tbl_SanP__DF6F443676EAB98E");

            entity.HasOne(d => d.FkSMaLoaiNavigation).WithMany(p => p.TblSanPhams).HasConstraintName("FK__tbl_SanPh__FK_sM__47DBAE45");

            entity.HasOne(d => d.FkSMaNccNavigation).WithMany(p => p.TblSanPhams).HasConstraintName("FK__tbl_SanPh__FK_sM__48CFD27E");
        });

        modelBuilder.Entity<TblTaiKhoan>(entity =>
        {
            entity.HasKey(e => e.PkSMaTk).HasName("PK__tbl_TaiK__DF6F4C4AA6C26A13");

            entity.HasOne(d => d.FkSMaQuyenNavigation).WithMany(p => p.TblTaiKhoans).HasConstraintName("FK__tbl_TaiKh__FK_sM__412EB0B6");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
