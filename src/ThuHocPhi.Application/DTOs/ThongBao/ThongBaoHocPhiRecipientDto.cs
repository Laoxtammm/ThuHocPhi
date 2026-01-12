using System;

namespace ThuHocPhi.Application.DTOs.ThongBao;

public sealed class ThongBaoHocPhiRecipientDto
{
    public long Id { get; set; }
    public long MaThongBao { get; set; }
    public string MaSV { get; set; } = string.Empty;
    public byte TrangThaiDoc { get; set; }
    public byte TrangThaiThanhToan { get; set; }
    public DateTime? NgayGui { get; set; }
    public DateTime? NgayXem { get; set; }
    public string? GhiChu { get; set; }
}
