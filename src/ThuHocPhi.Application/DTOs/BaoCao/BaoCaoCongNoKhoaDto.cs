namespace ThuHocPhi.Application.DTOs.BaoCao;

public sealed class BaoCaoCongNoKhoaDto
{
    public string MaKhoa { get; set; } = string.Empty;
    public string TenKhoa { get; set; } = string.Empty;
    public int TongSoSinhVien { get; set; }
    public decimal TongPhaiThu { get; set; }
    public decimal TongMienGiam { get; set; }
    public decimal TongDaThu { get; set; }
    public decimal TongConNo { get; set; }
    public int SoSVDaThanhToan { get; set; }
    public int SoSVQuaHan { get; set; }
}
