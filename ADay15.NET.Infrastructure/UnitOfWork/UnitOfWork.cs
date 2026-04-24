using ADay15.NET.Domain.Interfaces;
using ADay15.NET.Domain.Repositories;
using ADay15.NET.Infrastructure.DbContexts;
using Microsoft.EntityFrameworkCore.Storage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ADay15.NET.Infrastructure.UnitOfWork
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly AppDbContext _dbContext;
        private IDbContextTransaction _transaction;

        public IUserRepository UserRepository { get; }
        public IUserRoleRepository UserRoleRepository { get; }


        public UnitOfWork(AppDbContext dbContext,
                      IUserRepository userRepository,
                      IUserRoleRepository userRoleRepository)
        {
            _dbContext = dbContext;
            UserRepository = userRepository;
            UserRoleRepository = userRoleRepository;
        }


        public async Task<IDbContextTransaction> BeginTransactionAsync(CancellationToken cancellationToken = default)
        {
            _transaction = await _dbContext.Database.BeginTransactionAsync(cancellationToken);
            return _transaction;
        }

        public async Task SaveChangesAsync(CancellationToken cancellationToken = default) 
        {
            await _dbContext.SaveChangesAsync(cancellationToken);
        }

        public async Task CommitAsync(CancellationToken cancellationToken = default)
        {
            await _dbContext.SaveChangesAsync(cancellationToken);
            if (_transaction != null)
                await _transaction.CommitAsync(cancellationToken);
        }

        public async Task RollbackAsync(CancellationToken cancellationToken = default)
        {
            if (_transaction != null)
                await _transaction.RollbackAsync(cancellationToken);
        }
    }
}
