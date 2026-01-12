namespace ThuHocPhi.Domain.Entities;

public sealed class NguoiDungVaiTro
{
    public int MaNguoiDung { get; set; }
    public int MaVaiTro { get; set; }

    public NguoiDung NguoiDung { get; set; } = null!;
    public VaiTro VaiTro { get; set; } = null!;
}
