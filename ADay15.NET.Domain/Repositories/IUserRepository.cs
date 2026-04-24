using ADay15.NET.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ADay15.NET.Domain.Repositories
{
    /// <summary>
    /// 用户仓储接口
    /// </summary>
    public interface IUserRepository : IRepository<User>
    {
        /// <summary>
        /// 根据用户ID获取用户的信息以及角色列表信息
        /// </summary>
        /// <param name="userId">用户ID</param>
        /// <returns>用户的信息以及角色列表信息</returns>
        Task<object> GetUserWithRolesAsync(long userId);

        /// <summary>
        /// 根据Account获取用户信息
        /// </summary>
        /// <param name="account">账号</param>
        /// <returns>用户信息</returns>
        Task<User> GetUserByAccountAsync(string account);

    }
}
