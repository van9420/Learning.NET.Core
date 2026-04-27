using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ADay15.NET.Application.DTOs
{
    /// <summary>
    /// 分页查询用户列表的 DTO
    /// </summary>
    public class UserPageQueryDto
    {
        /// <summary>
        /// 页码，默认值为1
        /// </summary>
        public int PageNum { get; set; } = 1;

        /// <summary>
        /// 页大小，默认值为10
        /// </summary>
        public int PageSize { get; set; } =10;

        /// <summary>
        /// 账号（可选），用于模糊查询
        /// </summary>
        public string? Account { get; set; }

        /// <summary>
        /// 姓名（可选），用于模糊查询
        /// </summary>
        public string? Name { get; set; }
    }
}
