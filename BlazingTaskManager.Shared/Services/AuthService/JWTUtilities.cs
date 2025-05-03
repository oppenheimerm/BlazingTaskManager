
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
    public class JWTUtilities : IJWTUtilities
    {
        /// <summary>
        /// Generates a JWT token for the user.
        /// </summary>
        /// <param name="user"></param>
        /// <param name="secret"></param>
        /// <param name="issuer"></param>
        /// <param name="audience"></param>
        /// <returns></returns>
        public string GenerateToken(BTUserDTO user, string secret, string issuer, string audience)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret));
            //SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            var credentrials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256Signature);
            var userClaims = new List<Claim> {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.FirstName!),
                new Claim(ClaimTypes.Email, user.Email!),
            };

            if (user.Roles is not null)
            {
                foreach (var role in user.Roles)
                {
                    userClaims.Add(
                        new Claim(ClaimTypes.Role, role.RoleCode!));
                }
            }

            var token = new JwtSecurityToken(
                issuer,
                audience,
                claims: userClaims,
                expires: DateTime.Now.AddMinutes(2),
                signingCredentials: credentrials
                );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        /// <summary>
        /// Decrypts the JWT token and returns the user claims.
        /// </summary>
        /// <param name="jwtToken"></param>
        /// <returns></returns>
        public BTUserClaimDTO? DecryptToken(string jwtToken)
        {
            try
            {
                if (string.IsNullOrEmpty(jwtToken)) return null!;

                var handler = new JwtSecurityTokenHandler();
                var token = handler.ReadJwtToken(jwtToken);
                List<RoleDTO>? rolesCollection = [];

                var Id = token.Claims.FirstOrDefault(_ => _.Type == ClaimTypes.NameIdentifier);
                var firstName = token.Claims.FirstOrDefault(_ => _.Type == ClaimTypes.Name);
                var email = token.Claims.FirstOrDefault(_ => _.Type == ClaimTypes.Email);


                var _roles = token.Claims.Where(_ => _.Type == ClaimTypes.Role).ToList();
                if (_roles is not null)
                {

                    if (_roles.Any())
                    {
                        var usrRoles = _roles
                            .Select(r => new RoleDTO()
                            {
                                RoleCode = r.Value
                            });

                        rolesCollection = usrRoles.ToList();
                    }
                }
                return new BTUserClaimDTO()
                {
                    Id = Guid.Parse(Id!.Value),
                    FirstName = firstName!.Value,
                    Email = email!.Value,
                    Roles = rolesCollection
                };
            }
            catch
            {
                return null!;
            }
        }

        /// <summary>
        /// Validates the JWT token and returns the user ID if valid.
        /// </summary>
        /// <param name="token"></param>
        /// <param name="secret"></param>
        /// <param name="issuer"></param>
        /// <param name="audience"></param>
        /// <returns></returns>
        public Guid? ValidateJwtToken(string token, string secret, string issuer, string audience)
        {
            if (token == null)
                return null;

            var tokenHandler = new JwtSecurityTokenHandler();
            var key = new HMACSHA512(Encoding.UTF8.GetBytes(secret));
            try
            {
                tokenHandler.ValidateToken(token, new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidIssuer = issuer,
                    ValidAudience = audience,
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key.Key),
                    // set clockskew to zero so tokens expire exactly at token expiration time (instead of 5 minutes later)
                    ClockSkew = TimeSpan.Zero
                }, out SecurityToken validatedToken);

                var jwtToken = (JwtSecurityToken)validatedToken;
                if (Guid.TryParse((jwtToken.Claims.First(x => x.Type == ClaimTypes.NameIdentifier).Value), out var validGuid))
                {
                    return validGuid;
                }
                else
                {
                    return null!;
                }
            }
            catch (Exception err)
            {
                var _errorMessage = err.ToString();
                // return null if validation fails
                return null;
            }
        }
    }
}
