using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ThuHocPhi.Application.DTOs.HocBong;

namespace ThuHocPhi.Application.Interfaces.Finance;

public interface IHocBongService
{
    Task<IReadOnlyList<HocBongDto>> GetAllAsync(CancellationToken cancellationToken);
    Task<HocBongDto?> GetByIdAsync(string maHocBong, CancellationToken cancellationToken);
    Task<bool> CreateAsync(HocBongCreateRequest request, CancellationToken cancellationToken);
    Task<bool> UpdateAsync(string maHocBong, HocBongUpdateRequest request, CancellationToken cancellationToken);
    Task<bool> DeleteAsync(string maHocBong, CancellationToken cancellationToken);
    Task<bool> ApDungAsync(ApDungHocBongRequest request, CancellationToken cancellationToken);
}
