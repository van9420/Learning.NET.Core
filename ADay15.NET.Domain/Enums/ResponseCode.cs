using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ADay15.NET.Domain.Enums
{
    /// <summary>
    /// 全局统一响应码
    /// </summary>
    public enum ResponseCode
    {
        // 成功
        Success=200,
        // 参数错误
        ParamError=400,
        // 未授权
        Unauthorized=401,
        // 权限不足
        Forbidden=403,
        // 资源不存在
        NotFound=404,
        // 服务器错误
        ServerError=500
    }
}
