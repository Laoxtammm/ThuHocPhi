/*==============================================================*/
/* QLHP_HVKTQS - UPDATED SCHEMA v2                               */
/* - Gộp Lớp + Chuyên ngành => LOP_CHUYENNGANH                   */
/* - Thêm ảnh sinh viên                                          */
/* - Thêm bảng Khóa học                                          */
/* Date: 2026-01-10                                              */
/*==============================================================*/

---------------------------------------------------------------
-- 0) TẠO DATABASE
---------------------------------------------------------------
IF DB_ID('QLHP_HVKTQS') IS NOT NULL
BEGIN
    ALTER DATABASE QLHP_HVKTQS SET SINGLE_USER WITH ROLLBACK IMMEDIATE;
    DROP DATABASE QLHP_HVKTQS;
END
GO
CREATE DATABASE QLHP_HVKTQS;
GO
USE QLHP_HVKTQS;
GO

SET ANSI_NULLS ON;
SET QUOTED_IDENTIFIER ON;
GO

/*==============================================================*/
/* PHẦN 1: QUẢN TRỊ HỆ THỐNG & PHÂN QUYỀN                        */
/*==============================================================*/

CREATE TABLE dbo.VaiTro (
    MaVaiTro INT IDENTITY(1,1) CONSTRAINT PK_VaiTro PRIMARY KEY,
    TenVaiTro NVARCHAR(100) NOT NULL CONSTRAINT UQ_VaiTro_Ten UNIQUE,
    MoTa NVARCHAR(500) NULL,
    NgayTao DATETIME2 NOT NULL DEFAULT SYSUTCDATETIME(),
    TrangThai BIT NOT NULL DEFAULT 1
);

CREATE TABLE dbo.NguoiDung (
    MaNguoiDung INT IDENTITY(1,1) CONSTRAINT PK_NguoiDung PRIMARY KEY,
    TenDangNhap VARCHAR(50) NOT NULL CONSTRAINT UQ_NguoiDung_TenDangNhap UNIQUE,
    MatKhauHash VARCHAR(255) NOT NULL,
    HoTen NVARCHAR(255) NOT NULL,
    Email VARCHAR(100) NULL CONSTRAINT UQ_NguoiDung_Email UNIQUE,
    SoDienThoai VARCHAR(15) NULL,
    LanDangNhapCuoi DATETIME2 NULL,
    TrangThai BIT NOT NULL DEFAULT 1,
    NgayTao DATETIME2 NOT NULL DEFAULT SYSUTCDATETIME()
);

CREATE TABLE dbo.NguoiDung_VaiTro (
    MaNguoiDung INT NOT NULL,
    MaVaiTro INT NOT NULL,
    CONSTRAINT PK_NguoiDung_VaiTro PRIMARY KEY (MaNguoiDung, MaVaiTro),
    CONSTRAINT FK_NDVT_ND FOREIGN KEY (MaNguoiDung) REFERENCES dbo.NguoiDung(MaNguoiDung),
    CONSTRAINT FK_NDVT_VT FOREIGN KEY (MaVaiTro) REFERENCES dbo.VaiTro(MaVaiTro)
);

CREATE TABLE dbo.Quyen (
    MaQuyen INT IDENTITY(1,1) CONSTRAINT PK_Quyen PRIMARY KEY,
    TenQuyen VARCHAR(100) NOT NULL CONSTRAINT UQ_Quyen_Ten UNIQUE,
    MoTa NVARCHAR(500) NULL,
    Module VARCHAR(50) NULL
);

CREATE TABLE dbo.VaiTro_Quyen (
    MaVaiTro INT NOT NULL,
    MaQuyen INT NOT NULL,
    CONSTRAINT PK_VaiTro_Quyen PRIMARY KEY (MaVaiTro, MaQuyen),
    CONSTRAINT FK_VTQ_VT FOREIGN KEY (MaVaiTro) REFERENCES dbo.VaiTro(MaVaiTro),
    CONSTRAINT FK_VTQ_Q FOREIGN KEY (MaQuyen) REFERENCES dbo.Quyen(MaQuyen)
);

CREATE TABLE dbo.NhatKyHeThong (
    ID BIGINT IDENTITY(1,1) CONSTRAINT PK_NhatKyHeThong PRIMARY KEY,
    MaNguoiDung INT NULL,
    HanhDong NVARCHAR(255) NOT NULL,
    Module VARCHAR(50) NULL,
    ChiTiet NVARCHAR(MAX) NULL,
    DiaChi_IP VARCHAR(50) NULL,
    ThoiGian DATETIME2 NOT NULL DEFAULT SYSUTCDATETIME(),
    CONSTRAINT FK_NK_ND FOREIGN KEY (MaNguoiDung) REFERENCES dbo.NguoiDung(MaNguoiDung)
);
GO

/*==============================================================*/
/* PHẦN 2: TỔ CHỨC & ĐÀO TẠO (GỘP LỚP + CHUYÊN NGÀNH)            */
/*==============================================================*/

-- Khoa/Viện
CREATE TABLE dbo.Khoa (
    MaKhoa VARCHAR(20) CONSTRAINT PK_Khoa PRIMARY KEY,
    TenKhoa NVARCHAR(255) NOT NULL,
    DienThoai VARCHAR(15) NULL,
    Email VARCHAR(100) NULL,
    TruongKhoa NVARCHAR(255) NULL,
    NgayThanhLap DATE NULL,
    TrangThai BIT NOT NULL DEFAULT 1
);

-- Khóa học (K58, K59...) - độc lập để tái sử dụng cho nhiều lớp
CREATE TABLE dbo.KhoaHoc (
    MaKhoaHoc VARCHAR(20) CONSTRAINT PK_KhoaHoc PRIMARY KEY, -- ví dụ 'K58'
    TenKhoaHoc NVARCHAR(100) NOT NULL,                       -- ví dụ 'Khóa 58'
    NamBatDau INT NOT NULL,
    NamKetThuc INT NOT NULL,
    GhiChu NVARCHAR(500) NULL,
    TrangThai BIT NOT NULL DEFAULT 1,
    CONSTRAINT CK_KhoaHoc_Nam CHECK (NamKetThuc > NamBatDau)
);

-- Lớp-ChuyênNgành (gộp 2 bảng Lop + ChuyenNganh)
-- Ý nghĩa: Mỗi lớp đại diện cho một chuyên ngành cụ thể thuộc 1 khoa và 1 khóa học.
CREATE TABLE dbo.LopChuyenNganh (
    MaLopCN VARCHAR(30) CONSTRAINT PK_LopChuyenNganh PRIMARY KEY, -- ví dụ 'CNPM-K58', 'ANM-K58'
    TenLopCN NVARCHAR(200) NOT NULL,
    MaKhoa VARCHAR(20) NOT NULL,
    MaKhoaHoc VARCHAR(20) NOT NULL,           -- K58 / K59
    TenChuyenNganh NVARCHAR(255) NOT NULL,    -- lưu luôn tên chuyên ngành vì bạn muốn gộp
    GVCN NVARCHAR(255) NULL,
    SiSo INT NULL,
    TrangThai BIT NOT NULL DEFAULT 1,
    CONSTRAINT FK_LCN_Khoa FOREIGN KEY (MaKhoa) REFERENCES dbo.Khoa(MaKhoa),
    CONSTRAINT FK_LCN_KhoaHoc FOREIGN KEY (MaKhoaHoc) REFERENCES dbo.KhoaHoc(MaKhoaHoc)
);
CREATE INDEX IX_LCN_MaKhoa ON dbo.LopChuyenNganh(MaKhoa);
CREATE INDEX IX_LCN_MaKhoaHoc ON dbo.LopChuyenNganh(MaKhoaHoc);

