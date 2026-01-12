using Microsoft.EntityFrameworkCore;
using ThuHocPhi.Application.DTOs.BaoCao;
using ThuHocPhi.Application.DTOs.CongNo;
using ThuHocPhi.Application.DTOs.ThanhToan;
using ThuHocPhi.Domain.Entities;

namespace ThuHocPhi.Infrastructure.Data;

public sealed class ThuHocPhiDbContext : DbContext
{
    public ThuHocPhiDbContext(DbContextOptions<ThuHocPhiDbContext> options) : base(options)
    {
    }

    public DbSet<NguoiDung> NguoiDung => Set<NguoiDung>();
    public DbSet<VaiTro> VaiTro => Set<VaiTro>();
    public DbSet<NguoiDungVaiTro> NguoiDungVaiTro => Set<NguoiDungVaiTro>();
    public DbSet<Quyen> Quyen => Set<Quyen>();
    public DbSet<VaiTroQuyen> VaiTroQuyen => Set<VaiTroQuyen>();
    public DbSet<NhatKyHeThong> NhatKyHeThong => Set<NhatKyHeThong>();
    public DbSet<LoaiPhi> LoaiPhi => Set<LoaiPhi>();
    public DbSet<BieuPhi> BieuPhi => Set<BieuPhi>();
    public DbSet<HocKy> HocKy => Set<HocKy>();
    public DbSet<HocPhan> HocPhan => Set<HocPhan>();
    public DbSet<DangKyHocPhan> DangKyHocPhan => Set<DangKyHocPhan>();
    public DbSet<SinhVien> SinhVien => Set<SinhVien>();
    public DbSet<SinhVienAnh> SinhVienAnh => Set<SinhVienAnh>();
    public DbSet<CongNo> CongNo => Set<CongNo>();
    public DbSet<CongNoChiTiet> CongNoChiTiet => Set<CongNoChiTiet>();
    public DbSet<PhuongThucThanhToan> PhuongThucThanhToan => Set<PhuongThucThanhToan>();
    public DbSet<GiaoDich> GiaoDich => Set<GiaoDich>();
    public DbSet<BienLai> BienLai => Set<BienLai>();
    public DbSet<ChinhSachMienGiam> ChinhSachMienGiam => Set<ChinhSachMienGiam>();
    public DbSet<SinhVienMienGiam> SinhVienMienGiam => Set<SinhVienMienGiam>();
    public DbSet<HocBong> HocBong => Set<HocBong>();
    public DbSet<SinhVienHocBong> SinhVienHocBong => Set<SinhVienHocBong>();
    public DbSet<ThongBaoHocPhi> ThongBaoHocPhi => Set<ThongBaoHocPhi>();
    public DbSet<ThongBaoHocPhiSinhVien> ThongBaoHocPhiSinhVien => Set<ThongBaoHocPhiSinhVien>();
    public DbSet<DoiSoatGiaoDich> DoiSoatGiaoDich => Set<DoiSoatGiaoDich>();
    public DbSet<DoiSoatGiaoDichChiTiet> DoiSoatGiaoDichChiTiet => Set<DoiSoatGiaoDichChiTiet>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<NguoiDung>(entity =>
        {
            entity.ToTable("NguoiDung");
            entity.HasKey(x => x.MaNguoiDung);
            entity.Property(x => x.TenDangNhap).HasMaxLength(50).IsRequired();
            entity.Property(x => x.MatKhauHash).HasMaxLength(255).IsRequired();
            entity.Property(x => x.HoTen).HasMaxLength(255).IsRequired();
            entity.Property(x => x.Email).HasMaxLength(100);
            entity.Property(x => x.SoDienThoai).HasMaxLength(15);
            entity.Property(x => x.TrangThai).HasDefaultValue(true);
            entity.Property(x => x.NgayTao).HasColumnType("datetime2");
        });

