using ADay15.NET.Domain.Entities;
using ADay15.NET.Domain.Repositories;
using ADay15.NET.Infrastructure.DbContexts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ADay15.NET.Infrastructure.Repositories
{
    /// <summary>
    /// 用户角色仓储实现
    /// </summary>
    public class UserRoleRepository : RepositoryBase<UserRole>, IUserRoleRepository
    {
        public UserRoleRepository(AppDbContext dbContext) : base(dbContext)
        {
        }
    }
}
