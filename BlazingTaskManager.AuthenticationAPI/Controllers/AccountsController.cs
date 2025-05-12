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

        [AllowAnonymous]
        [HttpPost("login")]
        public async Task<ActionResult<APIResponseAuthentication>> LoginAsync(AuthenticateRequestDTO dto)
        {
            if (!ModelState.IsValid)
            {
                return new APIResponseAuthentication()
                {
                    Success = false,
                    JwtToken = string.Empty,
                    RefreshToken = string.Empty,
                    Message = "Password or email address is incorrect."
                };
            }

            var result = await _userRepositority.AuthenticateAsync(dto, ipAddress());

            if (result.Success)
            {
                setTokenCookie(result.RefreshToken!);
                return Ok(result);
            }
            else
            {
                return BadRequest(result);
            }

        }

        [AllowAnonymous]
        [HttpPost("verify-email")]
        public async Task<IActionResult> VerifyEmail(VerifyEmailRequestDTO model)
        {
            if (!ModelState.IsValid) { return BadRequest(); }

            if (!string.IsNullOrEmpty(model.Token))
            {
                var result = await _userRepositority.VerifyEmailAsync(model.Token);
                return Ok(new { message = "Verification successful, you can now login" });
            }
            else
            {
                return BadRequest("Invalid or missing validation token.");
            }

        }

        /// <summary>
        /// Get the IP address of the client
        /// </summary>
        /// <returns></returns>
        private string ipAddress()
        {
            if (Request.Headers.ContainsKey("X-Forwarded-For"))
            {
                string? forwardedFor = Request.Headers["X-Forwarded-For"].ToString();
                if (!string.IsNullOrEmpty(forwardedFor))
                {
                    return forwardedFor;
                }
            }

            if (HttpContext.Connection.RemoteIpAddress != null)
            {
                return HttpContext.Connection.RemoteIpAddress.MapToIPv4().ToString();
            }

            return "127.0.0.1"; // Fallback to localhost if no IP can be determined
        }

        private void setTokenCookie(string token)
        {
            var cookieOptions = new CookieOptions
            {
                HttpOnly = true,
                Expires = DateTime.UtcNow.AddDays(7)
            };
            Response.Cookies.Append("refreshToken", token, cookieOptions);
        }
    }
}
