namespace ThuHocPhi.Application.DTOs.HocBong;

public sealed class HocBongDto
{
    public string MaHocBong { get; set; } = string.Empty;
    public string TenHocBong { get; set; } = string.Empty;
    public string LoaiGiaTri { get; set; } = string.Empty;
    public decimal GiaTri { get; set; }
    public string? NamHoc { get; set; }
    public string? MoTa { get; set; }
    public bool TrangThai { get; set; }
}