        modelBuilder.Entity<VaiTro>(entity =>
        {
            entity.ToTable("VaiTro");
            entity.HasKey(x => x.MaVaiTro);
            entity.Property(x => x.TenVaiTro).HasMaxLength(100).IsRequired();
            entity.Property(x => x.MoTa).HasMaxLength(500);
            entity.Property(x => x.NgayTao).HasColumnType("datetime2");
            entity.Property(x => x.TrangThai).HasDefaultValue(true);
        });

        modelBuilder.Entity<NguoiDungVaiTro>(entity =>
        {
            entity.ToTable("NguoiDung_VaiTro");
            entity.HasKey(x => new { x.MaNguoiDung, x.MaVaiTro });
            entity.HasOne(x => x.NguoiDung)
                .WithMany(x => x.NguoiDungVaiTro)
                .HasForeignKey(x => x.MaNguoiDung);
            entity.HasOne(x => x.VaiTro)
                .WithMany(x => x.NguoiDungVaiTro)
                .HasForeignKey(x => x.MaVaiTro);
        });

        modelBuilder.Entity<Quyen>(entity =>
        {
            entity.ToTable("Quyen");
            entity.HasKey(x => x.MaQuyen);
            entity.Property(x => x.TenQuyen).HasMaxLength(100).IsRequired();
            entity.Property(x => x.MoTa).HasMaxLength(500);
            entity.Property(x => x.Module).HasMaxLength(50);
        });

        modelBuilder.Entity<VaiTroQuyen>(entity =>
        {
            entity.ToTable("VaiTro_Quyen");
            entity.HasKey(x => new { x.MaVaiTro, x.MaQuyen });
            entity.HasOne(x => x.VaiTro)
                .WithMany(x => x.VaiTroQuyen)
                .HasForeignKey(x => x.MaVaiTro);
            entity.HasOne(x => x.Quyen)
                .WithMany(x => x.VaiTroQuyen)
                .HasForeignKey(x => x.MaQuyen);
        });

        modelBuilder.Entity<NhatKyHeThong>(entity =>
        {
            entity.ToTable("NhatKyHeThong");
            entity.HasKey(x => x.Id);
            entity.Property(x => x.HanhDong).HasMaxLength(255).IsRequired();
            entity.Property(x => x.Module).HasMaxLength(50);
            entity.Property(x => x.DiaChiIp).HasColumnName("DiaChi_IP").HasMaxLength(50);
            entity.Property(x => x.ThoiGian).HasColumnType("datetime2");
            entity.HasOne(x => x.NguoiDung)
                .WithMany()
                .HasForeignKey(x => x.MaNguoiDung);
        });

        modelBuilder.Entity<LoaiPhi>(entity =>
        {
            entity.ToTable("LoaiPhi");
            entity.HasKey(x => x.MaLoaiPhi);
            entity.Property(x => x.MaLoaiPhi).HasMaxLength(20);
            entity.Property(x => x.TenLoaiPhi).HasMaxLength(255).IsRequired();
            entity.Property(x => x.DonViTinh).HasMaxLength(50);
            entity.Property(x => x.MoTa).HasMaxLength(500);
            entity.Property(x => x.TrangThai).HasDefaultValue(true);
        });

        modelBuilder.Entity<BieuPhi>(entity =>
        {
            entity.ToTable("BieuPhi");
            entity.HasKey(x => x.MaBieuPhi);
            entity.Property(x => x.MaLoaiPhi).HasMaxLength(20).IsRequired();
            entity.Property(x => x.MaHocKy).HasMaxLength(20).IsRequired();
            entity.Property(x => x.HeDaoTao).HasMaxLength(50);
            entity.Property(x => x.LoaiHinhDaoTao).HasMaxLength(50);
            entity.Property(x => x.DonGia).HasColumnType("decimal(18,2)");
            entity.Property(x => x.GhiChu).HasMaxLength(500);
            entity.Property(x => x.TrangThai).HasDefaultValue(true);
        });

        modelBuilder.Entity<HocKy>(entity =>
        {
            entity.ToTable("HocKy");
            entity.HasKey(x => x.MaHocKy);
            entity.Property(x => x.MaHocKy).HasMaxLength(20);
            entity.Property(x => x.TenHocKy).HasMaxLength(100).IsRequired();
            entity.Property(x => x.NamHoc).HasMaxLength(20);
            entity.Property(x => x.TrangThai).HasMaxLength(50);
        });

