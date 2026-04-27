using ADay15.NET.Application.DTOs;
using ADay15.NET.Application.Services;
using ADay15.NET.Domain.Entities;
using ADay15.NET.Domain.Enums;
using ADay15.NET.Infrastructure.Commons;
using Microsoft.AspNetCore.Mvc;
using System.Security.Principal;

namespace ADay15.NET.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserController:ControllerBase
    {

        private readonly IUserService _userService;

        public UserController(IUserService userService) 
        {
            _userService = userService;
        }

        /// <summary>
        /// 根据 ID 查询用户（带角色）
        /// 要求：AsNoTracking
        /// </summary>
        /// <param name="userId">用户 ID</param>
        /// <returns>：用户信息 + 角色列表</returns>
        [HttpGet("user-roles/{userId}")]
        public async Task<ActionResult<R<object>>> GetUserRoles(long userId)
        {
            return await _userService.GetUserWithRolesAsync(userId);
        }

        [HttpGet("user-account")]
        public async Task<ActionResult<R<User>>> GetUserByAccount([FromQuery]string account)
        {
            return await _userService.GetUserByAccountAsync(account);
        }

        /// <summary>
        /// 新增用户
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<ActionResult<R<string>>> AddUser(UserCreateDto dto) 
        {
            //数据校验
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            return await _userService.AddUserAsync(dto);
        }


        /// <summary>
        /// 修改用户
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        [HttpPut]
        public async Task<ActionResult<R<string>>> UpdUser(UserUpdateDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            return await _userService.UpdateUserAsync(dto);
        }

        
        /// <summary>
        /// 删除用户
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        [HttpDelete("{Id}")]
        public async Task<ActionResult<R<string>>> DelUser([FromRoute]int Id)
        {

            return await _userService.DeleteUserAsync(Id);
        }

        /// <summary>
        /// 分页查询
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<ActionResult<R<PageResult<User>>>> GetUsers(UserPageQueryDto dto)
        {
            return await _userService.GetUserPageListAsync(dto);
        }


        /// <summary>
        /// 批量修改状态
        /// 
        /// 
        /// postman 访问测试时，
        /// 参数位置：Body → raw → JSON
        /// 参数格式（数组格式！）：[10005,10006,10007,10008]
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        [HttpPut("batch-disable")]
        public async Task<ActionResult<R<string>>> BatchDisable([FromBody] long[] ids)
        {
            return await _userService.BatchDisableUserAsync(ids);
        }

        [HttpPost("login")]
        public async Task<ActionResult<R<User>>> LoginUser([FromBody]UserLoginDto dto)
        {
            return await _userService.LoginAsync(dto.Account, dto.Password);
        }
    }
}
