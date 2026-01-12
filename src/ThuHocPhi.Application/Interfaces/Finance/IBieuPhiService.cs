using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ThuHocPhi.Application.DTOs.BieuPhi;

namespace ThuHocPhi.Application.Interfaces.Finance;

public interface IBieuPhiService
{
    Task<IReadOnlyList<BieuPhiDto>> GetAllAsync(string? maHocKy, CancellationToken cancellationToken);
    Task<BieuPhiDto?> GetByIdAsync(long id, CancellationToken cancellationToken);
    Task<long> CreateAsync(BieuPhiCreateRequest request, CancellationToken cancellationToken);
    Task<bool> UpdateAsync(long id, BieuPhiUpdateRequest request, CancellationToken cancellationToken);
    Task<bool> DeleteAsync(long id, CancellationToken cancellationToken);
}