        modelBuilder.Entity<HocPhan>(entity =>
        {
            entity.ToTable("HocPhan");
            entity.HasKey(x => x.MaHocPhan);
            entity.Property(x => x.MaHocPhan).HasMaxLength(20);
            entity.Property(x => x.TenHocPhan).HasMaxLength(255).IsRequired();
            entity.Property(x => x.MaKhoa).HasMaxLength(20).IsRequired();
            entity.Property(x => x.LoaiHocPhan).HasMaxLength(50);
            entity.Property(x => x.MoTa).HasMaxLength(1000);
            entity.Property(x => x.TrangThai).HasDefaultValue(true);
        });

        modelBuilder.Entity<DangKyHocPhan>(entity =>
        {
            entity.ToTable("DangKyHocPhan");
            entity.HasKey(x => x.MaDangKy);
            entity.Property(x => x.MaSV).HasMaxLength(20).IsRequired();
            entity.Property(x => x.MaHocPhan).HasMaxLength(20).IsRequired();
            entity.Property(x => x.MaHocKy).HasMaxLength(20).IsRequired();
            entity.Property(x => x.NgayDangKy).HasColumnType("datetime2");
            entity.Property(x => x.TrangThai).HasMaxLength(50);
        });

        modelBuilder.Entity<SinhVien>(entity =>
        {
            entity.ToTable("SinhVien");
            entity.HasKey(x => x.MaSV);
            entity.Property(x => x.MaSV).HasMaxLength(20);
            entity.Property(x => x.HoTen).HasMaxLength(255).IsRequired();
            entity.Property(x => x.GioiTinh).HasMaxLength(10);
            entity.Property(x => x.CCCD).HasMaxLength(20);
            entity.Property(x => x.DiaChi).HasMaxLength(500);
            entity.Property(x => x.Email).HasMaxLength(100);
            entity.Property(x => x.SoDienThoai).HasMaxLength(15);
            entity.Property(x => x.MaLopCN).HasMaxLength(30);
            entity.Property(x => x.MaKhoaHoc).HasMaxLength(20);
            entity.Property(x => x.HeDaoTao).HasMaxLength(50);
            entity.Property(x => x.LoaiHinhDaoTao).HasMaxLength(50);
            entity.Property(x => x.TrangThai).HasMaxLength(50);
        });

        modelBuilder.Entity<SinhVienAnh>(entity =>
        {
            entity.ToTable("SinhVienAnh");
            entity.HasKey(x => x.MaAnh);
            entity.Property(x => x.MaSV).HasMaxLength(20).IsRequired();
            entity.Property(x => x.LoaiAnh).HasMaxLength(50).IsRequired();
            entity.Property(x => x.DuongDanAnh).HasMaxLength(500).IsRequired();
            entity.Property(x => x.NgayTao).HasColumnType("datetime2");
        });

        modelBuilder.Entity<CongNo>(entity =>
        {
            entity.ToTable("CongNo");
            entity.HasKey(x => x.MaCongNo);
            entity.Property(x => x.MaSV).HasMaxLength(20).IsRequired();
            entity.Property(x => x.MaHocKy).HasMaxLength(20).IsRequired();
            entity.Property(x => x.TongPhaiNop).HasColumnType("decimal(18,2)");
            entity.Property(x => x.TienMienGiam).HasColumnType("decimal(18,2)");
            entity.Property(x => x.TongDaNop).HasColumnType("decimal(18,2)");
            entity.Property(x => x.ConNo)
                .HasColumnType("decimal(18,2)")
                .HasComputedColumnSql("([TongPhaiNop]-[TienMienGiam]-[TongDaNop])", stored: true);
        });