-- Sinh viên (đổi FK sang LopChuyenNganh + thêm MaKhoaHoc để truy vấn nhanh)
CREATE TABLE dbo.SinhVien (
    MaSV VARCHAR(20) CONSTRAINT PK_SinhVien PRIMARY KEY,
    HoTen NVARCHAR(255) NOT NULL,
    NgaySinh DATE NULL,
    GioiTinh NVARCHAR(10) NULL CONSTRAINT CK_SV_GioiTinh CHECK (GioiTinh IN (N'Nam', N'Nữ', N'Khác')),
    CCCD VARCHAR(20) NULL CONSTRAINT UQ_SV_CCCD UNIQUE,
    DiaChi NVARCHAR(500) NULL,
    Email VARCHAR(100) NULL,
    SoDienThoai VARCHAR(15) NULL,

    MaLopCN VARCHAR(30) NOT NULL,    -- FK
    MaKhoaHoc VARCHAR(20) NOT NULL,  -- FK (denormalize có kiểm soát)

    HeDaoTao NVARCHAR(50) NOT NULL,        -- 'Đại học','Cao học'
    LoaiHinhDaoTao NVARCHAR(50) NOT NULL,  -- 'Chính quy','Vừa làm vừa học','Quân sự','Dân sự'...
    TrangThai NVARCHAR(50) NOT NULL DEFAULT N'Đang học',
    NgayNhapHoc DATE NULL,
    NgayTao DATETIME2 NOT NULL DEFAULT SYSUTCDATETIME(),

    CONSTRAINT FK_SV_LCN FOREIGN KEY (MaLopCN) REFERENCES dbo.LopChuyenNganh(MaLopCN),
    CONSTRAINT FK_SV_KhoaHoc FOREIGN KEY (MaKhoaHoc) REFERENCES dbo.KhoaHoc(MaKhoaHoc)
);
CREATE INDEX IX_SV_MaLopCN ON dbo.SinhVien(MaLopCN);
CREATE INDEX IX_SV_MaKhoaHoc ON dbo.SinhVien(MaKhoaHoc);
CREATE INDEX IX_SV_TrangThai ON dbo.SinhVien(TrangThai);
CREATE INDEX IX_SV_Email ON dbo.SinhVien(Email);

-- Ảnh sinh viên (lưu đường dẫn, có thể nhiều ảnh: thẻ SV, CCCD, chân dung...)
CREATE TABLE dbo.SinhVienAnh (
    MaAnh BIGINT IDENTITY(1,1) CONSTRAINT PK_SinhVienAnh PRIMARY KEY,
    MaSV VARCHAR(20) NOT NULL,
    LoaiAnh NVARCHAR(50) NOT NULL DEFAULT N'Chân dung', -- 'Chân dung','CCCD','Thẻ SV'...
    DuongDanAnh NVARCHAR(500) NOT NULL,                 -- URL hoặc path
    IsAvatar BIT NOT NULL DEFAULT 0,
    ThuTu INT NOT NULL DEFAULT 1,
    NgayTao DATETIME2 NOT NULL DEFAULT SYSUTCDATETIME(),
    TrangThai BIT NOT NULL DEFAULT 1,
    CONSTRAINT FK_SVA_SV FOREIGN KEY (MaSV) REFERENCES dbo.SinhVien(MaSV)
);
CREATE INDEX IX_SVA_MaSV ON dbo.SinhVienAnh(MaSV);
-- Mỗi SV chỉ 1 avatar đang active (lọc bằng unique filtered index)
CREATE UNIQUE INDEX UX_SVA_Avatar
ON dbo.SinhVienAnh(MaSV)
WHERE IsAvatar = 1 AND TrangThai = 1;

GO

/*==============================================================*/
/* PHẦN 3: HỌC KỲ - HỌC PHẦN - ĐĂNG KÝ                            */
/*==============================================================*/

CREATE TABLE dbo.HocKy (
    MaHocKy VARCHAR(20) CONSTRAINT PK_HocKy PRIMARY KEY,
    TenHocKy NVARCHAR(100) NOT NULL,
    NamHoc VARCHAR(20) NULL,
    NgayBatDau DATE NOT NULL,
    NgayKetThuc DATE NOT NULL,
    NgayBatDauDangKy DATE NULL,
    NgayKetThucDangKy DATE NULL,
    TrangThai NVARCHAR(50) NOT NULL DEFAULT N'Sắp diễn ra',
    CONSTRAINT CK_HK_Ngay CHECK (NgayKetThuc > NgayBatDau)
);

CREATE TABLE dbo.HocPhan (
    MaHocPhan VARCHAR(20) CONSTRAINT PK_HocPhan PRIMARY KEY,
    TenHocPhan NVARCHAR(255) NOT NULL,
    SoTinChi INT NOT NULL CONSTRAINT CK_HP_Tinchi CHECK (SoTinChi > 0),
    SoTietLyThuyet INT NULL,
    SoTietThucHanh INT NULL,
    MaKhoa VARCHAR(20) NOT NULL,
    LoaiHocPhan NVARCHAR(50) NULL,
    MoTa NVARCHAR(1000) NULL,
    TrangThai BIT NOT NULL DEFAULT 1,
    CONSTRAINT FK_HP_Khoa FOREIGN KEY (MaKhoa) REFERENCES dbo.Khoa(MaKhoa)
);

CREATE TABLE dbo.DangKyHocPhan (
    MaDangKy BIGINT IDENTITY(1,1) CONSTRAINT PK_DangKyHocPhan PRIMARY KEY,
    MaSV VARCHAR(20) NOT NULL,
    MaHocPhan VARCHAR(20) NOT NULL,
    MaHocKy VARCHAR(20) NOT NULL,
    NgayDangKy DATETIME2 NOT NULL DEFAULT SYSUTCDATETIME(),
    TrangThai NVARCHAR(50) NOT NULL DEFAULT N'Đã đăng ký',
    CONSTRAINT FK_DK_SV FOREIGN KEY (MaSV) REFERENCES dbo.SinhVien(MaSV),
    CONSTRAINT FK_DK_HP FOREIGN KEY (MaHocPhan) REFERENCES dbo.HocPhan(MaHocPhan),
    CONSTRAINT FK_DK_HK FOREIGN KEY (MaHocKy) REFERENCES dbo.HocKy(MaHocKy),
    CONSTRAINT UQ_DK UNIQUE (MaSV, MaHocPhan, MaHocKy)
);
CREATE INDEX IX_DK_MaSV ON dbo.DangKyHocPhan(MaSV);
CREATE INDEX IX_DK_MaHocKy ON dbo.DangKyHocPhan(MaHocKy);

GO

/*==============================================================*/
/* PHẦN 4: TÀI CHÍNH - HỌC PHÍ                                   */
/*==============================================================*/

