using ADay15.NET.Application.Extensions;
using ADay15.NET.Application.Services;
using ADay15.NET.Domain.Interfaces;
using ADay15.NET.Domain.Repositories;
using ADay15.NET.Infrastructure.DbContexts;
using ADay15.NET.Infrastructure.Middlewares;
using ADay15.NET.Infrastructure.Repositories;
using ADay15.NET.Infrastructure.UnitOfWork;
using Microsoft.EntityFrameworkCore;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

//注册Service
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IUserRoleRepository, UserRoleRepository>();
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped<IUserService, UserService>();

//注入 Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// 注入 EF Core
builder.Services.AddDbContext<AppDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));//从 appsettings.json 获取数据库连接
});

//Serilog 注册（用 Serilog 替换默认日志）
builder.Host.UseSerilog((ctx, cfg) =>
{
    cfg.ReadFrom.Configuration(ctx.Configuration);// 从 appsettings.json 读取日志配置
});


var app = builder.Build();
// 中间件顺序：日志 → 异常 → 其他

//启用日志中间件
app.UseLogMiddleware();

//启用全局异常中间件
app.UseGlobalException();

//app.UseRouting();   //路由
//app.UseAuthentication();    //认证
//app.UseAuthorization(); //授权

if (app.Environment.IsDevelopment()) 
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapControllers();
app.Run();
