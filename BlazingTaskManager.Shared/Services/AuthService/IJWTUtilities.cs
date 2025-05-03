
using BlazingTaskManager.Shared.Domain.DTO.Authentication;
using BlazingTaskManager.Shared.Domain.DTO.Role;
using BlazingTaskManager.Shared.Domain.DTO.User;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace BlazingTaskManager.Shared.Services.AuthService
{
    public interface IJWTUtilities
    {
        /// <summary>
        /// Generates a JWT token for the user.
        /// </summary>
        /// <param name="user"></param>
        /// <param name="secret"></param>
        /// <param name="issuer"></param>
        /// <param name="audience"></param>
        /// <returns></returns>
        string GenerateToken(BTUserDTO user, string secret, string issuer, string audience);

        /// <summary>
        /// Decrypts the JWT token and returns the user claims.
        /// </summary>
        /// <param name="jwtToken"></param>
        /// <returns></returns>
        BTUserClaimDTO? DecryptToken(string jwtToken);

        /// <summary>
        /// Validates the JWT token and returns the user ID if valid.
        /// </summary>
        /// <param name="token"></param>
        /// <param name="secret"></param>
        /// <param name="issuer"></param>
        /// <param name="audience"></param>
        /// <returns></returns>
        Guid? ValidateJwtToken(string token, string secret, string issuer, string audience);        
    }
}
