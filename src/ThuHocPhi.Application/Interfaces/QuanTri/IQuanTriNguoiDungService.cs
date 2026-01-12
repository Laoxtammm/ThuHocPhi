using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ThuHocPhi.Application.DTOs.QuanTri;

namespace ThuHocPhi.Application.Interfaces.QuanTri;

public interface IQuanTriNguoiDungService
{
    Task<IReadOnlyList<NguoiDungDto>> GetAllAsync(CancellationToken cancellationToken);
    Task<NguoiDungDto?> GetByIdAsync(int id, CancellationToken cancellationToken);
    Task<int> CreateAsync(NguoiDungCreateRequest request, CancellationToken cancellationToken);
    Task<bool> UpdateAsync(int id, NguoiDungUpdateRequest request, CancellationToken cancellationToken);
    Task<bool> ResetPasswordAsync(int id, ResetPasswordRequest request, CancellationToken cancellationToken);
    Task<bool> UpdateRolesAsync(int id, CapNhatVaiTroRequest request, CancellationToken cancellationToken);
    Task<bool> SetActiveAsync(int id, bool isActive, CancellationToken cancellationToken);
    Task<IReadOnlyList<VaiTroDto>> GetRolesAsync(CancellationToken cancellationToken);
}
