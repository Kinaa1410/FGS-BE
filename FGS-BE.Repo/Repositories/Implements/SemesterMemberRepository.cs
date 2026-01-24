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
    public class SemesterMemberRepository : GenericRepository<SemesterMember>, ISemesterMemberRepository
    {
        public SemesterMemberRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<int> CountUsersBySemesterAsync(
        int semesterId,
        CancellationToken cancellationToken = default)
        {
            return await Entities
                .AsNoTracking()
                .Where(x => x.SemesterId == semesterId)
                .Select(x => x.UserId)
                .Distinct()
                .CountAsync(cancellationToken);
        }
    }
}
