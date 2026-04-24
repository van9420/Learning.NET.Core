
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
    public class UserService 
    {
        private readonly IUnitOfWork _unit;

        // 构造函数注入
        public UserService(IUnitOfWork unit)
        {
            _unit = unit;
        }

        public async Task<dynamic> GetUserWithRolesAsync(long userId)
        {
            return await _unit.UserRepository.GetUserWithRolesAsync(userId);

        }

        public async Task<User> GetUserByAccountAsync(string account)
        {
            return await _unit.UserRepository.GetUserByAccountAsync(account);

        }

        /// <summary>
        /// 添加用户
        /// 校验：账号不能为空,账号不能重复
        /// 业务逻辑：新增用户,给用户分配角色（批量插入 SysUserRole）,两步必须在同一个事务中
        /// </summary>
        /// <param name="dto">接收参数：账号、密码、姓名、状态、角色 Id 列表</param>
        /// <returns>统一格式 R<T></returns>
        public async Task<R<dynamic>> AddUserAsync(UserCreateDto dto) 
        {
            //先判断账号是否唯一
            // 【重要】不能用 AsNoTracking，因为要判断存在
            var exists = await _unit.UserRepository.AnyAsync(u => u.Account == dto.Account);
            if (exists)
                return R<dynamic>.Fail("账号已存在");

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

                    return R<dynamic>.Sucess(new { user.Id, user.Account, user.Name });
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
        public async Task<R<dynamic>> UpdUserAsync(UserUpdateDto dto) 
        {
            //查询用户是否存在
            // 【关键】修改必须 TRACKING，不能 AsNoTracking
            var user = await _unit.UserRepository.FirstOrDefaultAsync(u => u.Id == dto.Id);

            if (user == null)
                return R<dynamic>.Fail("用户不存在");
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
                        return R<dynamic>.Sucess("修改成功");
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
        public async Task<R<dynamic>> DelUserAsync(int userId) 
        {
            var user = await _unit.UserRepository.FirstOrDefaultAsync(u => u.Id == userId);
            if (user == null)
                return R<dynamic>.Fail("用户不存在");

            using (await _unit.BeginTransactionAsync()) 
            {
                try 
                {
                    // 高性能删除关联
                    await _unit.UserRoleRepository.DeleteBatchAsync(ur => ur.UserId == user.Id);

                    await _unit.UserRepository.DeleteAsync(user);

                    await _unit.CommitAsync();

                    return R<dynamic>.Sucess("删除成功");

                } catch (Exception ex) 
                {
                    await _unit.RollbackAsync();
                    throw new Exception("Transaction failed", ex);
                }

            }
        }


        /// <summary>
        /// 分页查询用户列表
        /// </summary>
        /// <param name="pageNum">页码</param>
        /// <param name="pageSize">页大小</param>
        /// <param name="account">账号（可选）</param>
        /// <param name="name">姓名（可选）</param>
        /// <returns>总数+用户列表</returns>
        public async Task<(long total, List<User> list)> GetUsers(int pageNum, int pageSize, string? account, string? name) 
        {
            // 1. 默认条件
            Expression<Func<User, bool>> where = u => true;

            // 2. 动态拼接
            if (!string.IsNullOrEmpty(account))
                where = where.And(u => u.Account.Contains(account));

            if (!string.IsNullOrEmpty(name))
                where = where.And(u => u.Name.Contains(name));

            // 3. 调用泛型仓储
            return await _unit.UserRepository.GetPageListAsync(where, pageNum, pageSize);
       
        }

        /// <summary>
        /// 6. 批量禁用用户
        /// 要求：使用 ExecuteUpdateAsync
        /// 逻辑：批量更新 Status = 0
        /// </summary>
        /// <param name="userIds">用户 ID 数组</param>
        /// <returns></returns>
        public async Task<R<dynamic>> ChangeStatus(long[] userIds)
        {
            var num= await _unit.UserRepository.UpdateBatchAsync(u => userIds.Contains(u.Id), x => x.SetProperty(u => u.Status, 0));
            
            return R<dynamic>.Sucess($"批量禁用{num}条成功");
        }
    }
}
