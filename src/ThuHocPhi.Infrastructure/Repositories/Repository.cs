using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using ThuHocPhi.Application.Interfaces.Repositories;
using ThuHocPhi.Infrastructure.Data;

namespace ThuHocPhi.Infrastructure.Repositories;

public sealed class Repository<T> : IRepository<T> where T : class
{
    private readonly ThuHocPhiDbContext _dbContext;

    public Repository(ThuHocPhiDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public Task<T?> GetByIdAsync(object id, CancellationToken cancellationToken)
    {
        return _dbContext.Set<T>().FindAsync(new[] { id }, cancellationToken).AsTask();
    }

    public async Task<IReadOnlyList<T>> ListAsync(CancellationToken cancellationToken)
    {
        return await _dbContext.Set<T>().AsNoTracking().ToListAsync(cancellationToken);
    }

    public async Task AddAsync(T entity, CancellationToken cancellationToken)
    {
        await _dbContext.Set<T>().AddAsync(entity, cancellationToken);
    }

    public void Update(T entity)
    {
        _dbContext.Set<T>().Update(entity);
    }

    public void Delete(T entity)
    {
        _dbContext.Set<T>().Remove(entity);
    }

    public Task<int> SaveChangesAsync(CancellationToken cancellationToken)
    {
        return _dbContext.SaveChangesAsync(cancellationToken);
    }
}
