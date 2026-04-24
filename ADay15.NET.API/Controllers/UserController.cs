using ADay15.NET.Application.DTOs;
using ADay15.NET.Application.Services;
using ADay15.NET.Domain.Entities;
using ADay15.NET.Domain.Enums;
using ADay15.NET.Infrastructure.Commons;
using Microsoft.AspNetCore.Mvc;

namespace ADay15.NET.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserController:ControllerBase
    {

        private readonly UserService _userService;

        public UserController(UserService userService) 
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
            var data = await _userService.GetUserWithRolesAsync(userId);
            return Ok(R<object>.Sucess(data));
        }

        [HttpGet("user-account")]
        public async Task<ActionResult<R<User>>> GetUserByAccoun([FromQuery]string account)
        {
            var data = await _userService.GetUserByAccountAsync(account);
            return Ok(R<User>.Sucess(data));
        }

        /// <summary>
        /// 新增用户
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<ActionResult<R<User>>> AddUser(UserCreateDto dto) 
        {
            //数据校验
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var data = await _userService.AddUserAsync(dto);

            return Ok(data);
        }


        /// <summary>
        /// 修改用户
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        [HttpPut]
        public async Task<ActionResult<R<User>>> UpdUser(UserUpdateDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var data = await _userService.UpdUserAsync(dto);

            if (data == null)
                return NotFound();

            return Ok(data);
        }

        
        /// <summary>
        /// 删除用户
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        [HttpDelete("{Id}")]
        public async Task<ActionResult<R<User>>> DelUser([FromRoute]int Id)
        {
            var data = await _userService.DelUserAsync(Id);

            if (data == null)
                return NotFound();

            return Ok(data);
        }

        /// <summary>
        /// 分页查询
        /// </summary>
        /// <param name="pageNum"></param>
        /// <param name="pageSize"></param>
        /// <param name="account"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<ActionResult<R<object>>> GetUsers(
            [FromQuery] int pageNum = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] string? account = null,
            [FromQuery] string? name = null)
        {
            var (total, list) = await _userService.GetUsers(pageNum, pageSize, account, name);
            return Ok(R<object>.Sucess(new { total, list }));
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
        public async Task<ActionResult<R<dynamic>>> BatchDisable([FromBody] long[] ids)
        {
            var res = await _userService.ChangeStatus(ids);
            return Ok(res);
        }
    }
}