        modelBuilder.Entity<CongNoChiTiet>(entity =>
        {
            entity.ToTable("CongNoChiTiet");
            entity.HasKey(x => x.MaCongNoCT);
            entity.Property(x => x.LoaiDong).HasMaxLength(30).IsRequired();
            entity.Property(x => x.MoTa).HasMaxLength(300).IsRequired();
            entity.Property(x => x.SoLuong).HasColumnType("decimal(18,2)");
            entity.Property(x => x.DonGia).HasColumnType("decimal(18,2)");
            entity.Property(x => x.ThanhTien)
                .HasColumnType("decimal(18,2)")
                .HasComputedColumnSql("([SoLuong]*[DonGia])", stored: true);
            entity.Property(x => x.MienGiam).HasColumnType("decimal(18,2)");
            entity.Property(x => x.PhaiThu)
                .HasColumnType("decimal(18,2)")
                .HasComputedColumnSql("(([SoLuong]*[DonGia])-[MienGiam])", stored: true);
        });

        modelBuilder.Entity<PhuongThucThanhToan>(entity =>
        {
            entity.ToTable("PhuongThucThanhToan");
            entity.HasKey(x => x.MaPhuongThuc);
            entity.Property(x => x.MaPhuongThuc).HasMaxLength(20);
            entity.Property(x => x.TenPhuongThuc).HasMaxLength(100).IsRequired();
            entity.Property(x => x.MoTa).HasMaxLength(500);
            entity.Property(x => x.TrangThai).HasDefaultValue(true);
        });

        modelBuilder.Entity<GiaoDich>(entity =>
        {
            entity.ToTable("GiaoDich");
            entity.HasKey(x => x.MaGiaoDich);
            entity.Property(x => x.MaGiaoDich).HasMaxLength(50);
            entity.Property(x => x.MaPhuongThuc).HasMaxLength(20).IsRequired();
            entity.Property(x => x.MaGiaoDichNganHang).HasMaxLength(100);
            entity.Property(x => x.NoiDung).HasMaxLength(500);
            entity.Property(x => x.NguoiNop).HasMaxLength(255);
            entity.Property(x => x.GhiChu).HasMaxLength(500);
            entity.Property(x => x.SoTien).HasColumnType("decimal(18,2)");
        });

        modelBuilder.Entity<BienLai>(entity =>
        {
            entity.ToTable("BienLai");
            entity.HasKey(x => x.MaBienLai);
            entity.Property(x => x.MaGiaoDich).HasMaxLength(50).IsRequired();
            entity.Property(x => x.SoBienLai).HasMaxLength(50).IsRequired();
            entity.Property(x => x.FilePath).HasMaxLength(500);
            entity.Property(x => x.TrangThai).HasDefaultValue(true);
        });

        modelBuilder.Entity<ChinhSachMienGiam>(entity =>
        {
            entity.ToTable("ChinhSachMienGiam");
            entity.HasKey(x => x.MaChinhSach);
            entity.Property(x => x.MaChinhSach).HasMaxLength(20);
            entity.Property(x => x.TenChinhSach).HasMaxLength(255).IsRequired();
            entity.Property(x => x.DoiTuongApDung).HasMaxLength(500);
            entity.Property(x => x.LoaiMienGiam).HasMaxLength(50).IsRequired();
            entity.Property(x => x.GiaTriMienGiam).HasColumnType("decimal(18,2)");
            entity.Property(x => x.TrangThai).HasDefaultValue(true);
        });

        modelBuilder.Entity<SinhVienMienGiam>(entity =>
        {
            entity.ToTable("SinhVien_MienGiam");
            entity.HasKey(x => x.Id);
            entity.Property(x => x.MaSV).HasMaxLength(20).IsRequired();
            entity.Property(x => x.MaChinhSach).HasMaxLength(20).IsRequired();
            entity.Property(x => x.MaHocKy).HasMaxLength(20).IsRequired();
            entity.Property(x => x.GhiChu).HasMaxLength(500);
            entity.Property(x => x.TrangThai).HasDefaultValue(true);
        });

