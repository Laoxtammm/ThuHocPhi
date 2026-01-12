using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ThuHocPhi.Application.DTOs.ThanhToan;

namespace ThuHocPhi.Application.Interfaces.Finance;

public interface IThanhToanService
{
    Task<ThanhToanResult> ThanhToanAsync(ThanhToanRequest request, CancellationToken cancellationToken);
    Task<IReadOnlyList<GiaoDichDto>> GetGiaoDichAsync(string? maSv, string? maHocKy, CancellationToken cancellationToken);
}
