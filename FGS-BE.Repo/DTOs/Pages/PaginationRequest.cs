using LinqKit;
using System.Linq.Dynamic.Core;
using System.Linq.Expressions;

namespace FGS_BE.Repo.DTOs.Pages;
public abstract record PaginationRequest<T> where T : class
{
    private const string Id = nameof(Id);
    private int _pageIndex = PaginationConstants.DefaultPageIndex;
    private int _pageSize = PaginationConstants.DefaultPageSize;

    public int PageIndex
    {
        get => _pageIndex;
        set => _pageIndex = value > 0
            ? value
            : PaginationConstants.DefaultPageIndex;
    }

    public int PageSize
    {
        get => _pageSize;
        set => _pageSize = value > 0 && value <= PaginationConstants.MaxPageSize
            ? value
            : PaginationConstants.DefaultPageSize;
    }

    public string? SortColumn { get; set; } = Id;

    public SortDirection? SortDir { get; set; } = SortDirection.Asc;

    protected Expression<Func<T, bool>> Expression = PredicateBuilder.New<T>(true);

    public abstract Expression<Func<T, bool>> GetExpressions();

    public Func<IQueryable<T>, IOrderedQueryable<T>>? GetOrder()
    {
        if (string.IsNullOrWhiteSpace(SortColumn)) return null;

        return query => query.OrderBy($"{SortColumn} {SortDir}");
    }

    public string? GetDynamicOrder()
    {
        if (string.IsNullOrWhiteSpace(SortColumn)) return null;

        return $"{SortColumn} {SortDir}";
    }

}