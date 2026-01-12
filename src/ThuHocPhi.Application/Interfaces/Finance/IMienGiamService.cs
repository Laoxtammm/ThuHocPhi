using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ThuHocPhi.Application.DTOs.MienGiam;

namespace ThuHocPhi.Application.Interfaces.Finance;

public interface IMienGiamService
{
    Task<IReadOnlyList<ChinhSachMienGiamDto>> GetAllAsync(CancellationToken cancellationToken);
    Task<ChinhSachMienGiamDto?> GetByIdAsync(string maChinhSach, CancellationToken cancellationToken);
    Task<bool> CreateAsync(ChinhSachMienGiamCreateRequest request, CancellationToken cancellationToken);
    Task<bool> UpdateAsync(string maChinhSach, ChinhSachMienGiamUpdateRequest request, CancellationToken cancellationToken);
    Task<bool> DeleteAsync(string maChinhSach, CancellationToken cancellationToken);
    Task<bool> ApDungAsync(ApDungMienGiamRequest request, CancellationToken cancellationToken);
}
