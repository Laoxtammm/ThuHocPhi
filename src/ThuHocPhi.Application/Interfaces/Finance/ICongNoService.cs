using System.Threading;
using System.Threading.Tasks;
using ThuHocPhi.Application.DTOs.CongNo;

namespace ThuHocPhi.Application.Interfaces.Finance;

public interface ICongNoService
{
    Task<CongNoTinhResult> TinhCongNoAsync(CongNoTinhRequest request, CancellationToken cancellationToken);
    Task<CongNoDto?> GetCongNoAsync(string maSv, string maHocKy, CancellationToken cancellationToken);
}
