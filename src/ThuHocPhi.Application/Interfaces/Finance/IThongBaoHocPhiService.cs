using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ThuHocPhi.Application.DTOs.ThongBao;

namespace ThuHocPhi.Application.Interfaces.Finance;

public interface IThongBaoHocPhiService
{
    Task<IReadOnlyList<ThongBaoHocPhiDto>> GetAllAsync(CancellationToken cancellationToken);
    Task<ThongBaoHocPhiDto?> GetByIdAsync(long id, CancellationToken cancellationToken);
    Task<long> CreateAsync(ThongBaoHocPhiCreateRequest request, CancellationToken cancellationToken);
    Task<bool> UpdateAsync(long id, ThongBaoHocPhiUpdateRequest request, CancellationToken cancellationToken);
    Task<bool> DeleteAsync(long id, CancellationToken cancellationToken);
    Task<IReadOnlyList<ThongBaoHocPhiRecipientDto>> GetRecipientsAsync(long maThongBao, CancellationToken cancellationToken);
    Task<int> AssignRecipientsAsync(long maThongBao, ThongBaoHocPhiAssignRequest request, CancellationToken cancellationToken);
}
