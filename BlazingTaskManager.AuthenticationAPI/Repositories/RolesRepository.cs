using BlazingTaskManager.AuthenticationAPI.Data;
using BlazingTaskManager.Shared.APIServiceLogs;
using BlazingTaskManager.Shared.Domain;
using BlazingTaskManager.Shared.Domain.DTO.Role;
using BlazingTaskManager.Shared.Responses;
using BlazingTaskManager.Shared.Services.AuthService;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace BlazingTaskManager.AuthenticationAPI.Repositories
{
    public class RolesRepository : IRolesRepository
    {
        readonly IConfiguration _configuration;
        readonly AuthenticationDataContext _authenticationDataContext;
        readonly IJWTUtilities _jwtUtilities;

        public RolesRepository(IConfiguration configuration, AuthenticationDataContext authenticationDataContext,
            IJWTUtilities jWTUtilities)
        {
            _configuration = configuration;
            _authenticationDataContext = authenticationDataContext;
            _jwtUtilities = jWTUtilities;
        }

        /// <summary>
        /// Add a new role to the database.
        /// </summary>
        /// <param name="role"></param>
        /// <returns></returns>
        public async Task<APIResponseRole> AddRoleAsync(Role role)
        {
            try
            {
                var findRole = await _authenticationDataContext.Roles.FirstOrDefaultAsync(r => r.RoleCode == role.RoleCode!.ToUpper());
                if (findRole is not null) return new APIResponseRole(false, $"Role code: {role.RoleCode} already in use.");

                role.RoleCode = role.RoleCode!.ToUpper();
                await _authenticationDataContext.Roles.AddAsync(role);
                await _authenticationDataContext.SaveChangesAsync();
                var msg = $"Successfully created a new rolw with the name: {role.RoleName} and Id: {role.RoleCode}. Timestamp: {DateTime.UtcNow}";
                LogException.LogToDebugger(msg);
                LogException.LogToConsole(msg);
                return new APIResponseRole(true, msg, role);
            }
            catch (Exception err)
            {

                LogException.LogToDebugger(err.ToString());
                return new APIResponseRole(false, $"Failed to add role:{role.RoleName} to database");
            }
        }

        /// <summary>
        /// Add a user to the USER role
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        public async Task<BaseAPIResponse> AddUserToUserRole(AddUserToRoleRequestDTO dto)
        {
            var user = await _authenticationDataContext.Users.FirstOrDefaultAsync(u => u.Id == dto.UserId);
            var role = await _authenticationDataContext.Roles.FirstOrDefaultAsync(r => r.RoleCode == dto.RoleCode);

            if (user is not null && role is not null)
            {
                try
                {
                    //  User already in this role?
                    var userInRoleCheck = await _authenticationDataContext.UserRoles
                        .Where(r => r.RoleCode == dto.RoleCode && r.UserId == dto.UserId)
                        .FirstOrDefaultAsync();

                    if (userInRoleCheck is not null)
                    {
                        return new BaseAPIResponse() { Success = false, Message = $"UserId: {dto.UserId}, already in role: {dto.RoleCode} ." };
                    }
                    else
                    {
                        var newUserRole = new UserRole() { RoleCode = dto.RoleCode!.ToUpper(), UserId = dto.UserId, AddedDate = DateTime.UtcNow };
                        await _authenticationDataContext.UserRoles.AddAsync(newUserRole);
                        await _authenticationDataContext.SaveChangesAsync();


                        var msg = $"Successfully added userId {dto.UserId} to role: {dto.RoleCode}. Timestamp: {DateTime.UtcNow}";
                        LogException.LogToDebugger(msg);
                        LogException.LogToConsole(msg);
                        return new BaseAPIResponse() { Success = true, Message = string.Empty };
                    }

                }
                catch (Exception err)
                {
                    LogException.LogToDebugger(err.ToString());
                    return new BaseAPIResponse() { Success = false, Message = $"Failed to add userId {dto.UserId} to role: {dto.RoleCode}." };
                }
            }
            else
            {
                return new BaseAPIResponse() { Success = false, Message = "User or role code was incorrect." };
            }
        }

        /// <summary>
        /// Initialize the roles in the database.
        /// </summary>
        /// <returns></returns>
        public async Task InitRolesAsync()
        {
            var roles = await _authenticationDataContext.Roles
                .AsNoTracking()
                .ToListAsync();

            var _newRoles = new List<Role>()
             {
                new() { RoleCode = "ADMN", RoleName = "Administrator", Description = "Administrator role" },
                new() { RoleCode = "USER", RoleName = "User", Description = "User role" }
             };

            if (roles != null && roles.Count == 0)
            {
                _authenticationDataContext.Roles
                    .AddRange(_newRoles);
                await _authenticationDataContext.SaveChangesAsync();
            }
        }

        #region Utilities

        /// <summary>
        /// When we create a user in <see cref="UserRepository"/> on successful creation, we add the
        /// user to the USER role.  We need to ensue that we have at least one role created first.e
        /// </summary>
        /// <returns></returns>


        #endregion
    }
}