CREATE TABLE dbo.LoaiPhi (
    MaLoaiPhi VARCHAR(20) CONSTRAINT PK_LoaiPhi PRIMARY KEY,
    TenLoaiPhi NVARCHAR(255) NOT NULL,
    DonViTinh NVARCHAR(50) NULL,
    MoTa NVARCHAR(500) NULL,
    TrangThai BIT NOT NULL DEFAULT 1
);

CREATE TABLE dbo.BieuPhi (
    MaBieuPhi BIGINT IDENTITY(1,1) CONSTRAINT PK_BieuPhi PRIMARY KEY,
    MaLoaiPhi VARCHAR(20) NOT NULL,
    MaHocKy VARCHAR(20) NOT NULL,
    HeDaoTao NVARCHAR(50) NULL,
    LoaiHinhDaoTao NVARCHAR(50) NULL,
    DonGia DECIMAL(18,2) NOT NULL CONSTRAINT CK_BieuPhi_DonGia CHECK (DonGia >= 0),
    NgayApDung DATE NOT NULL,
    NgayHetHan DATE NULL,
    GhiChu NVARCHAR(500) NULL,
    TrangThai BIT NOT NULL DEFAULT 1,
    CONSTRAINT FK_BP_LoaiPhi FOREIGN KEY (MaLoaiPhi) REFERENCES dbo.LoaiPhi(MaLoaiPhi),
    CONSTRAINT FK_BP_HocKy FOREIGN KEY (MaHocKy) REFERENCES dbo.HocKy(MaHocKy),
    CONSTRAINT CK_BP_Ngay CHECK (NgayHetHan IS NULL OR NgayHetHan >= NgayApDung)
);
CREATE INDEX IX_BP_TraGia ON dbo.BieuPhi(MaHocKy, MaLoaiPhi, HeDaoTao, LoaiHinhDaoTao, TrangThai, NgayApDung);

CREATE TABLE dbo.ChinhSachMienGiam (
    MaChinhSach VARCHAR(20) CONSTRAINT PK_ChinhSachMienGiam PRIMARY KEY,
    TenChinhSach NVARCHAR(255) NOT NULL,
    DoiTuongApDung NVARCHAR(500) NULL,
    LoaiMienGiam NVARCHAR(50) NOT NULL,
    GiaTriMienGiam DECIMAL(18,2) NOT NULL CONSTRAINT CK_CSMG_GiaTri CHECK (GiaTriMienGiam >= 0),
    NgayApDung DATE NULL,
    NgayHetHan DATE NULL,
    TrangThai BIT NOT NULL DEFAULT 1,
    CONSTRAINT CK_CSMG_Ngay CHECK (NgayHetHan IS NULL OR NgayHetHan >= ISNULL(NgayApDung,'19000101'))
);

CREATE TABLE dbo.SinhVien_MienGiam (
    ID BIGINT IDENTITY(1,1) CONSTRAINT PK_SV_MienGiam PRIMARY KEY,
    MaSV VARCHAR(20) NOT NULL,
    MaChinhSach VARCHAR(20) NOT NULL,
    MaHocKy VARCHAR(20) NOT NULL,
    NgayPheDuyet DATE NULL,
    NguoiPheDuyet INT NULL,
    GhiChu NVARCHAR(500) NULL,
    TrangThai BIT NOT NULL DEFAULT 1,
    CONSTRAINT FK_SVMG_SV FOREIGN KEY (MaSV) REFERENCES dbo.SinhVien(MaSV),
    CONSTRAINT FK_SVMG_CS FOREIGN KEY (MaChinhSach) REFERENCES dbo.ChinhSachMienGiam(MaChinhSach),
    CONSTRAINT FK_SVMG_HK FOREIGN KEY (MaHocKy) REFERENCES dbo.HocKy(MaHocKy),
    CONSTRAINT FK_SVMG_ND FOREIGN KEY (NguoiPheDuyet) REFERENCES dbo.NguoiDung(MaNguoiDung),
    CONSTRAINT UQ_SVMG UNIQUE (MaSV, MaChinhSach, MaHocKy)
);

CREATE TABLE dbo.CongNo (
    MaCongNo BIGINT IDENTITY(1,1) CONSTRAINT PK_CongNo PRIMARY KEY,
    MaSV VARCHAR(20) NOT NULL,
    MaHocKy VARCHAR(20) NOT NULL,
    TongPhaiNop DECIMAL(18,2) NOT NULL DEFAULT 0 CONSTRAINT CK_CN_TongPhaiNop CHECK (TongPhaiNop >= 0),
    TienMienGiam DECIMAL(18,2) NOT NULL DEFAULT 0 CONSTRAINT CK_CN_MienGiam CHECK (TienMienGiam >= 0),
    TongDaNop DECIMAL(18,2) NOT NULL DEFAULT 0 CONSTRAINT CK_CN_TongDaNop CHECK (TongDaNop >= 0),
    ConNo AS (TongPhaiNop - TienMienGiam - TongDaNop) PERSISTED,
    HanNop DATE NULL,
    NgayTao DATETIME2 NOT NULL DEFAULT SYSUTCDATETIME(),
    NgayCapNhat DATETIME2 NULL,
    TrangThai TINYINT NOT NULL DEFAULT 1, -- 1:Chưa TT,2:1 phần,3:Đã TT,4:Quá hạn
    CONSTRAINT FK_CN_SV FOREIGN KEY (MaSV) REFERENCES dbo.SinhVien(MaSV),
    CONSTRAINT FK_CN_HK FOREIGN KEY (MaHocKy) REFERENCES dbo.HocKy(MaHocKy),
    CONSTRAINT UQ_CN UNIQUE (MaSV, MaHocKy)
);
CREATE INDEX IX_CN_MaSV ON dbo.CongNo(MaSV);
CREATE INDEX IX_CN_MaHocKy ON dbo.CongNo(MaHocKy);
CREATE INDEX IX_CN_TrangThai ON dbo.CongNo(TrangThai);
CREATE INDEX IX_CN_HanNop ON dbo.CongNo(HanNop);

CREATE TABLE dbo.CongNoChiTiet (
    MaCongNoCT BIGINT IDENTITY(1,1) CONSTRAINT PK_CongNoChiTiet PRIMARY KEY,
    MaCongNo BIGINT NOT NULL,
    LoaiDong VARCHAR(30) NOT NULL, -- HP_TINCHI, PHI_CODINH, DIEU_CHINH
    RefId BIGINT NULL,
    MoTa NVARCHAR(300) NOT NULL,
    SoLuong DECIMAL(18,2) NOT NULL DEFAULT 1 CONSTRAINT CK_CNCT_SoLuong CHECK (SoLuong >= 0),
    DonGia DECIMAL(18,2) NOT NULL DEFAULT 0 CONSTRAINT CK_CNCT_DonGia CHECK (DonGia >= 0),
    ThanhTien AS (SoLuong * DonGia) PERSISTED,
    MienGiam DECIMAL(18,2) NOT NULL DEFAULT 0 CONSTRAINT CK_CNCT_MG CHECK (MienGiam >= 0),
    PhaiThu AS ((SoLuong * DonGia) - MienGiam) PERSISTED,
    CONSTRAINT FK_CNCT_CN FOREIGN KEY (MaCongNo) REFERENCES dbo.CongNo(MaCongNo)
);
CREATE INDEX IX_CNCT_MaCongNo ON dbo.CongNoChiTiet(MaCongNo);

