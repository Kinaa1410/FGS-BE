using FGS_BE.Repo.Entities;
using System.Linq.Expressions;

namespace FGS_BE.Repo.Repositories.Interfaces
{
    public interface IUserProjectStatsRepository : IGenericRepository<UserProjectStats>
    {
        Task<UserProjectStats?> FindByAsync(Expression<Func<UserProjectStats, bool>> predicate, CancellationToken cancellationToken = default);
    }
}