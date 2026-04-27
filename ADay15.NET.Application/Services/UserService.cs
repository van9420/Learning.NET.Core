
using ADay15.NET.Application.DTOs;
using ADay15.NET.Domain.Entities;
using ADay15.NET.Domain.Extensions;
using ADay15.NET.Domain.Interfaces;
using ADay15.NET.Domain.Repositories;
using ADay15.NET.Infrastructure.Commons;
using ADay15.NET.Infrastructure.DbContexts;
using ADay15.NET.Infrastructure.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace ADay15.NET.Application.Services
{
    public class UserService:IUserService
    {
        private readonly IUnitOfWork _unit;

        // 构造函数注入
        public UserService(IUnitOfWork unit)
        {
            _unit = unit;
        }


        /// <summary>
        /// 根据Account获取用户信息
        /// </summary>
        /// <param name="account">账号</param>
        /// <returns>用户信息</returns>
        public async Task<R<User>> GetUserByAccountAsync(string account)
        {
            var user= await _unit.UserRepository.GetUserByAccountAsync(account);
            R<User> res = user != null ? R<User>.Sucess(user) : R<User>.Fail("用户不存在");
            return res;
        }        

        /// <summary>
        /// 根据ID查用户+角色
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public async Task<R<dynamic>> GetUserWithRolesAsync(long userId)
        {
            var data = await _unit.UserRepository.GetUserWithRolesAsync(userId);
            R<dynamic> res = data != null ? R<dynamic>.Sucess(data) : R<dynamic>.Fail("用户不存在");

            return res;
        }

        /// <summary>
        /// 分页多条件查询用户列表
        /// </summary>
        /// <param name="dto">分页查询用户列表的DTO</param>
        /// <returns>总数+用户列表</returns>
        public async Task<R<PageResult<User>>> GetUserPageListAsync(UserPageQueryDto dto)
        {
            // 1. 默认条件
            Expression<Func<User, bool>> where = u => true;

            // 2. 动态拼接
            if (!string.IsNullOrEmpty(dto.Account))
                where = where.And(u => u.Account.Contains(dto.Account));

            if (!string.IsNullOrEmpty(dto.Name))
                where = where.And(u => u.Name.Contains(dto.Name));

            // 3. 调用泛型仓储
            (int total, List<User> list) data = await _unit.UserRepository.GetPageListAsync(where, dto.PageNum, dto.PageSize);

            return R<PageResult<User>>.Sucess(new PageResult<User>
            {
                Items = data.list,
                TotalCount = data.total,
                PageNumber = dto.PageNum,
                PageSize = dto.PageSize
            });
        }

        /// <summary>
        /// 添加用户
        /// 校验：账号不能为空,账号不能重复
        /// 业务逻辑：新增用户,给用户分配角色（批量插入 SysUserRole）,两步必须在同一个事务中
        /// </summary>
        /// <param name="dto">接收参数：账号、密码、姓名、状态、角色 Id 列表</param>
        /// <returns>统一格式 R<T></returns>
        public async Task<R<string>> AddUserAsync(UserCreateDto dto)
        {
            //先判断账号是否唯一
            // 【重要】不能用 AsNoTracking，因为要判断存在
            var exists = await _unit.UserRepository.AnyAsync(u => u.Account == dto.Account);
            if (exists)
                return R<string>.Fail("账号已存在");

            //事务
            using (await _unit.BeginTransactionAsync())
            {
                try
                {
                    // 执行第一个数据库操作:新增用户
                    User user = new User();
                    user.Account = dto.Account;
                    user.Name = dto.Name;
                    user.Password = dto.Password;// 未来必须加密，这里先保留
                    user.Status = dto.Status;

                    await _unit.UserRepository.AddAsync(user);

                    // -----------------------
                    // 【关键】在这里 SaveChanges 拿 ID，完全安全！
                    // -----------------------
                    await _unit.SaveChangesAsync();   // 这个方法内部会 SaveChanges

                    // 执行第二个数据库操作: 给用户分配角色（批量插入 SysUserRole）
                    // 批量分配角色
                    var userRoles = dto.RoleId.Select(roleId => new UserRole
                    {
                        UserId = user.Id,
                        RoleId = roleId
                    }).ToList();

                    await _unit.UserRoleRepository.AddRangeAsync(userRoles);

                    // ✅ 统一提交
                    await _unit.CommitAsync();

                    return R<string>.Sucess("用户添加成功");
                }
                catch (Exception ex)
                {
                    // 发生异常时回滚事务
                    await _unit.RollbackAsync();
                    // 可以选择抛出异常或进行其他错误处理
                    throw; // 或者处理异常，例如：throw new Exception("Transaction failed", ex);
                }
            }
        }

        /// <summary>
        /// 更新用户 
        /// 逻辑：查询用户是否存在，更新用户基础信息，删除该用户原有角色关系，新增新的角色关系，全部操作必须事务保证
        /// </summary>
        /// <param name="dto">参数：用户 ID、姓名、状态、角色 Id 列表</param>
        /// <returns></returns>
        public async Task<R<string>> UpdateUserAsync(UserUpdateDto dto)
        {
            //查询用户是否存在
            // 【关键】修改必须 TRACKING，不能 AsNoTracking
            var user = await _unit.UserRepository.FirstOrDefaultAsync(u => u.Id == dto.Id);

            if (user == null)
                return R<string>.Fail("用户不存在");
            else
            {
                using (await _unit.BeginTransactionAsync())
                {
                    try
                    {
                        // 执行第一个数据库操作:更新用户基础信息
                        user.Name = dto.Name ?? user.Name;
                        user.Status = dto.Status ?? user.Status;

                        await _unit.UserRepository.UpdateAsync(user);

                        // 【高性能】直接删，不加载数据
                        await _unit.UserRoleRepository.DeleteBatchAsync(ur => ur.UserId == user.Id);

                        // 新增新角色
                        var userRoles = dto.RoleId.Select(roleId => new UserRole
                        {
                            UserId = user.Id,
                            RoleId = roleId
                        }).ToList();

                        await _unit.UserRoleRepository.AddRangeAsync(userRoles);

                        await _unit.CommitAsync();
                        return R<string>.Sucess("修改成功");
                    }
                    catch (Exception ex)
                    {
                        // 发生异常时回滚事务
                        await _unit.RollbackAsync();
                        // 可以选择抛出异常或进行其他错误处理
                        throw; // 或者处理异常，例如：throw new Exception("Transaction failed", ex);
                    }
                }
            }
        }

        /// <summary>
        /// 删除用户
        /// 逻辑：判断用户是否存在,开启事务,删除用户角色关联记录,删除用户,提交事务
        /// </summary>
        /// <param name="userId">用户 ID</param>
        /// <returns></returns>
        public async Task<R<string>> DeleteUserAsync(long userId)
        {
            var user = await _unit.UserRepository.FirstOrDefaultAsync(u => u.Id == userId);
            if (user == null)
                return R<string>.Fail("用户不存在");

            using (await _unit.BeginTransactionAsync())
            {
                try
                {
                    // 高性能删除关联
                    await _unit.UserRoleRepository.DeleteBatchAsync(ur => ur.UserId == user.Id);

                    await _unit.UserRepository.DeleteAsync(user);

                    await _unit.CommitAsync();

                    return R<string>.Sucess("删除成功");

                }
                catch (Exception ex)
                {
                    await _unit.RollbackAsync();
                    throw new Exception("Transaction failed", ex);
                }

            }
        }


        /// <summary>
        /// 6. 批量禁用用户
        /// 要求：使用 ExecuteUpdateAsync
        /// 逻辑：批量更新 Status = 0
        /// </summary>
        /// <param name="userIds">用户 ID 数组</param>
        /// <returns></returns>
        public async Task<R<string>> BatchDisableUserAsync(long[] userIds)
        {
            var num = await _unit.UserRepository.UpdateBatchAsync(u => userIds.Contains(u.Id), x => x.SetProperty(u => u.Status, 0));

            return R<string>.Sucess($"批量禁用{num}条成功");
        }


        /// <summary>
        /// 用户登录
        /// </summary>
        /// <param name="account">账号</param>
        /// <param name="password">密码</param>
        /// <returns></returns>
        public async Task<R<User>> LoginAsync(string account, string password)
        {
            if (string.IsNullOrEmpty(account) || string.IsNullOrEmpty(password))
                return R<User>.Fail("账户或密码不能为空！");

            var user= await _unit.UserRepository.LoginAsync(account, password);

            R<User> res = user != null ? R<User>.Sucess(user) : R<User>.Fail("账户或密码不正确！");

            return res;

        }
    }
}
