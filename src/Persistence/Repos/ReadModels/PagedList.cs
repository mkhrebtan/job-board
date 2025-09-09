using Domain.ReadModels;
using Microsoft.EntityFrameworkCore;
using System.Threading;

namespace Persistence.Repos.ReadModels;

public class PagedList<T> : IPagedList<T>
{
    private PagedList(List<T> items, int page, int pageSize, int totalCount)
    {
        Items = items;
        Page = page;
        PageSize = pageSize;
        TotalCount = totalCount;
    }

    public PagedList()
    {
        Items = new List<T>();
        Page = 1;
        PageSize = 10;
        TotalCount = 0;
    }

    public List<T> Items { get; }

    public int Page { get; }

    public int PageSize { get; }

    public int TotalCount { get; }

    public bool HasNextPage => Page * PageSize < TotalCount;

    public bool HasPreviousPage => Page > 1;

    public async Task<IPagedList<T>> Create(IQueryable<T> query, int page, int pageSize)
    {
        if (page <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(page), "Page number must be greater than zero.");
        }

        if (pageSize <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(pageSize), "Page size must be greater than zero.");
        }

        var totalCount = await query.CountAsync();
        var items = await query.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();

        return new PagedList<T>(items, page, pageSize, totalCount);
    }
}
