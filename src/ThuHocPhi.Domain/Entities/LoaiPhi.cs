namespace ThuHocPhi.Domain.Entities;

public sealed class LoaiPhi
{
    public string MaLoaiPhi { get; set; } = string.Empty;
    public string TenLoaiPhi { get; set; } = string.Empty;
    public string? DonViTinh { get; set; }
    public string? MoTa { get; set; }
    public bool TrangThai { get; set; }
}
