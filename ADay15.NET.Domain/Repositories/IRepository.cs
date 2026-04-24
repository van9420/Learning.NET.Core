using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.Query;

namespace ADay15.NET.Domain.Repositories
{
    /// <summary>
    /// 泛型仓储接口
    /// </summary>
    public interface IRepository<T>
    {
        #region 1、基础 CRUD

        Task<T> FindByIdAsync(long id);
        Task<List<T>> GetAllAsync();
        Task AddAsync(T entity);
        Task AddRangeAsync(List<T> list);
        Task UpdateAsync(T entity);
        Task UpdateRangeAsync(List<T> list);
        Task DeleteAsync(T entity);
        Task DeleteRangeAsync(List<T> list);

        #endregion

        #region 2、条件查询

        Task<List<T>> GetListAsync(Expression<Func<T, bool>> where);
        Task<T> FirstOrDefaultAsync(Expression<Func<T, bool>> where);
        Task<bool> AnyAsync(Expression<Func<T, bool>> where);
        Task<long> CountAsync(Expression<Func<T, bool>> where);

        #endregion

        #region 3、分页
        Task<(long total, List<T> list)> GetPageListAsync(
            Expression<Func<T, bool>> where,
            int pageIndex, int pageSize,
            Expression<Func<T, object>> orderBy = null,
            bool isAsc = true);
        #endregion


        #region 4、批量高效操作（EF Core 7+）

        //【高性能】直接删
        Task<int> DeleteBatchAsync(Expression<Func<T, bool>> where);
        // 批量更新（正确版本）
        Task<int> UpdateBatchAsync( Expression<Func<T, bool>> where, Expression<Func<SetPropertyCalls<T>, SetPropertyCalls<T>>> setProperty);
        #endregion
    }
}
