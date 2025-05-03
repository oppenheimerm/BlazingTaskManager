using BlazingTaskManager.AuthenticationAPI.Data;
using BlazingTaskManager.AuthenticationAPI.Helpers;
using BlazingTaskManager.Shared.APIServiceLogs;
using BlazingTaskManager.Shared.Domain;
using BlazingTaskManager.Shared.Domain.DTO.Authentication;
using BlazingTaskManager.Shared.Domain.DTO.Role;
using BlazingTaskManager.Shared.Domain.DTO.User;
using BlazingTaskManager.Shared.Responses;
using BlazingTaskManager.Shared.Services.AuthService;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;

namespace BlazingTaskManager.AuthenticationAPI.Repositories
{
    public class UserRepository : IUserRepository
    {
        readonly IConfiguration _configuration;
        readonly AuthenticationDataContext _authenticationDataContext;
        readonly IJWTUtilities _jwtUtilities;


        public UserRepository(IConfiguration configuration, AuthenticationDataContext authenticationDataContext,
            IJWTUtilities jwtUtilities)
        {
            _configuration = configuration;
            _authenticationDataContext = authenticationDataContext;
            _jwtUtilities = jwtUtilities;
        }

        /// <summary>
        /// Register a new <see cref="BTUser"/>
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public async Task<BaseAPIResponse> RegisterAsync(RegisterRequestDTO model)
        {
            // validate
            if (_authenticationDataContext.Users.Any(x => x.Email == model.Email.ToLower()))
            {
                return new BaseAPIResponse() { Success = false, Message = $"Account with email: {model.Email},already exist." };
            }

            //  Validate password
            var passwordValid = AccountHelpers.ValidatePassword(model.Password!, int.Parse(_configuration["ApplicationSettings:MinimumPasswordLength"]!));
            if (!passwordValid)
                return new BaseAPIResponse() { Success = false, Message = $"Passwords must be a minimum of {int.Parse(_configuration["ApplicationSettings:MinimumPasswordLength"]!)}, contain at least one upper case character, one lower case character, at least one number and at least one non alpha numeric character." };


            var account = model.ToEntity();

            account.VerificationToken = GenerateVerificationToken();
            // hash password
            account.PasswordHash = BCrypt.Net.BCrypt.HashPassword(model.Password);

            try
            {
                // save account
                _authenticationDataContext.Users.Add(account);
                await _authenticationDataContext.SaveChangesAsync();

                //  add user to USER Role
                await AddUserToUserRole(
                    new AddUserToRoleRequestDTO()
                    {
                        UserId = account.Id,
                        RoleCode = "USER"
                    });

                // send email
                //SendVerificationEmail(account, origin);
                //  /account/verify-email?token={account.VerificationToken}"
                return new BaseAPIResponse() { Success = true, Message = account.VerificationToken };
            }
            catch (Exception ex)
            {
                LogException.LogToDebugger($"Register user failed : {ex.ToString()}. Timestamp : {DateTime.UtcNow}");
                LogException.LogToConsole($"Register user failed - {DateTime.UtcNow}");
                return new BaseAPIResponse() { Success = false, Message = "Failed to register, please check form for errors." };
            }


        }

        /// <summary>
        /// Get instance of <see cref="BTUser"/> by user Id
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        public async Task<BTUser?> GetUserByIdAsync(Guid Id)
        {
            var user = await _authenticationDataContext.Users
                .Where(u => u.Id == Id)
                .Include(r => r.Roles)
                .AsNoTracking()
                .FirstOrDefaultAsync();

            if (user is not null)
            {
                List<Role>? roles = await GetRolesForUser(user.Id);
                if (roles is not null)
                {
                    user.Roles = roles.ToList();
                }
                return user;

            }
            return null!;
        }

        /// <summary>
        /// Get instance of <see cref="BTUser"/> by email
        /// </summary>
        /// <param name="Email"></param>
        /// <returns></returns>
        public async Task<BTUser?> GetUserByEmailAsync(string Email)
        {
            if (!string.IsNullOrEmpty(Email))
            {
                var user = await _authenticationDataContext.Users
                .Where(u => u.Email == Email.ToLower())
                .Include(r => r.Roles)
                .AsNoTracking()
                .FirstOrDefaultAsync();

                if (user is not null)
                {
                    List<Role>? roles = await GetRolesForUser(user.Id);
                    if (roles is not null)
                    {
                        user.Roles = roles.ToList();
                    }
                    return user;
                }
                return null!;
            }
            else
            {
                return null!;
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

        #region utilities

        private string GenerateVerificationToken()
        {
            // token is a cryptographically strong random sequence of values
            var token = Convert.ToHexString(RandomNumberGenerator.GetBytes(64));

            // ensure token is unique by checking against db
            var tokenIsUnique = !_authenticationDataContext.Users.Any(x => x.VerificationToken == token);
            if (!tokenIsUnique)
                return GenerateVerificationToken();

            return token;
        }


        private async Task<List<Role>?> GetRolesForUser(Guid userId)
        {
            var _roles = await _authenticationDataContext.UserRoles.Where(r => r.UserId == userId)
                .Include(r => r.Role)
                .Select(x => new Role() { Description = x.Role!.Description, RoleCode = x.Role.RoleCode, RoleName = x.Role.RoleName })
                .AsNoTracking()
                .ToListAsync();

            return _roles;
        }

        #endregion
    }
}
