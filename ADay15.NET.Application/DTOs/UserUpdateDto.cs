using ADay15.NET.Domain.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ADay15.NET.Application.DTOs
{
    /// <summary>
    /// 参数：用户 ID、姓名、状态、角色 Id 列表
    /// </summary>
    public class UserUpdateDto
    {
        [Required]
        public long Id { get; set; }

        /// <summary>
        /// 姓名
        /// </summary>
        public string? Name { get; set; }

        /// <summary>
        /// 状态 0=禁用 1=正常
        /// </summary>
        public int? Status { get; set; } 


        /// <summary>
        /// 角色 Id 列表
        /// </summary>
        public int[]? RoleId { get; set; }
    }
}
