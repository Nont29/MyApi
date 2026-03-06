using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
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
    }
}
