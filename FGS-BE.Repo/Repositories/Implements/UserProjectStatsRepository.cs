using FGS_BE.Repo.Data;
using FGS_BE.Repo.Entities;
using FGS_BE.Repo.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace FGS_BE.Repo.Repositories.Implements
{
    public class UserProjectStatsRepository : GenericRepository<UserProjectStats>, IUserProjectStatsRepository
    {
        public UserProjectStatsRepository(ApplicationDbContext context) : base(context) { }

        public async Task<UserProjectStats?> FindByAsync(Expression<Func<UserProjectStats, bool>> predicate, CancellationToken cancellationToken = default)
        {
            return await Entities.FirstOrDefaultAsync(predicate, cancellationToken);
        }
    }
}