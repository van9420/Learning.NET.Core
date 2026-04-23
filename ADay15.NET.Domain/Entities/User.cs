using ADay15.NET.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ADay15.NET.Domain.Entities
{
    /// <summary>
    /// 用户表
    /// </summary>
    public class User
    {
        /// <summary>
        /// 用户ID
        /// </summary>
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }

        /// <summary>
        /// 账号
        /// </summary>
        [Required]
        [Column(TypeName = "varchar(50)")]
        [DisplayName("账号")]
        public string Account { get; set; }

        /// <summary>
        /// 密码
        /// </summary>
        [Required]
        [Column(TypeName = "varchar(100)")]
        [DisplayName("密码")]
        public string Password { get; set; }

        /// <summary>
        /// 姓名
        /// </summary>
        [Column(TypeName = "nvarchar(20)")]
        [DisplayName("姓名")]
        public string Name { get; set; }

        /// <summary>
        /// 状态 0=禁用 1=正常
        /// </summary>
        [DisplayName("状态")]
        public int Status { get; set; } = 1;

        /// <summary>
        /// 创建时间
        /// </summary>
        [DisplayName("创建时间")]
        public DateTime CreateTime { get; set; } = DateTime.Now;


        [DisplayName("电话")]
        public string? PhoneNumber {  get; set; }
        
        [DisplayName("邮箱")]
        public string? EmailAddress {  get; set; }

        [DisplayName("性别")]
        public GenderCode Gender {  get; set; }

        [DisplayName("头像")]
        public string? AvatarUrl { get; set; } // 保存头像路径或URL



        // 导航属性：一个用户多个角色
        public virtual ICollection<UserRole> UserRoles { get; set; } = new List<UserRole>();

    }
}
