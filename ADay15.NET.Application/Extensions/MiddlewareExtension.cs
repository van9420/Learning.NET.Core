using ADay15.NET.Infrastructure.Middlewares;
using Microsoft.AspNetCore.Builder;
using System.Text;
using System.Threading.Tasks;

namespace ADay15.NET.Application.Extensions
{
    public static class MiddlewareExtension
    {
        /// <summary>
        /// 全局异常中间件
        /// </summary>
        public static IApplicationBuilder UseGlobalException(this IApplicationBuilder app) 
        {
            return app.UseMiddleware<ExceptionMiddleware>(); //UseMiddleWare : 一般用于注册自定义封装的中间件，内部其实是使用Use的方式进行中间件注册；
        }

        /// <summary>
        /// 请求日志中间件
        /// </summary>
        public static IApplicationBuilder UseLogMiddleware(this IApplicationBuilder app) 
        {
            return app.UseMiddleware<LogMiddleware>();
        }
    }
}
