namespace ThuHocPhi.Application.DTOs.CongNo;

public sealed class CongNoChiTietDto
{
    public long MaCongNoCT { get; set; }
    public long MaCongNo { get; set; }
    public string LoaiDong { get; set; } = string.Empty;
    public long? RefId { get; set; }
    public string MoTa { get; set; } = string.Empty;
    public decimal SoLuong { get; set; }
    public decimal DonGia { get; set; }
    public decimal ThanhTien { get; set; }
    public decimal MienGiam { get; set; }
    public decimal PhaiThu { get; set; }
}
