using BlazingTaskManager.AuthenticationAPI.Repositories;
using BlazingTaskManager.Shared.Services.AuthService;

namespace BlazingTaskManager.AuthenticationAPI.Authorization
{
    public class JwtMiddleware
    {
        readonly IConfiguration _configuration;
        private readonly RequestDelegate _next;

        public JwtMiddleware(RequestDelegate next, IConfiguration configuration)
        {
            _next = next;
            _configuration = configuration;
        }

        public async Task Invoke(HttpContext context, IUserRepository repo, IJWTUtilities _jwtUtility)
        {
            var token = context.Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();

            if (!string.IsNullOrEmpty(token))
            {
                var accountId = _jwtUtility.ValidateJwtToken(token, _configuration["JwtSettings:Secret"]!, 
                    _configuration["JwtSettings:Issuer"]!,
                    _configuration["JwtSettings:Audience"]!);

                if (accountId.HasValue)
                {
                    // attach account to context on successful jwt validation
                    var account = await repo.GetUserByIdAsync(accountId.Value);
                    context.Items["Account"] = account;
                }
            }

            await _next(context);
        }
    }
}
