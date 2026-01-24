using FGS_BE.Repo.DTOs.Pages;
using FGS_BE.Repo.Entities;
using FGS_BE.Repo.Enums;
using LinqKit;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace FGS_BE.Repo.DTOs.Users;
public sealed record GetUsersQuery : PaginationRequest<User>
{

    /// <summary>
    /// The user's email address which acts as a user name.
    /// </summary>
    public string? Search { get; set; }

    public RoleEnums? Role { get; set; }

    public override Expression<Func<User, bool>> GetExpressions()
    {
        if (!string.IsNullOrWhiteSpace(Search))
        {
            Search = Search.Trim();
            Expression = Expression
                .And(u => EF.Functions.Like(u.PhoneNumber, $"%{Search}%"))
                .Or(u => EF.Functions.Like(u.FullName, $"%{Search}%"))
                .Or(u => EF.Functions.Like(u.Email, $"%{Search}%"));
        }
        Expression = Expression.And(u => !Role.HasValue || u.UserRoles.Any(ur => ur.Role.Name == Role.ToString()));

        return Expression;
    }
}
