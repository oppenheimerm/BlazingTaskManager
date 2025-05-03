using BlazingTaskManager.AuthenticationAPI.Repositories;
using BlazingTaskManager.Shared.Domain;
using BlazingTaskManager.Shared.Domain.DTO.Role;
using BlazingTaskManager.Shared.Responses;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BlazingTaskManager.AuthenticationAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    //[Authorize]
    public class RolesController : Controller
    {
        readonly IConfiguration _configuration;
        readonly IRolesRepository _rolesRepository;

        public RolesController( IConfiguration configuration, IRolesRepository rolesRepository)
        {
            _configuration = configuration;
            _rolesRepository = rolesRepository;
        }


        [HttpPost("add-role")]
        public async Task<ActionResult<BaseAPIResponse>> CreateRoleAsync(Role role)
        {
            if (!ModelState.IsValid) { return BadRequest(); }

            var result = await _rolesRepository.AddRoleAsync(role);
            return result.Success ? Ok(result) : BadRequest(result.Message);
        }

        [HttpPost("add-userto-role")]
        public async Task<ActionResult<BaseAPIResponse>> AddUserToRoleAsync(AddUserToRoleRequestDTO dto)
        {
            if (!ModelState.IsValid) { return BadRequest(); };

            var result = await _rolesRepository.AddUserToUserRole(dto);
            return result.Success ? Ok(result) : BadRequest(result.Message);
        }

        [HttpGet("init-roles")]
        public async Task InitRolesAsync()
        {
            await _rolesRepository.InitRolesAsync();
        }
    }
}
