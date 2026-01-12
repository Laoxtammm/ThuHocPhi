using System.Threading;
using System.Threading.Tasks;
using ThuHocPhi.Domain.Entities;

namespace ThuHocPhi.Application.Interfaces.Repositories;

public interface IUserRepository
{
    Task<NguoiDung?> GetByUsernameAsync(string username, CancellationToken cancellationToken);
    Task<NguoiDung?> GetByIdAsync(int userId, CancellationToken cancellationToken);
    Task UpdateAsync(NguoiDung user, CancellationToken cancellationToken);
}
