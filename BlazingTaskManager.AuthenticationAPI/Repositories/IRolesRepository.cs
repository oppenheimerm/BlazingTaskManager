using BlazingTaskManager.Shared.Domain;
using BlazingTaskManager.Shared.Domain.DTO.Role;
using BlazingTaskManager.Shared.Responses;

namespace BlazingTaskManager.AuthenticationAPI.Repositories
{
    public interface IRolesRepository
    {
        /// <summary>
        /// Add a new role to the database.
        /// </summary>
        /// <param name="role"></param>
        /// <returns></returns>
        Task<APIResponseRole> AddRoleAsync(Role role);
        /// <summary>
        /// Add a user to the USER role
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        Task<BaseAPIResponse> AddUserToUserRole(AddUserToRoleRequestDTO dto);
        /// <summary>
        /// Initialize the roles in the database.
        /// </summary>
        /// <returns></returns>
        Task InitRolesAsync();
    }
}
