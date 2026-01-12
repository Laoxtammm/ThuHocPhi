using System.Threading;
using System.Threading.Tasks;
using ThuHocPhi.Application.DTOs.Auth;

namespace ThuHocPhi.Application.Interfaces.Security;

public interface IAuthService
{
    Task<LoginResponse?> LoginAsync(LoginRequest request, CancellationToken cancellationToken);
}
