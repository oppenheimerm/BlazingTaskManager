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
        /// User must verfiy their email address with the token generated on registration.
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        public async Task<BaseAPIResponse> VerifyEmailAsync(string token)
        {
            var account = await _authenticationDataContext.Users.SingleOrDefaultAsync(x => x.VerificationToken == token);

            if (account == null)
                return new BaseAPIResponse() { Success = false, Message = "Email verification failed" };

            account.Verified = DateTime.UtcNow;
            account.VerificationToken = null;

            _authenticationDataContext.Users.Update(account);
            await _authenticationDataContext.SaveChangesAsync();
            return new BaseAPIResponse() { Success = true, Message = string.Empty };
        }

        /// <summary>
        /// Get instance of <see cref="BTUser"/> by user Id
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        public async Task<BTUserDTO?> GetUserByIdAsync(Guid Id)
        {
            var user = await _authenticationDataContext.Users
                .Where(u => u.Id == Id)
                .Include(r => r.Roles)
                .AsNoTracking()
                .FirstOrDefaultAsync();

            if (user is not null)
            {
                var userFound = user.ToDto();
                List<RoleDTO>? roles = await GetRolesForUser(user.Id);
                if (roles is not null)
                {
                    userFound.Roles = roles.ToList();
                }
                return userFound;

            }
            return null!;
        }

        /// <summary>
        /// Get instance of <see cref="BTUser"/> by email
        /// </summary>
        /// <param name="Email"></param>
        /// <returns></returns>
        public async Task<BTUserDTO?> GetUserByEmailAsync(string Email)
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
                    var userFound = user.ToDto();
                    List<RoleDTO>? roles = await GetRolesForUser(user.Id);
                    if (roles is not null)
                    {
                        userFound.Roles = roles.ToList();
                    }
                    return userFound;
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

        /// <summary>
        /// On successful authentication the API returns a short lived JWT access token that expires after 15 minutes, 
        /// and a refresh token that expires after 7 days in an HTTP Only cookie. The JWT is used for accessing secure 
        /// routes on the API and the refresh token is used for generating new JWT access tokens when (or just before) 
        /// they expire.
        /// </summary>
        /// <param name="model"></param>
        /// <param name="ipAddress"></param>
        /// <returns></returns>
        public async Task<APIResponseAuthentication> AuthenticateAsync(AuthenticateRequestDTO model, string ipAddress)
        {
            if (!string.IsNullOrEmpty(model.Email))
            {
                var account = await _authenticationDataContext.Users
                                .Include(t => t.RefreshTokens)
                                .AsNoTracking()
                                .FirstOrDefaultAsync();

                // validate
                // validate
                if (account == null || !BCrypt.Net.BCrypt.Verify(model.Password, account.PasswordHash))
                    return new APIResponseAuthentication(false, "Email or password is incorrect");

                if (!account.IsVerified)
                    return new APIResponseAuthentication(false, "Pleae verify your account");

                if (account.AccountLockedOut == true)
                    return new APIResponseAuthentication(false, "Your account is locked.  Please contact our help desk for assistance.");


                var accountToBTUserDTO = account.ToDto();
                List<RoleDTO>? roles = await GetRolesForUser(account.Id);
                if (roles is not null)
                {
                    accountToBTUserDTO.Roles = roles.ToList();
                }

                // authentication successful so generate jwt and refresh tokens
                var jwtToken = _jwtUtilities.GenerateToken(accountToBTUserDTO, _configuration["JwtSettings:Secret"]!,
                    _configuration["JwtSettings:Issuer"]!, _configuration["JwtSettings:Audience"]!);
                var refreshToken = await GenerateRefreshToken(ipAddress, accountToBTUserDTO.Id);
                if (account.RefreshTokens is not null  && refreshToken is not null)
                {
                    account.RefreshTokens.Add(refreshToken);
                }

                // remove old refresh tokens from account
                removeOldRefreshTokens(account);

                // save changes to db
                _authenticationDataContext.Update(account);
                await _authenticationDataContext.SaveChangesAsync();

                var accountToDTO = account.ToDto();
                // get roles for user
                if (accountToDTO is not null)
                {
                    List<RoleDTO>? _roles = await GetRolesForUser(account.Id);
                    if (_roles is not null)
                    {
                        accountToDTO.Roles = _roles.ToList();
                    }
                }

                var response = new APIResponseAuthentication(true, string.Empty, accountToDTO, jwtToken, refreshToken.Token);
                return response;
            }
            else
            {
                return new APIResponseAuthentication(false, "User not found"); ;
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


        private async Task<List<RoleDTO>?> GetRolesForUser(Guid userId)
        {
            var _roles = await _authenticationDataContext.UserRoles.Where(r => r.UserId == userId)
                .Include(r => r.Role)
                .Select(x => new RoleDTO() { Description = x.Role!.Description, RoleCode = x.Role.RoleCode, RoleName = x.Role.RoleName })
                .AsNoTracking()
                .ToListAsync();

            return _roles;
        }

        /// <summary>
        /// Generate a new refresh token for the user.  It's a cryptographically strong random sequence of values.
        /// This inclusion here and not in JwtUtul, is because it has a dependency on _applicationDBContext
        /// </summary>
        /// <param name="ipAddress"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        async Task<RefreshToken?> GenerateRefreshToken(string ipAddress, Guid userId)
        {
            var refreshToken = new RefreshToken
            {
                // token is a cryptographically strong random sequence of values
                Token = Convert.ToHexString(RandomNumberGenerator.GetBytes(64)),
                // token is valid for 7 days
                Expires = DateTime.UtcNow.AddDays(7),
                Created = DateTime.UtcNow,
                CreatedByIp = ipAddress,
                AccountId = userId
            };

            // ensure token is unique by checking against db
            var tokenFound = await _authenticationDataContext.RefreshTokens.Where(t => t.Token == refreshToken.Token).FirstOrDefaultAsync();

            if (tokenFound is not null)
                return await GenerateRefreshToken(ipAddress, userId);

            try
            {
                await _authenticationDataContext.RefreshTokens.AddAsync(refreshToken);
                await _authenticationDataContext.SaveChangesAsync();
                return refreshToken;
            }
            catch
            {
                return null!;
            }

        }

        /// <summary>
        /// Remove old inactive refresh tokens from user based on TTL in app settings
        /// </summary>
        /// <param name="user"></param>
        private void removeOldRefreshTokens(BTUser user)
        {
            if (user.RefreshTokens is not null)
            {
                if (user.RefreshTokens.Count >= 1)
                {
                    // remove old inactive refresh tokens from user based on TTL in app settings
                    user.RefreshTokens.RemoveAll(x =>
                        !x.IsActive && x.Created!.Value.AddDays(int.Parse(_configuration["ApplicationSettings:RefreshTokenTTL"]!)) <= DateTime.UtcNow);
                }
            }
        }

        /*private void List<RefreshTokenDTO> GetRefreshTokensForUser(Guid userId){
            var refreshTokens = _authenticationDataContext.RefreshTokens
                .Where(t => t.AccountId == userId)
                .Select(t => new RefreshTokenDTO()
                {
                    Token = t.Token,
                    Expires = t.Expires,
                    Created = t.Created,
                    CreatedByIp = t.CreatedByIp
                })
                .AsNoTracking()
                .ToList();
        }*/

        #endregion
    }
}
