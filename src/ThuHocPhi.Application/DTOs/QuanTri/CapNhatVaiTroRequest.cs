using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ThuHocPhi.Application.DTOs.QuanTri;

public sealed class CapNhatVaiTroRequest
{
    [Required]
    public List<int> MaVaiTro { get; set; } = new();
}