CREATE TABLE dbo.PhuongThucThanhToan (
    MaPhuongThuc VARCHAR(20) CONSTRAINT PK_PhuongThuc PRIMARY KEY,
    TenPhuongThuc NVARCHAR(100) NOT NULL,
    MoTa NVARCHAR(500) NULL,
    TrangThai BIT NOT NULL DEFAULT 1
);

CREATE TABLE dbo.GiaoDich (
    MaGiaoDich VARCHAR(50) CONSTRAINT PK_GiaoDich PRIMARY KEY,
    MaCongNo BIGINT NOT NULL,
    SoTien DECIMAL(18,2) NOT NULL CONSTRAINT CK_GD_SoTien CHECK (SoTien > 0),
    MaPhuongThuc VARCHAR(20) NOT NULL,
    MaGiaoDichNganHang VARCHAR(100) NULL,
    NgayGiaoDich DATETIME2 NOT NULL DEFAULT SYSUTCDATETIME(),
    NoiDung NVARCHAR(500) NULL,
    NguoiNop NVARCHAR(255) NULL,
    NguoiThuTien INT NULL,
    TrangThai TINYINT NOT NULL DEFAULT 2, -- 1:Fail,2:Success,3:Pending
    GhiChu NVARCHAR(500) NULL,
    CONSTRAINT FK_GD_CN FOREIGN KEY (MaCongNo) REFERENCES dbo.CongNo(MaCongNo),
    CONSTRAINT FK_GD_PTTT FOREIGN KEY (MaPhuongThuc) REFERENCES dbo.PhuongThucThanhToan(MaPhuongThuc),
    CONSTRAINT FK_GD_ND FOREIGN KEY (NguoiThuTien) REFERENCES dbo.NguoiDung(MaNguoiDung)
);
CREATE INDEX IX_GD_NgayGD ON dbo.GiaoDich(NgayGiaoDich);
CREATE INDEX IX_GD_TrangThai ON dbo.GiaoDich(TrangThai);
CREATE INDEX IX_GD_MaGDNH ON dbo.GiaoDich(MaGiaoDichNganHang);

CREATE TABLE dbo.PhanBoThanhToan (
    MaPB BIGINT IDENTITY(1,1) CONSTRAINT PK_PhanBoThanhToan PRIMARY KEY,
    MaGiaoDich VARCHAR(50) NOT NULL,
    MaCongNoCT BIGINT NOT NULL,
    SoTienPhanBo DECIMAL(18,2) NOT NULL CONSTRAINT CK_PB_SoTien CHECK (SoTienPhanBo > 0),
    CONSTRAINT FK_PB_GD FOREIGN KEY (MaGiaoDich) REFERENCES dbo.GiaoDich(MaGiaoDich),
    CONSTRAINT FK_PB_CNCT FOREIGN KEY (MaCongNoCT) REFERENCES dbo.CongNoChiTiet(MaCongNoCT),
    CONSTRAINT UQ_PB UNIQUE (MaGiaoDich, MaCongNoCT)
);

CREATE TABLE dbo.BienLai (
    MaBienLai UNIQUEIDENTIFIER NOT NULL DEFAULT NEWID() CONSTRAINT PK_BienLai PRIMARY KEY,
    MaGiaoDich VARCHAR(50) NOT NULL,
    SoBienLai VARCHAR(50) NOT NULL CONSTRAINT UQ_BienLai_So UNIQUE,
    NgayXuat DATETIME2 NOT NULL DEFAULT SYSUTCDATETIME(),
    NguoiXuat INT NULL,
    FilePath VARCHAR(500) NULL,
    TrangThai BIT NOT NULL DEFAULT 1,
    CONSTRAINT FK_BL_GD FOREIGN KEY (MaGiaoDich) REFERENCES dbo.GiaoDich(MaGiaoDich),
    CONSTRAINT FK_BL_ND FOREIGN KEY (NguoiXuat) REFERENCES dbo.NguoiDung(MaNguoiDung)
);

CREATE TABLE dbo.ThongBaoHocPhi (
    MaThongBao BIGINT IDENTITY(1,1) CONSTRAINT PK_ThongBaoHocPhi PRIMARY KEY,
    MaHocKy VARCHAR(20) NOT NULL,
    NamHoc VARCHAR(20) NULL,
    TieuDe NVARCHAR(255) NOT NULL,
    NoiDung NVARCHAR(MAX) NULL,
    NgayPhatHanh DATE NOT NULL DEFAULT CAST(GETDATE() AS DATE),
    HanNop DATE NULL,
    DoiTuongApDung NVARCHAR(500) NULL,
    TongSoSinhVien INT NULL,
    TongSoTienCongNo DECIMAL(18,2) NULL,
    TrangThai TINYINT NOT NULL DEFAULT 1,
    HinhThucGui VARCHAR(50) NULL,
    ThoiDiemGui DATETIME2 NULL,
    NguoiPhatHanh INT NULL,
    NgayTao DATETIME2 NOT NULL DEFAULT SYSUTCDATETIME(),
    NgayCapNhat DATETIME2 NULL,
    CONSTRAINT FK_TBHP_HocKy FOREIGN KEY (MaHocKy) REFERENCES dbo.HocKy(MaHocKy),
    CONSTRAINT FK_TBHP_ND FOREIGN KEY (NguoiPhatHanh) REFERENCES dbo.NguoiDung(MaNguoiDung)
);
CREATE INDEX IX_TBHP_MaHocKy ON dbo.ThongBaoHocPhi(MaHocKy);
CREATE INDEX IX_TBHP_TrangThai ON dbo.ThongBaoHocPhi(TrangThai);

CREATE TABLE dbo.ThongBaoHocPhi_SinhVien (
    Id BIGINT IDENTITY(1,1) CONSTRAINT PK_ThongBaoHocPhi_SinhVien PRIMARY KEY,
    MaThongBao BIGINT NOT NULL,
    MaSV VARCHAR(20) NOT NULL,
    TrangThaiDoc TINYINT NOT NULL DEFAULT 0,
    TrangThaiThanhToan TINYINT NOT NULL DEFAULT 1,
    NgayGui DATETIME2 NULL,
    NgayXem DATETIME2 NULL,
    GhiChu NVARCHAR(500) NULL,
    CONSTRAINT FK_TBHPSV_TB FOREIGN KEY (MaThongBao) REFERENCES dbo.ThongBaoHocPhi(MaThongBao),
    CONSTRAINT FK_TBHPSV_SV FOREIGN KEY (MaSV) REFERENCES dbo.SinhVien(MaSV),
    CONSTRAINT UQ_TBHPSV UNIQUE (MaThongBao, MaSV)
);
CREATE INDEX IX_TBHPSV_MaSV ON dbo.ThongBaoHocPhi_SinhVien(MaSV);
CREATE INDEX IX_TBHPSV_TrangThaiDoc ON dbo.ThongBaoHocPhi_SinhVien(TrangThaiDoc);

GO
CREATE SEQUENCE dbo.SeqBienLai AS INT START WITH 1 INCREMENT BY 1;
GO

