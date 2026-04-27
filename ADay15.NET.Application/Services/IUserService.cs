using ADay15.NET.Application.DTOs;
using ADay15.NET.Domain.Entities;
using ADay15.NET.Infrastructure.Commons;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ADay15.NET.Application.Services
{
    public interface IUserService
    {
        Task<R<User>> GetUserByAccountAsync(string account);

        // 1. 根据ID查用户+角色
        Task<R<dynamic>> GetUserWithRolesAsync(long userId);

        // 2. 分页多条件查询用户列表
        Task<R<PageResult<User>>> GetUserPageListAsync(UserPageQueryDto dto);

        // 3. 新增用户 + 分配角色（事务）
        Task<R<string>> AddUserAsync(UserCreateDto dto);

        // 4. 修改用户 + 重置角色（事务）
        Task<R<string>> UpdateUserAsync(UserUpdateDto dto);

        // 5. 删除用户 + 删除关联（事务）
        Task<R<string>> DeleteUserAsync(long userId);

        // 6. 批量禁用用户
        Task<R<string>> BatchDisableUserAsync(long[] userIds);

        // 7. 登录校验
        Task<R<User>> LoginAsync(string account, string password);
    }
}
