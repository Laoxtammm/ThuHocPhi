using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace ThuHocPhi.Application.Interfaces.Repositories;

public interface IRepository<T> where T : class
{
    Task<T?> GetByIdAsync(object id, CancellationToken cancellationToken);
    Task<IReadOnlyList<T>> ListAsync(CancellationToken cancellationToken);
    Task AddAsync(T entity, CancellationToken cancellationToken);
    void Update(T entity);
    void Delete(T entity);
    Task<int> SaveChangesAsync(CancellationToken cancellationToken);
}
