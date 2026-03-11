using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MyApi.Dto;
using MyApi.Model;
using MyApi.Services;

namespace MyApi.Controllers
{
    [Route("api/users")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserService service;

        public UserController(IUserService service)
        {
            this.service = service;
        }

        [HttpGet]
        public async Task<IActionResult> GetPage(string? keyword = null, int Page = 1, int PageSize = 10)
        {
            return Ok(await service.GetAsync(keyword,Page,PageSize));
        }


        [HttpGet("{Id:int}")]
        public async Task<IActionResult> GetById(int Id)
        {
            var result = await service.GetByIdAsync(Id);
            return result is null ? NotFound(new { message = "ไม่พบข้อมูล"}) : Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] UserCreateRequest Request)
        {
            var result = await service.CreateAsync(Request);
            return CreatedAtAction(nameof(GetById), new { id = result.UserId }, result);
        }

        [HttpPut("{Id:int}")]
        public async Task<IActionResult> Update([FromRoute] int Id, [FromBody] UserUpdateRequest Request)
        {
            var Updated = await service.UpdateAsync(Id, Request);
            return Updated is null ? NotFound() : Ok(Updated);
        }

        [HttpDelete("{Id:int}")]
        public async Task<IActionResult> Delete(int Id)
        {
            var result = await service.DeleteAsync(Id);
            return result ? NoContent() : NotFound();
        }

        [HttpGet("role")]
        public async Task<IActionResult> GetRole()
        {
            var result = await service.GetRoleList();
            return Ok(result);
        }
    }
}
