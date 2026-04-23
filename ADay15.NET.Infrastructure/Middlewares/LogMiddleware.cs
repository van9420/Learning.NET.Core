using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ADay15.NET.Infrastructure.Middlewares
{
    public class LogMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<LogMiddleware> _logger;

        /// <summary>
        /// 构造函数注入：日志 + 配置
        /// </summary>
        /// <param name="next"></param>
        /// <param name="logger"></param>
        public LogMiddleware(RequestDelegate next, ILogger<LogMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var path = context.Request.Path;
            var method = context.Request.Method;

            _logger.LogInformation($"【请求开始】{method} {path}");

            var stopwatch = Stopwatch.StartNew();

            await _next(context);

            stopwatch.Stop();

            _logger.LogInformation($"【请求结束】{method} {path} | 耗时：{stopwatch.ElapsedMilliseconds}ms");
        }
    }
}
