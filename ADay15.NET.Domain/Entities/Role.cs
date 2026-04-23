using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ADay15.NET.Domain.Entities
{
    /// <summary>
    /// 角色表
    /// </summary>
    public class Role
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }

        /// <summary>
        /// 角色名称
        /// </summary>
        [Required]
        [Column(TypeName = "nvarchar(50)")]
        public string RoleName { get; set; }

        /// <summary>
        /// 角色编码
        /// </summary>
        [Required]
        [Column(TypeName = "varchar(50)")]
        public string RoleCode { get; set; }

        public int Status { get; set; } = 1;

        public DateTime CreateTime { get; set; } = DateTime.Now;

        // 导航属性：一个角色多个用户
        public ICollection<UserRole> UserRoles { get; set; } = new List<UserRole>();
    }
}
