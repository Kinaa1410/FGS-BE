using FGS_BE.Repo.DTOs.Pages;
using FGS_BE.Repo.Entities;
using LinqKit;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace FGS_BE.Repo.DTOs.Achievements;
public sealed record GetAchievementsQuery : PaginationRequest<Achievement>
{
    public string? Search { get; set; }
    public string? ConditionType { get; set; }
    public bool? IsActive { get; set; }
    public int? PointsReward { get; set; }

    public override Expression<Func<Achievement, bool>> GetExpressions()
    {
        Expression = Expression.And(_ => string.IsNullOrWhiteSpace(Search) || EF.Functions.Like(_.Name, $"%{Search}%"));
        Expression = Expression.And(_ => string.IsNullOrWhiteSpace(ConditionType) || EF.Functions.Like(_.ConditionType, $"%{ConditionType}%"));
        Expression = Expression.And(_ => !IsActive.HasValue || _.IsActive == IsActive);
        Expression = Expression.And(_ => !PointsReward.HasValue || _.PointsReward == PointsReward);

        return Expression;
    }
}
