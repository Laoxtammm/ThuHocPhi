namespace ThuHocPhi.Domain.Entities;

public sealed class HocPhan
{
    public string MaHocPhan { get; set; } = string.Empty;
    public string TenHocPhan { get; set; } = string.Empty;
    public int SoTinChi { get; set; }
    public int? SoTietLyThuyet { get; set; }
    public int? SoTietThucHanh { get; set; }
    public string MaKhoa { get; set; } = string.Empty;
    public string? LoaiHocPhan { get; set; }
    public string? MoTa { get; set; }
    public bool TrangThai { get; set; }
}
