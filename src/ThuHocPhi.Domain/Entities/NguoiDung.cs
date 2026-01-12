using System;
using System.Collections.Generic;

namespace ThuHocPhi.Domain.Entities;

public sealed class NguoiDung
{
    public int MaNguoiDung { get; set; }
    public string TenDangNhap { get; set; } = string.Empty;
    public string MatKhauHash { get; set; } = string.Empty;
    public string HoTen { get; set; } = string.Empty;
    public string? Email { get; set; }
    public string? SoDienThoai { get; set; }
    public DateTime? LanDangNhapCuoi { get; set; }
    public bool TrangThai { get; set; }
    public DateTime NgayTao { get; set; }

    public ICollection<NguoiDungVaiTro> NguoiDungVaiTro { get; set; } = new List<NguoiDungVaiTro>();
}
