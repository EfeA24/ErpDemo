using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Abstractions
{
    public interface IWriteRepository<T, TKey> where T : class
    {
        Task AddAsync(T entity, CancellationToken ct = default);
        Task AddRangeAsync(IEnumerable<T> entities, CancellationToken ct = default);

        void Update(T entity);
        void UpdateRange(IEnumerable<T> entities);

        Task SoftDeleteAsync(T entity, CancellationToken ct = default);
        Task SoftDeleteByIdAsync(TKey id, CancellationToken ct = default);

        Task HardDeleteAsync(T entity, CancellationToken ct = default);
        Task HardDeleteByIdAsync(TKey id, CancellationToken ct = default);

        Task<int> SaveChangesAsync(CancellationToken ct = default);
    }
}
