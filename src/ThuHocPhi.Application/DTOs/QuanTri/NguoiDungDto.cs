using System;
using System.Collections.Generic;

namespace ThuHocPhi.Application.DTOs.QuanTri;

public sealed class NguoiDungDto
{
    public int MaNguoiDung { get; set; }
    public string TenDangNhap { get; set; } = string.Empty;
    public string HoTen { get; set; } = string.Empty;
    public string? Email { get; set; }
    public string? SoDienThoai { get; set; }
    public DateTime? LanDangNhapCuoi { get; set; }
    public bool TrangThai { get; set; }
    public DateTime NgayTao { get; set; }
    public IReadOnlyList<string> VaiTro { get; set; } = new List<string>();
}
