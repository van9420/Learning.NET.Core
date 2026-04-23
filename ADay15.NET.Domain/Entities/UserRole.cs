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
    /// 用户角色关联表
    /// </summary>
    public class UserRole
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]   //主键自增
        public long  Id { get; set; }

        public long UserId { get; set; }

        public long RoleId { get; set; }

        // 导航
        public virtual User User { get; set; }

        public virtual Role Role { get; set; }
    }
}