CREATE OR ALTER TRIGGER dbo.trg_SinhSoBienLai
ON dbo.BienLai
INSTEAD OF INSERT
AS
BEGIN
    SET NOCOUNT ON;

    INSERT INTO dbo.BienLai (MaBienLai, MaGiaoDich, SoBienLai, NgayXuat, NguoiXuat, FilePath, TrangThai)
    SELECT
        ISNULL(i.MaBienLai, NEWID()),
        i.MaGiaoDich,
        CONCAT('BL-', FORMAT(SYSUTCDATETIME(), 'yyyyMMdd'), '-', RIGHT('000000' + CAST(NEXT VALUE FOR dbo.SeqBienLai AS VARCHAR(10)), 6)),
        ISNULL(i.NgayXuat, SYSUTCDATETIME()),
        i.NguoiXuat,
        i.FilePath,
        ISNULL(i.TrangThai, 1)
    FROM inserted i;
END;
GO

/*==============================================================*/
/* PHẦN 5: AUDIT CÔNG NỢ                                         */
/*==============================================================*/

CREATE TABLE dbo.LichSu_CongNo (
    ID BIGINT IDENTITY(1,1) CONSTRAINT PK_LichSu_CongNo PRIMARY KEY,
    MaCongNo BIGINT NOT NULL,
    Truoc_TongPhaiNop DECIMAL(18,2) NULL,
    Sau_TongPhaiNop DECIMAL(18,2) NULL,
    Truoc_TienMienGiam DECIMAL(18,2) NULL,
    Sau_TienMienGiam DECIMAL(18,2) NULL,
    Truoc_TongDaNop DECIMAL(18,2) NULL,
    Sau_TongDaNop DECIMAL(18,2) NULL,
    LyDoThayDoi NVARCHAR(500) NULL,
    NguoiThayDoi INT NULL,
    NgayThayDoi DATETIME2 NOT NULL DEFAULT SYSUTCDATETIME(),
    CONSTRAINT FK_LS_CN FOREIGN KEY (MaCongNo) REFERENCES dbo.CongNo(MaCongNo),
    CONSTRAINT FK_LS_ND FOREIGN KEY (NguoiThayDoi) REFERENCES dbo.NguoiDung(MaNguoiDung)
);
GO

CREATE OR ALTER TRIGGER dbo.trg_LogThayDoiCongNo
ON dbo.CongNo
AFTER UPDATE
AS
BEGIN
    SET NOCOUNT ON;

    INSERT INTO dbo.LichSu_CongNo
    (
        MaCongNo,
        Truoc_TongPhaiNop, Sau_TongPhaiNop,
        Truoc_TienMienGiam, Sau_TienMienGiam,
        Truoc_TongDaNop, Sau_TongDaNop,
        LyDoThayDoi, NguoiThayDoi
    )
    SELECT
        d.MaCongNo,
        d.TongPhaiNop, i.TongPhaiNop,
        d.TienMienGiam, i.TienMienGiam,
        d.TongDaNop, i.TongDaNop,
        N'Update Công nợ', NULL
    FROM deleted d
    INNER JOIN inserted i ON i.MaCongNo = d.MaCongNo
    WHERE
        ISNULL(d.TongPhaiNop,0) <> ISNULL(i.TongPhaiNop,0)
        OR ISNULL(d.TienMienGiam,0) <> ISNULL(i.TienMienGiam,0)
        OR ISNULL(d.TongDaNop,0) <> ISNULL(i.TongDaNop,0);
END;
GO

/*==============================================================*/
/* PHẦN 6: STORED PROCEDURES                                     */
/*==============================================================*/

