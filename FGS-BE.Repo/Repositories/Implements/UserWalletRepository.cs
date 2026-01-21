using FGS_BE.Repo.Data;
using FGS_BE.Repo.Entities;
using FGS_BE.Repo.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FGS_BE.Repo.Repositories.Implements
{
    public class UserWalletRepository : GenericRepository<UserWallet>, IUserWalletRepository
    {
        public UserWalletRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<UserWallet?> GetByUserIdAsync(int userId)
        {
            return await Entities
                .Include(w => w.PointTransactions)
                .FirstOrDefaultAsync(w => w.UserId == userId);
        }
    }
}
