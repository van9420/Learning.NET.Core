using ADay15.NET.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ADay15.NET.Infrastructure.DbContexts
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        // 表
        public DbSet<User> Users { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<UserRole> UserRoles { get; set; }

        // FluentAPI 配置（可选，比注解更强大）
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // 联合索引/唯一键示例
            modelBuilder.Entity<User>()
               .HasIndex(u => u.Account)
               .IsUnique();

            modelBuilder.Entity<Role>()
               .HasIndex(u => u.RoleCode)
               .IsUnique();

            modelBuilder.Entity<UserRole>()
               .HasIndex(ur => new { ur.UserId, ur.RoleId })
               .IsUnique();
        }
    }
}
