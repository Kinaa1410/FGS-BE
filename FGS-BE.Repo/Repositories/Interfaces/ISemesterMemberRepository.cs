using FGS_BE.Repo.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FGS_BE.Repo.Repositories.Interfaces
{
    public interface ISemesterMemberRepository : IGenericRepository<SemesterMember>
    {
        Task<int> CountUsersBySemesterAsync(
        int semesterId,
        CancellationToken cancellationToken = default);
    }
}
