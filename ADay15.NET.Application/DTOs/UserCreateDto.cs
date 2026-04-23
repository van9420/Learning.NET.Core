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
    public class UserCreateDto
    {
        /// <summary>
        /// 账号，不能为空且唯一
        /// </summary>
        [Required(ErrorMessage ="账号不能为空")]
        public string Account { get; set; }

        /// <summary>
        /// 密码
        /// </summary>
        public string Password { get; set; }

        /// <summary>
        /// 姓名
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 状态 0=禁用 1=正常
        /// </summary>
        public int Status { get; set; } = 1;


        /// <summary>
        /// 角色 Id 列表
        /// </summary>
        public int[] RoleId { get; set; }
    }
}