        modelBuilder.Entity<HocBong>(entity =>
        {
            entity.ToTable("HocBong");
            entity.HasKey(x => x.MaHocBong);
            entity.Property(x => x.MaHocBong).HasMaxLength(20);
            entity.Property(x => x.TenHocBong).HasMaxLength(255).IsRequired();
            entity.Property(x => x.LoaiGiaTri).HasMaxLength(50).IsRequired();
            entity.Property(x => x.GiaTri).HasColumnType("decimal(18,2)");
            entity.Property(x => x.NamHoc).HasMaxLength(20);
            entity.Property(x => x.MoTa).HasMaxLength(500);
            entity.Property(x => x.TrangThai).HasDefaultValue(true);
        });

        modelBuilder.Entity<SinhVienHocBong>(entity =>
        {
            entity.ToTable("SinhVien_HocBong");
            entity.HasKey(x => x.Id);
            entity.Property(x => x.MaSV).HasMaxLength(20).IsRequired();
            entity.Property(x => x.MaHocBong).HasMaxLength(20).IsRequired();
            entity.Property(x => x.MaHocKy).HasMaxLength(20).IsRequired();
            entity.Property(x => x.GhiChu).HasMaxLength(500);
            entity.Property(x => x.TrangThai).HasDefaultValue(true);
        });

        modelBuilder.Entity<ThongBaoHocPhi>(entity =>
        {
            entity.ToTable("ThongBaoHocPhi");
            entity.HasKey(x => x.MaThongBao);
            entity.Property(x => x.MaHocKy).HasMaxLength(20).IsRequired();
            entity.Property(x => x.NamHoc).HasMaxLength(20);
            entity.Property(x => x.TieuDe).HasMaxLength(255).IsRequired();
            entity.Property(x => x.DoiTuongApDung).HasMaxLength(500);
            entity.Property(x => x.TongSoTienCongNo).HasColumnType("decimal(18,2)");
            entity.Property(x => x.HinhThucGui).HasMaxLength(50);
        });

        modelBuilder.Entity<ThongBaoHocPhiSinhVien>(entity =>
        {
            entity.ToTable("ThongBaoHocPhi_SinhVien");
            entity.HasKey(x => x.Id);
            entity.Property(x => x.MaSV).HasMaxLength(20).IsRequired();
            entity.Property(x => x.GhiChu).HasMaxLength(500);
        });

        modelBuilder.Entity<DoiSoatGiaoDich>(entity =>
        {
            entity.ToTable("DoiSoatGiaoDich");
            entity.HasKey(x => x.MaDoiSoat);
            entity.Property(x => x.NguonDuLieu).HasMaxLength(100);
            entity.Property(x => x.GhiChu).HasMaxLength(500);
        });

        modelBuilder.Entity<DoiSoatGiaoDichChiTiet>(entity =>
        {
            entity.ToTable("DoiSoatGiaoDichChiTiet");
            entity.HasKey(x => x.Id);
            entity.Property(x => x.MaGiaoDichNganHang).HasMaxLength(100).IsRequired();
            entity.Property(x => x.MaGiaoDichHeThong).HasMaxLength(50);
            entity.Property(x => x.SoTienNganHang).HasColumnType("decimal(18,2)");
            entity.Property(x => x.SoTienHeThong).HasColumnType("decimal(18,2)");
            entity.Property(x => x.GhiChu).HasMaxLength(500);
        });

        modelBuilder.Entity<CongNoTinhResult>(entity =>
        {
            entity.HasNoKey();
            entity.Property(x => x.TongPhaiNop).HasPrecision(18, 2);
            entity.Property(x => x.TienMienGiam).HasPrecision(18, 2);
        });

        modelBuilder.Entity<ThanhToanResult>().HasNoKey();

        modelBuilder.Entity<BaoCaoCongNoKhoaDto>(entity =>
        {
            entity.HasNoKey();
            entity.Property(x => x.TongPhaiThu).HasPrecision(18, 2);
            entity.Property(x => x.TongMienGiam).HasPrecision(18, 2);
            entity.Property(x => x.TongDaThu).HasPrecision(18, 2);
            entity.Property(x => x.TongConNo).HasPrecision(18, 2);
        });

        modelBuilder.Entity<DoanhThuNgayDto>(entity =>
        {
            entity.HasNoKey();
            entity.Property(x => x.TongThu).HasPrecision(18, 2);
        });
    }
}
