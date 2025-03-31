using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using URLS.Application.Services.Interfaces;
using URLS.Infrastructure.Data.Context;

namespace URLS.Application.Services.Implementations;

public class CommonService(URLSDbContext db) : ICommonService
{
    public async Task<int> CountAsync<TResult>(Expression<Func<TResult, bool>> expression = null) where TResult : class
    {
        if (expression == null)
            return await db.Set<TResult>().CountAsync();
        return await db.Set<TResult>().CountAsync(expression);
    }

    public async Task<bool> IsExistAsync<TResult>(Expression<Func<TResult, bool>> expression = null) where TResult : class
    {
        if(expression == null)
            return await db.Set<TResult>().AnyAsync();
        return await db.Set<TResult>().AnyAsync(expression);
    }

    public async Task<(bool, List<TResult>)> IsExistWithResultsAsync<TResult>(Expression<Func<TResult, bool>> expression = null) where TResult : class
    {
        if (expression == null)
        {
            var allItems = await db.Set<TResult>().AsNoTracking().ToListAsync();
            return (allItems.Count > 0, allItems);
        }
        var items = await db.Set<TResult>().AsNoTracking().Where(expression).ToListAsync();
        return (items.Count > 0, items);
    }
}
