using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using ThuHocPhi.Application.Interfaces.Repositories;
using ThuHocPhi.Domain.Entities;
using ThuHocPhi.Infrastructure.Data;

namespace ThuHocPhi.Infrastructure.Repositories;

public sealed class UserRepository : IUserRepository
{
    private readonly ThuHocPhiDbContext _dbContext;

    public UserRepository(ThuHocPhiDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public Task<NguoiDung?> GetByUsernameAsync(string username, CancellationToken cancellationToken)
    {
        return _dbContext.NguoiDung
            .Include(x => x.NguoiDungVaiTro)
            .ThenInclude(x => x.VaiTro)
            .FirstOrDefaultAsync(x => x.TenDangNhap == username, cancellationToken);
    }

    public Task<NguoiDung?> GetByIdAsync(int userId, CancellationToken cancellationToken)
    {
        return _dbContext.NguoiDung
            .Include(x => x.NguoiDungVaiTro)
            .ThenInclude(x => x.VaiTro)
            .FirstOrDefaultAsync(x => x.MaNguoiDung == userId, cancellationToken);
    }

    public async Task UpdateAsync(NguoiDung user, CancellationToken cancellationToken)
    {
        _dbContext.NguoiDung.Update(user);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }
}
