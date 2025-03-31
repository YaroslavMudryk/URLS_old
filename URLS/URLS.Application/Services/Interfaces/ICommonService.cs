using System.Linq.Expressions;

namespace URLS.Application.Services.Interfaces;

public interface ICommonService
{
    Task<(bool IsExist, List<TResult> Results)> IsExistWithResultsAsync<TResult>(Expression<Func<TResult, bool>> expression = null) where TResult : class;
    Task<bool> IsExistAsync<TResult>(Expression<Func<TResult, bool>> expression = null) where TResult : class;
    Task<int> CountAsync<TResult>(Expression<Func<TResult, bool>> expression = null) where TResult : class;
}
