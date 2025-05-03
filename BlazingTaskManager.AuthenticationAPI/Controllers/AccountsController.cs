using BlazingTaskManager.AuthenticationAPI.Repositories;
using BlazingTaskManager.Shared.APIServiceLogs;
using BlazingTaskManager.Shared.Domain.DTO.Authentication;
using BlazingTaskManager.Shared.Responses;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BlazingTaskManager.AuthenticationAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    //[Authorize]
    public class AccountsController : BaseController
    {
        readonly IConfiguration _configuration;
        readonly IUserRepository _userRepositority;

        public AccountsController(IConfiguration configuration, IUserRepository userRepositority)
        {
            _configuration = configuration;
            _userRepositority = userRepositority;
        }

        [AllowAnonymous]
        [HttpGet("/")]
        public IActionResult Get()
        {
            LogException.LogToConsole("Hello from the root route.");
            return Ok(new { message = "Hello from the root route." });
        }

        [AllowAnonymous]
        [HttpPost("register")]
        public async Task<ActionResult<BaseAPIResponse>> RegisterAsync(RegisterRequestDTO dto)
        {
            if (!ModelState.IsValid) { return BadRequest(); }

            var result = await _userRepositority.RegisterAsync(dto);
            return result.Success ? Ok(result) : BadRequest(result.Message);
        }
    }
}
