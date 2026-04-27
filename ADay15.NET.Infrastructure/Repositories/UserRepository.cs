using ADay15.NET.Domain.Entities;
using ADay15.NET.Domain.Repositories;
using ADay15.NET.Infrastructure.DbContexts;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace ADay15.NET.Infrastructure.Repositories
{
    /// <summary>
    /// 用户仓储实现
    /// </summary>
    public class UserRepository : RepositoryBase<User>, IUserRepository
    {
        // 只需要这一个构造，把 dbContext 传给基类就行，自定义复杂查询可以直接用 _dbContext.Users（更方便）
        public UserRepository(AppDbContext dbContext) : base(dbContext)
        {

        }
        
        public async Task<object> GetUserWithRolesAsync(long userId)
        {
            // 这里直接用 _dbSet / _dbContext
            var user = await _dbContext.Users
            .AsNoTracking()
            .Include(u => u.UserRoles)
                .ThenInclude(ur => ur.Role)
            .Where(u => u.Id == userId)
            .Select(u => new
            {
                u.Id,
                u.Account,
                u.Name,
                Roles = u.UserRoles.Select(ur => new
                {
                    ur.Role.Id,
                    ur.Role.RoleName,
                    ur.Role.RoleCode
                }).ToList()
            })
            .FirstOrDefaultAsync();

            return user;
        }


        
        public async Task<User> GetUserByAccountAsync(string account)
        {
            var user = await _dbContext.Users
                .AsNoTracking()
                .Where(u => u.Account == account)
                .Select(u => new User { Id = u.Id, Name = u.Name, Account = u.Account, Status = u.Status, CreateTime = u.CreateTime })
                .FirstOrDefaultAsync();
            return user;
        }


        public async Task<User> LoginAsync(string account, string password)
        {
            var user = await _dbContext.Users
                .AsNoTracking()
                .Where(u => u.Account == account && u.Password == password)
                .Select(u => new User { Id = u.Id, Name = u.Name, Account = u.Account, Status = u.Status, CreateTime = u.CreateTime })
                .FirstOrDefaultAsync();
            return user;
        }
    }
}
