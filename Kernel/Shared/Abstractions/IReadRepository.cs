using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Abstractions
{
    public interface IReadRepository<T> where T : class
    {
        Task<T?> GetByIdAsync(int id, CancellationToken ct = default, bool asNoTracking = true);

        Task<bool> ExistsAsync(Expression<Func<T, bool>> predicate, CancellationToken ct = default);

        Task<long> CountAsync(Expression<Func<T, bool>>? predicate = null, CancellationToken ct = default);

        Task<IReadOnlyList<T>> ListAsync(
            Expression<Func<T, bool>>? filter = null,
            Func<IQueryable<T>, IOrderedQueryable<T>>? orderBy = null,
            int? skip = null,
            int? take = null,
            CancellationToken ct = default,
            bool asNoTracking = true);
    }
}
