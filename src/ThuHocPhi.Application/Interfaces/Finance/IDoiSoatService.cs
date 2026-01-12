using System.Threading;
using System.Threading.Tasks;
using ThuHocPhi.Application.DTOs.DoiSoat;

namespace ThuHocPhi.Application.Interfaces.Finance;

public interface IDoiSoatService
{
    Task<DoiSoatResultDto> DoiSoatAsync(DoiSoatRequest request, CancellationToken cancellationToken);
    Task<DoiSoatResultDto?> GetByIdAsync(long maDoiSoat, CancellationToken cancellationToken);
}
