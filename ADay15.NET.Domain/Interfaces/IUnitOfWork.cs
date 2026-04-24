using ADay15.NET.Domain.Repositories;
using Microsoft.EntityFrameworkCore.Storage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ADay15.NET.Domain.Interfaces
{
    /// <summary>
    /// 工作单元:
    ///     1、统一管理事务
    ///     2、统一管理多个仓储
    ///     3、统一提交 SaveChanges
    ///     4、跨仓储事务（用户 + 角色 + 日志一起成功 / 失败）
    /// </summary>
    public interface IUnitOfWork
    {
        IUserRepository UserRepository { get; }
        //IRoleRepository RoleRepository { get; }
        IUserRoleRepository UserRoleRepository { get; }

        /// <summary>
        /// 开始事务
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<IDbContextTransaction> BeginTransactionAsync(CancellationToken cancellationToken = default);

        /// <summary>
        /// 单独 提交SaveChanges（把标记变成 SQL，发送到数据库事务里）
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task SaveChangesAsync(CancellationToken cancellationToken = default);


        /// <summary>
        /// 提交 SaveChanges和事务
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task CommitAsync(CancellationToken cancellationToken = default);
        
        /// <summary>
        /// 回滚事务
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task RollbackAsync(CancellationToken cancellationToken = default);
    }
}
