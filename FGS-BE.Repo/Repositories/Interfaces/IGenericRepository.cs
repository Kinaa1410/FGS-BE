using FGS_BE.Repo.DTOs.Pages;
using System.Linq.Expressions;

namespace FGS_BE.Repo.Repositories.Interfaces;
public interface IGenericRepository<T> where T : class
{
    Task<bool> ExistsByAsync(
        Expression<Func<T, bool>>? expression = null,
        CancellationToken cancellationToken = default);

    Task<T?> FindByIdAsync(object id, CancellationToken cancellationToken = default);

    IQueryable<T> Entities { get; }

    Task<T?> FindByAsync(
        Expression<Func<T, bool>> expression,
        Func<IQueryable<T>, IQueryable<T>>? includeFunc = null,
        CancellationToken cancellationToken = default);

    Task<IList<T>> FindAsync(
        Expression<Func<T, bool>>? expression = null,
        Func<IQueryable<T>, IOrderedQueryable<T>>? orderBy = null,
        bool isAsNoTracking = true,
        CancellationToken cancellationToken = default);

    Task<TDTO?> FindByAsync<TDTO>(
        Expression<Func<T, bool>> expression,
        CancellationToken cancellationToken = default) where TDTO : class;

    Task<IList<TDTO>> FindAsync<TDTO>(
        Expression<Func<T, bool>>? expression = null,
        Func<IQueryable<T>, IOrderedQueryable<T>>? orderBy = null,
        CancellationToken cancellationToken = default) where TDTO : class;

    Task<PaginatedList<TDTO>> FindAsync<TDTO>(
        int pageIndex,
        int pageSize,
        Expression<Func<T, bool>>? expression = null,
        Func<IQueryable<T>, IOrderedQueryable<T>>? orderBy = null,
        CancellationToken cancellationToken = default) where TDTO : class;

    Task<int> CountAsync(
        Expression<Func<T, bool>>? expression = null,
        CancellationToken cancellationToken = default);

    Task UpdateAsync(T entity);
    Task CreateAsync(T entity, CancellationToken cancellationToken = default);

    Task DeleteAsync(T entity);

}