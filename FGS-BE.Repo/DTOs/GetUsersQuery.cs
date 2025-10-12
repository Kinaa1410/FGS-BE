using FGS_BE.Repo.DTOs.Pages;
using FGS_BE.Repo.Entities;
using LinqKit;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace FGS_BE.Repo.DTOs;
public sealed record GetUsersQuery : PaginationRequest<User>
{
    public string? Search { get; set; }

    public override Expression<Func<User, bool>> GetExpressions()
    {
        if (!string.IsNullOrWhiteSpace(Search))
        {
            Search = Search.Trim();
            Expression = Expression
                .And(u => EF.Functions.Like(u.Phone, $"%{Search}%"))
                .Or(u => EF.Functions.Like(u.FullName, $"%{Search}%"))
                .Or(u => EF.Functions.Like(u.Email, $"%{Search}%"));
        }

        return Expression;
    }
}
