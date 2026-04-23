using ADay15.NET.Domain.Enums;
using ADay15.NET.Infrastructure.Commons;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace ADay15.NET.Infrastructure.Middlewares
{
    /// <summary>
    /// 自定义  全局异常捕获处理 中间件
    /// 
    /// 基本步骤：
    ///     1、‌定义中间件类‌：
    ///             创建一个类，并实现RequestDelegate委托。
    ///     2、‌实现中间件逻辑‌：
    ///             在实现中，你可以访问HttpContext对象来处理请求和响应。
    ///     3、注册中间件‌：
    ///             在Startup.cs或Program.cs中注册你的中间件。
    /// </summary>
    public class ExceptionMiddleware
    {
        //RequestDelegate是一个委托类型，代表了中间件处理请求的函数签名。
        private readonly RequestDelegate _next;

        public ExceptionMiddleware(RequestDelegate next) 
        {
            _next = next;
        }


        public async Task InvokeAsync(HttpContext context) 
        {
            try
            {
                //调用后续管道
                await _next(context);
            }
            catch (Exception ex) 
            {
                // 统一处理异常
                await HandleExceptionAsync(context, ex);
            }
            
        }


        /// <summary>
        /// 异常处理逻辑
        /// </summary>
        private async Task HandleExceptionAsync(HttpContext context, Exception ex) 
        {
            // 设置响应类型
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;

            // 构建统一返回格式
            var result = R<string>.Fail(ResponseCode.ServerError, $"服务器异常：{ex.Message}");

            // JSON序列化返回
            var json =JsonSerializer.Serialize(result);
            await context.Response.WriteAsync(json);

            Console.WriteLine($"【全局异常】{ex.Message}\n{ex.StackTrace}");  //ex.StackTrace 是 C# 中用于获取异常调用堆栈信息的属性，返回一个字符串，描述从抛出异常的位置到当前捕获点之间的方法调用序列
        }
    }
}
