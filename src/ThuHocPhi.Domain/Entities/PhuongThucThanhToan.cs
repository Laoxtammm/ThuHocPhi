namespace ThuHocPhi.Domain.Entities;

public sealed class PhuongThucThanhToan
{
    public string MaPhuongThuc { get; set; } = string.Empty;
    public string TenPhuongThuc { get; set; } = string.Empty;
    public string? MoTa { get; set; }
    public bool TrangThai { get; set; }
}
