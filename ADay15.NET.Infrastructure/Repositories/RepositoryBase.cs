using ADay15.NET.Domain.Repositories;
using ADay15.NET.Infrastructure.DbContexts;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace ADay15.NET.Infrastructure.Repositories
{
    /// <summary>
    /// 泛型仓储基类
    /// </summary>
    public class RepositoryBase<T> : IRepository<T> where T : class
    {

        protected readonly AppDbContext _dbContext;     // 数据库上下文
        protected readonly DbSet<T> _dbSet;             // 对应数据库中的一张表。

        // 构造方法注入，只在这里注入一次
        public RepositoryBase(AppDbContext dbContext)
        {
            _dbContext = dbContext;
            _dbSet = _dbContext.Set<T>();    // 自动获取 T 对应的表
        }

        #region 基础 CURD → **统一不在仓储 SaveChanges**
        public async Task AddAsync(T entity)
        {
            await _dbSet.AddAsync(entity);
            // ❌ 删掉 SaveChanges，UOW 统一提交
        }

        public async Task AddRangeAsync(List<T> list)
        {
            await _dbSet.AddRangeAsync(list);
        }

        public Task UpdateAsync(T entity)
        {
            _dbSet.Update(entity);
            return Task.CompletedTask;
        }

        public Task UpdateRangeAsync(List<T> list)
        {
            _dbSet.UpdateRange(list);
            return Task.CompletedTask;
        }

        public Task DeleteAsync(T entity)
        {
            _dbSet.Remove(entity);
            return Task.CompletedTask;
        }

        public Task DeleteRangeAsync(List<T> list)
        {
            _dbSet.RemoveRange(list);
            return Task.CompletedTask;
        }
        #endregion

        public async Task<bool> AnyAsync(Expression<Func<T, bool>> where)
        {
            return await _dbSet.AnyAsync(where);
        }

        public async Task<long> CountAsync(Expression<Func<T, bool>> where)
        {
            return await _dbSet.LongCountAsync(where);
        }

        public async Task<int> DeleteBatchAsync(Expression<Func<T, bool>> where)
        {
            return await _dbSet.Where(where).ExecuteDeleteAsync();
        }

        public async Task<T> FindByIdAsync(long id)
        {
            return await _dbSet.FindAsync(id);
        }

        public async Task<T> FirstOrDefaultAsync(Expression<Func<T, bool>> where)
        {
            return await _dbSet.FirstOrDefaultAsync(where);
        }

        public async Task<List<T>> GetAllAsync()
        {
            return await _dbSet.AsNoTracking().ToListAsync();
        }

        public async Task<List<T>> GetListAsync(Expression<Func<T, bool>> where)
        {
            return await _dbSet.AsNoTracking().Where(where).ToListAsync();
        }

        public async Task<(int total, List<T> list)> GetPageListAsync(
            Expression<Func<T, bool>> where,
            int pageIndex, int pageSize,
            Expression<Func<T, object>> orderBy = null,
            bool isAsc = true)
        {
            var query = _dbSet.AsNoTracking().Where(where);
            int total = await query.CountAsync();

            if (orderBy != null)
            {
                query = isAsc ? query.OrderBy(orderBy) : query.OrderByDescending(orderBy);
            }

            var list = await query
                .Skip((pageIndex - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return (total, list);
        }

        public async Task<int> UpdateBatchAsync(
            Expression<Func<T, bool>> where,
            Expression<Func<SetPropertyCalls<T>, SetPropertyCalls<T>>> setProperty)
        {
            return await _dbSet.Where(where).ExecuteUpdateAsync(setProperty);
        }
    }
}
