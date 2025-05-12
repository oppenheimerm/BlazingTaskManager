using BlazingTaskManager.Shared.Domain.DTO.Authentication;
using BlazingTaskManager.Shared.Domain.DTO.User;
using BlazingTaskManager.Shared.Responses;
using BlazingTaskManager.Shared.Services.AuthService;
using Microsoft.AspNetCore.Components.Authorization;
using Blazored.LocalStorage;
using System.Security.Claims;
using System.Text.Json;

namespace BlazingTaskManager.Client.AuthState
{
    public class CustomAuthenticationStateProvider : AuthenticationStateProvider
    {
        readonly IJWTUtilities _jwtUtilities;
        readonly IConfiguration _configuration;
        readonly ILocalStorageService _localStorageService;
        //Our IdentityUser Instance
        readonly ClaimsPrincipal anonymous = new(new ClaimsIdentity());
        AuthLocalStorageDTO? AuthLocalStorageDTO { get; set; }

        public CustomAuthenticationStateProvider( IJWTUtilities jwtUtilities, IConfiguration configuration, ILocalStorageService localStorageService)
        {
            _jwtUtilities = jwtUtilities;
            _configuration = configuration;
            _localStorageService = localStorageService;
        }

        public override async Task<AuthenticationState> GetAuthenticationStateAsync()
        {
            try
            {
                var authData = await _localStorageService.GetItemAsStringAsync(_configuration["ApplicationSettings:LocalStorageKey"]!);

                //  User is anonymous / not logged in or authenticated
                if (authData is null)
                    return await Task.FromResult(new AuthenticationState(anonymous));


                //  Not null so get claims
                AuthLocalStorageDTO = JsonSerializer.Deserialize<AuthLocalStorageDTO>(authData);

                if (AuthLocalStorageDTO is not null && AuthLocalStorageDTO.JWtToken is not null && AuthLocalStorageDTO.Id is not null)
                {
                    var getUserClaims = _jwtUtilities.DecryptToken(AuthLocalStorageDTO.JWtToken);
                    if (getUserClaims is null || string.IsNullOrEmpty(getUserClaims.Email) || getUserClaims.Id == Guid.Empty)
                    {
                        return await Task.FromResult(new AuthenticationState(anonymous));
                    }

                    //  Create a claims principal
                    var claimsPrincipal = SetClaimsPrincipal(getUserClaims);
                    NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(claimsPrincipal)));
                    return await Task.FromResult(new AuthenticationState(claimsPrincipal));


                }
                else
                {
                    return await Task.FromResult(new AuthenticationState(anonymous));
                }
            }
            catch(Exception err)
            {
                // Ignore error during prerendering
                return await Task.FromResult(new AuthenticationState(anonymous));
            }
        }

        public static ClaimsPrincipal SetClaimsPrincipal(BTUserClaimsDTO claims)
        {
            if (claims.Email is null) return new ClaimsPrincipal();

            var userClaims = new List<Claim> {
                new Claim(ClaimTypes.NameIdentifier, claims.Id.ToString()!),
                new Claim(ClaimTypes.Name, claims.FirstName!),
                new Claim(ClaimTypes.Email, claims.Email!),
            };

            if (claims.Roles is not null)
            {
                foreach (var role in claims.Roles)
                {
                    userClaims.Add(
                        new Claim(ClaimTypes.Role, role.RoleCode!));
                }
            }


            return new ClaimsPrincipal(
                new ClaimsIdentity(userClaims, "JwtAuth"
                ));
        }

        public async Task UpdateAuthenticatedState(APIResponseAuthentication? apiResponseAuthentication)
        {
            var claimsPrincipal = new ClaimsPrincipal();
            if (apiResponseAuthentication is not null &&
                apiResponseAuthentication.Success &&
                !string.IsNullOrEmpty(apiResponseAuthentication.JwtToken))
            {
                var getUserClaims = _jwtUtilities.DecryptToken(apiResponseAuthentication.JwtToken!);
                if(getUserClaims is not null) {
                    if (getUserClaims.Id.HasValue && getUserClaims.Id != Guid.Empty && !string.IsNullOrEmpty(getUserClaims.Email))
                    {
                        if(apiResponseAuthentication.User is not null)
                        {
                            AuthLocalStorageDTO = new AuthLocalStorageDTO()
                            {
                                JWtToken = apiResponseAuthentication.JwtToken,
                                RefreshToken = apiResponseAuthentication.RefreshToken,
                                TimeStamp = DateTime.UtcNow,
                                Id = apiResponseAuthentication.User.Id
                            };

                            var jsonString = JsonSerializer.Serialize(AuthLocalStorageDTO);

                            claimsPrincipal = SetClaimsPrincipal(getUserClaims);
                            await _localStorageService.SetItemAsStringAsync(_configuration["ApplicationSettings:LocalStorageKey"]!, jsonString);
                        }
                    }
                }
                
            }
            else
            {
                await _localStorageService.RemoveItemAsync(_configuration["ApplicationSettings:LocalStorageKey"]!);
            }
            //  Notify App to rerender
            NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(claimsPrincipal)));
        }
    }
}