---------------------------------------------------------------
-- SP: Tính công nợ (sửa theo schema mới: LopChuyenNganh + KhoaHoc)
---------------------------------------------------------------
CREATE OR ALTER PROCEDURE dbo.sp_TinhCongNo
    @MaSV VARCHAR(20),
    @MaHocKy VARCHAR(20)
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @HeDaoTao NVARCHAR(50);
    DECLARE @LoaiHinhDaoTao NVARCHAR(50);

    SELECT
        @HeDaoTao = HeDaoTao,
        @LoaiHinhDaoTao = LoaiHinhDaoTao
    FROM dbo.SinhVien
    WHERE MaSV = @MaSV;

    IF @HeDaoTao IS NULL
        THROW 50010, N'Không tìm thấy sinh viên', 1;

    BEGIN TRY
        BEGIN TRAN;

        DECLARE @MaCongNo BIGINT;

        IF EXISTS (SELECT 1 FROM dbo.CongNo WHERE MaSV=@MaSV AND MaHocKy=@MaHocKy)
            SELECT @MaCongNo = MaCongNo FROM dbo.CongNo WHERE MaSV=@MaSV AND MaHocKy=@MaHocKy;
        ELSE
        BEGIN
            INSERT INTO dbo.CongNo (MaSV, MaHocKy, HanNop)
            VALUES (@MaSV, @MaHocKy, NULL);
            SET @MaCongNo = SCOPE_IDENTITY();
        END

        -- rebuild lines
        DELETE FROM dbo.PhanBoThanhToan
        WHERE MaCongNoCT IN (SELECT MaCongNoCT FROM dbo.CongNoChiTiet WHERE MaCongNo=@MaCongNo);

        DELETE FROM dbo.CongNoChiTiet WHERE MaCongNo=@MaCongNo;

        -- biểu phí học phí tín chỉ
        DECLARE @DonGiaTinChi DECIMAL(18,2);

        SELECT TOP 1 @DonGiaTinChi = bp.DonGia
        FROM dbo.BieuPhi bp
        WHERE bp.MaHocKy = @MaHocKy
          AND bp.MaLoaiPhi IN ('HP_TC','HP_CH')
          AND (bp.HeDaoTao = @HeDaoTao OR bp.HeDaoTao IS NULL)
          AND (bp.LoaiHinhDaoTao = @LoaiHinhDaoTao OR bp.LoaiHinhDaoTao IS NULL)
          AND bp.TrangThai = 1
          AND bp.NgayApDung <= CAST(GETDATE() AS DATE)
          AND (bp.NgayHetHan IS NULL OR bp.NgayHetHan >= CAST(GETDATE() AS DATE))
        ORDER BY
          CASE WHEN bp.LoaiHinhDaoTao = @LoaiHinhDaoTao THEN 0 ELSE 1 END,
          CASE WHEN bp.HeDaoTao = @HeDaoTao THEN 0 ELSE 1 END,
          bp.NgayApDung DESC;

        SET @DonGiaTinChi = ISNULL(@DonGiaTinChi, 0);

        -- build lines từ đăng ký
        INSERT INTO dbo.CongNoChiTiet (MaCongNo, LoaiDong, RefId, MoTa, SoLuong, DonGia, MienGiam)
        SELECT
            @MaCongNo,
            'HP_TINCHI',
            CAST(dk.MaDangKy AS BIGINT),
            CONCAT(N'Học phí: ', hp.MaHocPhan, N' - ', hp.TenHocPhan),
            CAST(hp.SoTinChi AS DECIMAL(18,2)),
            @DonGiaTinChi,
            0
        FROM dbo.DangKyHocPhan dk
        INNER JOIN dbo.HocPhan hp ON hp.MaHocPhan = dk.MaHocPhan
        WHERE dk.MaSV = @MaSV
          AND dk.MaHocKy = @MaHocKy
          AND dk.TrangThai = N'Đã đăng ký';

        -- BHYT (nếu có)
        DECLARE @PhiBHYT DECIMAL(18,2) = 0;

        SELECT TOP 1 @PhiBHYT = bp.DonGia
        FROM dbo.BieuPhi bp
        WHERE bp.MaHocKy = @MaHocKy
          AND bp.MaLoaiPhi = 'BHYT'
          AND (bp.HeDaoTao = @HeDaoTao OR bp.HeDaoTao IS NULL)
          AND (bp.LoaiHinhDaoTao = @LoaiHinhDaoTao OR bp.LoaiHinhDaoTao IS NULL)
          AND bp.TrangThai = 1
          AND bp.NgayApDung <= CAST(GETDATE() AS DATE)
          AND (bp.NgayHetHan IS NULL OR bp.NgayHetHan >= CAST(GETDATE() AS DATE))
        ORDER BY bp.NgayApDung DESC;

        SET @PhiBHYT = ISNULL(@PhiBHYT, 0);

        IF @PhiBHYT > 0
            INSERT INTO dbo.CongNoChiTiet (MaCongNo, LoaiDong, RefId, MoTa, SoLuong, DonGia, MienGiam)
            VALUES (@MaCongNo, 'PHI_CODINH', NULL, N'Bảo hiểm y tế', 1, @PhiBHYT, 0);

        -- tính miễn giảm trên tổng học phí tín chỉ
        DECLARE @TongHocPhi DECIMAL(18,2) = 0;
        DECLARE @TienMienGiam DECIMAL(18,2) = 0;

        SELECT @TongHocPhi = ISNULL(SUM(ThanhTien),0)
        FROM dbo.CongNoChiTiet
        WHERE MaCongNo=@MaCongNo AND LoaiDong='HP_TINCHI';

        SELECT @TienMienGiam = ISNULL(SUM(
            CASE
                WHEN cs.LoaiMienGiam = N'Phần trăm' THEN (@TongHocPhi * cs.GiaTriMienGiam / 100.0)
                ELSE cs.GiaTriMienGiam
            END
        ),0)
        FROM dbo.SinhVien_MienGiam svmg
        INNER JOIN dbo.ChinhSachMienGiam cs ON cs.MaChinhSach = svmg.MaChinhSach
        WHERE svmg.MaSV=@MaSV AND svmg.MaHocKy=@MaHocKy AND svmg.TrangThai=1
          AND cs.TrangThai=1
          AND (cs.NgayApDung IS NULL OR cs.NgayApDung <= CAST(GETDATE() AS DATE))
          AND (cs.NgayHetHan IS NULL OR cs.NgayHetHan >= CAST(GETDATE() AS DATE));

        IF @TienMienGiam > @TongHocPhi SET @TienMienGiam = @TongHocPhi;

        DECLARE @TongPhaiNop DECIMAL(18,2) = 0;
        SELECT @TongPhaiNop = ISNULL(SUM(PhaiThu),0)
        FROM dbo.CongNoChiTiet
        WHERE MaCongNo=@MaCongNo;

        UPDATE dbo.CongNo
        SET TongPhaiNop = @TongPhaiNop,
            TienMienGiam = @TienMienGiam,
            NgayCapNhat = SYSUTCDATETIME(),
            TrangThai = CASE
                WHEN (TongDaNop >= (@TongPhaiNop - @TienMienGiam)) AND (@TongPhaiNop - @TienMienGiam) > 0 THEN 3
                WHEN TongDaNop > 0 THEN 2
                ELSE 1
            END
        WHERE MaCongNo=@MaCongNo;

        COMMIT;

        SELECT @MaCongNo AS MaCongNo, @TongPhaiNop AS TongPhaiNop, @TienMienGiam AS TienMienGiam;
    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0 ROLLBACK;
        THROW;
    END CATCH
END;
GO

---------------------------------------------------------------
-- SP: Thanh toán (giữ nguyên logic allocation)
---------------------------------------------------------------
CREATE OR ALTER PROCEDURE dbo.sp_XuLyThanhToan
    @MaSV VARCHAR(20),
    @MaHocKy VARCHAR(20),
    @SoTien DECIMAL(18,2),
    @MaPhuongThuc VARCHAR(20),
    @MaGiaoDichNganHang VARCHAR(100) = NULL,
    @NguoiThuTien INT = NULL,
    @XuatBienLai BIT = 1
AS
BEGIN
    SET NOCOUNT ON;

    BEGIN TRY
        BEGIN TRAN;

        DECLARE @MaCongNo BIGINT;

        SELECT @MaCongNo = MaCongNo
        FROM dbo.CongNo WITH (UPDLOCK, ROWLOCK)
        WHERE MaSV=@MaSV AND MaHocKy=@MaHocKy;

        IF @MaCongNo IS NULL
            THROW 50001, N'Không tìm thấy công nợ cho sinh viên này trong học kỳ', 1;

        IF @SoTien <= 0
            THROW 50002, N'Số tiền thanh toán không hợp lệ', 1;

        DECLARE @MaGiaoDich VARCHAR(50) =
            CONCAT('GD', FORMAT(GETDATE(), 'yyyyMMddHHmmss'), '_', @MaSV);

        INSERT INTO dbo.GiaoDich
        (MaGiaoDich, MaCongNo, SoTien, MaPhuongThuc, MaGiaoDichNganHang, NoiDung, NguoiNop, NguoiThuTien, TrangThai)
        VALUES
        (
            @MaGiaoDich, @MaCongNo, @SoTien, @MaPhuongThuc, @MaGiaoDichNganHang,
            N'Thu học phí', (SELECT HoTen FROM dbo.SinhVien WHERE MaSV=@MaSV), @NguoiThuTien, 2
        );

        DECLARE @ConLai DECIMAL(18,2) = @SoTien;

        ;WITH CT AS
        (
            SELECT
                ct.MaCongNoCT,
                ct.PhaiThu,
                ISNULL(pb.DaPB, 0) AS DaPB,
                (ct.PhaiThu - ISNULL(pb.DaPB,0)) AS ConPhaiThu
            FROM dbo.CongNoChiTiet ct
            OUTER APPLY (
                SELECT SUM(pbt.SoTienPhanBo) AS DaPB
                FROM dbo.PhanBoThanhToan pbt
                WHERE pbt.MaCongNoCT = ct.MaCongNoCT
            ) pb
            WHERE ct.MaCongNo=@MaCongNo
              AND (ct.PhaiThu - ISNULL(pb.DaPB,0)) > 0
        )
        SELECT * INTO #CT_CAN_PB FROM CT ORDER BY MaCongNoCT;

        DECLARE @MaCongNoCT BIGINT, @Can DECIMAL(18,2), @PB DECIMAL(18,2);

        WHILE EXISTS (SELECT 1 FROM #CT_CAN_PB WHERE ConPhaiThu > 0) AND @ConLai > 0
        BEGIN
            SELECT TOP 1 @MaCongNoCT = MaCongNoCT, @Can = ConPhaiThu
            FROM #CT_CAN_PB
            WHERE ConPhaiThu > 0
            ORDER BY MaCongNoCT;

            SET @PB = CASE WHEN @ConLai >= @Can THEN @Can ELSE @ConLai END;

            INSERT INTO dbo.PhanBoThanhToan (MaGiaoDich, MaCongNoCT, SoTienPhanBo)
            VALUES (@MaGiaoDich, @MaCongNoCT, @PB);

            UPDATE #CT_CAN_PB
            SET ConPhaiThu = ConPhaiThu - @PB
            WHERE MaCongNoCT = @MaCongNoCT;

            SET @ConLai = @ConLai - @PB;
        END

        DROP TABLE #CT_CAN_PB;

        DECLARE @TongPhaiNop DECIMAL(18,2), @TienMienGiam DECIMAL(18,2), @TongDaNopMoi DECIMAL(18,2);

        SELECT
            @TongPhaiNop = TongPhaiNop,
            @TienMienGiam = TienMienGiam,
            @TongDaNopMoi = TongDaNop + @SoTien
        FROM dbo.CongNo
        WHERE MaCongNo=@MaCongNo;

        UPDATE dbo.CongNo
        SET TongDaNop = TongDaNop + @SoTien,
            NgayCapNhat = SYSUTCDATETIME(),
            TrangThai = CASE
                WHEN (@TongPhaiNop - @TienMienGiam - @TongDaNopMoi) <= 0 AND (@TongPhaiNop - @TienMienGiam) > 0 THEN 3
                WHEN @TongDaNopMoi > 0 THEN 2
                ELSE 1
            END
        WHERE MaCongNo=@MaCongNo;

        IF @XuatBienLai = 1
            INSERT INTO dbo.BienLai (MaGiaoDich, NguoiXuat, FilePath) VALUES (@MaGiaoDich, @NguoiThuTien, NULL);

        COMMIT;

        SELECT @MaGiaoDich AS MaGiaoDich, N'Thanh toán thành công' AS ThongBao;
    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0 ROLLBACK;
        THROW;
    END CATCH
END;
GO

---------------------------------------------------------------
-- SP: Báo cáo theo khoa
---------------------------------------------------------------
CREATE OR ALTER PROCEDURE dbo.sp_BaoCaoCongNo_TheoKhoa
    @MaHocKy VARCHAR(20)
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        k.MaKhoa,
        k.TenKhoa,
        COUNT(DISTINCT cn.MaSV) AS TongSoSinhVien,
        SUM(cn.TongPhaiNop) AS TongPhaiThu,
        SUM(cn.TienMienGiam) AS TongMienGiam,
        SUM(cn.TongDaNop) AS TongDaThu,
        SUM(cn.ConNo) AS TongConNo,
        SUM(CASE WHEN cn.TrangThai = 3 THEN 1 ELSE 0 END) AS SoSVDaThanhToan,
        SUM(CASE WHEN cn.TrangThai = 4 THEN 1 ELSE 0 END) AS SoSVQuaHan
    FROM dbo.CongNo cn
    INNER JOIN dbo.SinhVien sv ON cn.MaSV = sv.MaSV
    INNER JOIN dbo.LopChuyenNganh lcn ON sv.MaLopCN = lcn.MaLopCN
    INNER JOIN dbo.Khoa k ON lcn.MaKhoa = k.MaKhoa
    WHERE cn.MaHocKy = @MaHocKy
    GROUP BY k.MaKhoa, k.TenKhoa
    ORDER BY k.TenKhoa;
END;
GO

---------------------------------------------------------------
-- SP: Job quét quá hạn
---------------------------------------------------------------
CREATE OR ALTER PROCEDURE dbo.sp_CapNhatTrangThaiQuaHan
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @SoBanGhi INT = 0;

    UPDATE dbo.CongNo
    SET TrangThai = 4,
        NgayCapNhat = SYSUTCDATETIME()
    WHERE HanNop < CAST(GETDATE() AS DATE)
      AND ConNo > 0
      AND TrangThai <> 4;

    SET @SoBanGhi = @@ROWCOUNT;

    INSERT INTO dbo.NhatKyHeThong (HanhDong, Module, ChiTiet)
    VALUES (N'Chạy Job Quét Quá Hạn', 'Job-System', CONCAT(N'Đã cập nhật ', @SoBanGhi, N' khoản công nợ sang Quá hạn'));
END;
GO

/*==============================================================*/
/* PHẦN 7: SEED DATA (DEMO)                                      */
/*==============================================================*/

-- Vai trò
INSERT INTO dbo.VaiTro (TenVaiTro, MoTa) VALUES
(N'Administrator', N'Quản trị viên toàn quyền'),
(N'PhongTaiChinh', N'Cán bộ Tài chính/Kế toán'),
(N'PhongDaoTao',   N'Cán bộ Đào tạo'),
(N'SinhVien',      N'Người học');

-- Người dùng (demo md5 123456)
INSERT INTO dbo.NguoiDung (TenDangNhap, MatKhauHash, HoTen, Email, SoDienThoai) VALUES
('admin',    'e10adc3949ba59abbe56e057f20f883e', N'Trần Quản Trị', 'admin@mta.edu.vn',   '0901234567'),
('ketoan01', 'e10adc3949ba59abbe56e057f20f883e', N'Nguyễn Thị Thu Ngân', 'nganttt@mta.edu.vn', '0912345678'),
('daotao01', 'e10adc3949ba59abbe56e057f20f883e', N'Phạm Giáo Vụ', 'vupg@mta.edu.vn',    '0987654321');

-- Gán user-role
INSERT INTO dbo.NguoiDung_VaiTro (MaNguoiDung, MaVaiTro)
SELECT nd.MaNguoiDung, vt.MaVaiTro
FROM dbo.NguoiDung nd
JOIN dbo.VaiTro vt ON
    (nd.TenDangNhap='admin' AND vt.TenVaiTro=N'Administrator')
 OR (nd.TenDangNhap='ketoan01' AND vt.TenVaiTro=N'PhongTaiChinh')
 OR (nd.TenDangNhap='daotao01' AND vt.TenVaiTro=N'PhongDaoTao');

-- Khoa
INSERT INTO dbo.Khoa (MaKhoa, TenKhoa, Email, TruongKhoa) VALUES
('FIT',  N'Khoa Công nghệ Thông tin', 'fit@mta.edu.vn',  N'TS. Nguyễn Văn A'),
('DTVT', N'Khoa Điện tử Viễn thông',  'dtvt@mta.edu.vn', N'PGS. Trần Văn B'),
('CK',   N'Khoa Cơ khí',              'cokhi@mta.edu.vn',N'TS. Lê Văn C');

-- Khóa học
INSERT INTO dbo.KhoaHoc (MaKhoaHoc, TenKhoaHoc, NamBatDau, NamKetThuc, GhiChu) VALUES
('K58', N'Khóa 58', 2022, 2027, N'Đại học 5 năm'),
('K59', N'Khóa 59', 2023, 2028, N'Đại học 5 năm');

-- Lớp chuyên ngành
INSERT INTO dbo.LopChuyenNganh (MaLopCN, TenLopCN, MaKhoa, MaKhoaHoc, TenChuyenNganh, GVCN) VALUES
('CNPM-K58', N'Lớp CNPM Khóa 58', 'FIT', 'K58', N'Công nghệ phần mềm', N'Thầy Hùng'),
('ANM-K58',  N'Lớp ANM Khóa 58',  'FIT', 'K58', N'An ninh mạng',       N'Thầy Dũng'),
('OTO-K59',  N'Lớp Ô tô QS Khóa 59','CK', 'K59', N'Kỹ thuật Ô tô quân sự', N'Thầy Tuấn');

-- Sinh viên
INSERT INTO dbo.SinhVien (MaSV, HoTen, NgaySinh, GioiTinh, CCCD, Email, MaLopCN, MaKhoaHoc, HeDaoTao, LoaiHinhDaoTao)
VALUES
('SV001', N'Nguyễn Văn An',  '2005-01-15', N'Nam', '001099000001', 'sv001@st.mta.edu.vn', 'CNPM-K58', 'K58', N'Đại học', N'Dân sự'),
('SV002', N'Trần Thị Bích',  '2005-03-20', N'Nữ',  '001099000002', 'sv002@st.mta.edu.vn', 'CNPM-K58', 'K58', N'Đại học', N'Dân sự'),
('SV004', N'Phạm Minh Duy',  '2005-07-25', N'Nam', '001099000004', 'sv004@st.mta.edu.vn', 'ANM-K58',  'K58', N'Đại học', N'Quân sự'),
('SV009', N'Vũ Văn Ích',     '2003-08-08', N'Nam', '001099000009', 'sv009@st.mta.edu.vn', 'OTO-K59',  'K59', N'Đại học', N'Dân sự');

-- Ảnh sinh viên (demo)
INSERT INTO dbo.SinhVienAnh (MaSV, LoaiAnh, DuongDanAnh, IsAvatar, ThuTu)
VALUES
('SV001', N'Chân dung', N'/uploads/students/SV001/avatar.jpg', 1, 1),
('SV002', N'Chân dung', N'/uploads/students/SV002/avatar.jpg', 1, 1);

-- Học kỳ
INSERT INTO dbo.HocKy (MaHocKy, TenHocKy, NamHoc, NgayBatDau, NgayKetThuc, TrangThai) VALUES
('HK1_2526', N'Học kỳ 1 Năm học 2025-2026', '2025-2026', '2025-08-15', '2026-01-15', N'Đang diễn ra');

-- Học phần
INSERT INTO dbo.HocPhan (MaHocPhan, TenHocPhan, SoTinChi, MaKhoa, LoaiHocPhan) VALUES
('ENG101', N'Tiếng Anh cơ bản 1', 4, 'FIT',  N'Bắt buộc'),
('IT201',  N'Cấu trúc dữ liệu và giải thuật', 3, 'FIT', N'Bắt buộc'),
('IT202',  N'Cơ sở dữ liệu', 3, 'FIT', N'Bắt buộc');

-- Đăng ký
INSERT INTO dbo.DangKyHocPhan (MaSV, MaHocPhan, MaHocKy) VALUES
('SV001','IT201','HK1_2526'), ('SV001','IT202','HK1_2526'), ('SV001','ENG101','HK1_2526'),
('SV002','IT201','HK1_2526'), ('SV002','ENG101','HK1_2526');

-- Loại phí
INSERT INTO dbo.LoaiPhi (MaLoaiPhi, TenLoaiPhi, DonViTinh) VALUES
('HP_TC', N'Học phí tín chỉ', N'VNĐ/Tín chỉ'),
('BHYT',  N'Bảo hiểm y tế',   N'VNĐ/Năm');

-- Biểu phí
INSERT INTO dbo.BieuPhi (MaLoaiPhi, MaHocKy, HeDaoTao, LoaiHinhDaoTao, DonGia, NgayApDung, GhiChu)
VALUES
('HP_TC','HK1_2526', N'Đại học', N'Dân sự', 480000, '2025-08-01', N'ĐH dân sự'),
('HP_TC','HK1_2526', N'Đại học', N'Quân sự', 0,     '2025-08-01', N'Quân sự miễn'),
('BHYT', 'HK1_2526', N'Đại học', N'Dân sự',  850000,'2025-08-01', N'Thu theo năm');

-- Miễn giảm
INSERT INTO dbo.ChinhSachMienGiam (MaChinhSach, TenChinhSach, LoaiMienGiam, GiaTriMienGiam)
VALUES
('CS02', N'Hộ nghèo/Vùng sâu vùng xa', N'Phần trăm', 50.00);

-- H?c b?ng
INSERT INTO dbo.HocBong (MaHocBong, TenHocBong, LoaiGiaTri, GiaTri, NamHoc)
VALUES
('HB01', N'H?c b?ng khuy?n kh¡ch', N'Ph?n tram', 30.00, '2025-2026');

INSERT INTO dbo.SinhVien_MienGiam (MaSV, MaChinhSach, MaHocKy, NguoiPheDuyet)
SELECT 'SV002','CS02','HK1_2526', nd.MaNguoiDung
FROM dbo.NguoiDung nd
WHERE nd.TenDangNhap='ketoan01';

INSERT INTO dbo.SinhVien_HocBong (MaSV, MaHocBong, MaHocKy, NguoiPheDuyet)
SELECT 'SV001','HB01','HK1_2526', nd.MaNguoiDung
FROM dbo.NguoiDung nd
WHERE nd.TenDangNhap='ketoan01';

-- PTTT
INSERT INTO dbo.PhuongThucThanhToan (MaPhuongThuc, TenPhuongThuc) VALUES
('MB', N'App MB Bank'),
('TIENMAT', N'Tiền mặt tại quầy');

GO

/*==============================================================*/
/* PHẦN 8: DEMO FLOW                                             */
/*==============================================================*/

PRINT N'--- TÍNH CÔNG NỢ ---';
EXEC dbo.sp_TinhCongNo 'SV001','HK1_2526';
EXEC dbo.sp_TinhCongNo 'SV002','HK1_2526';

PRINT N'--- THANH TOÁN SV001 (đóng hết) ---';
DECLARE @ConNoSV001 DECIMAL(18,2);
SELECT @ConNoSV001 = ConNo FROM dbo.CongNo WHERE MaSV='SV001' AND MaHocKy='HK1_2526';

EXEC dbo.sp_XuLyThanhToan
    @MaSV='SV001',
    @MaHocKy='HK1_2526',
    @SoTien=@ConNoSV001,
    @MaPhuongThuc='MB',
    @MaGiaoDichNganHang='MB_TRANS_009988',
    @NguoiThuTien=(SELECT MaNguoiDung FROM dbo.NguoiDung WHERE TenDangNhap='ketoan01'),
    @XuatBienLai=1;

PRINT N'--- QUICK CHECK ---';
SELECT * FROM dbo.CongNo ORDER BY MaSV;
SELECT cn.MaSV, ct.* FROM dbo.CongNoChiTiet ct JOIN dbo.CongNo cn ON cn.MaCongNo=ct.MaCongNo ORDER BY cn.MaSV, ct.MaCongNoCT;
SELECT * FROM dbo.GiaoDich ORDER BY NgayGiaoDich DESC;
SELECT * FROM dbo.PhanBoThanhToan ORDER BY MaPB DESC;
SELECT * FROM dbo.BienLai ORDER BY NgayXuat DESC;
SELECT * FROM dbo.SinhVienAnh ORDER BY MaSV, ThuTu;
GO
