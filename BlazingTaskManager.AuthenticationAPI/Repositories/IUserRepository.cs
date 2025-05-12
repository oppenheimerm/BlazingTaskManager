using BlazingTaskManager.AuthenticationAPI.Data;
using BlazingTaskManager.AuthenticationAPI.Helpers;
using BlazingTaskManager.Shared.Domain;
using BlazingTaskManager.Shared.Domain.DTO.Authentication;
using BlazingTaskManager.Shared.Domain.DTO.User;
using BlazingTaskManager.Shared.Responses;

namespace BlazingTaskManager.AuthenticationAPI.Repositories
{
    public interface IUserRepository
    {
        /// <summary>
        /// Register a new <see cref="BTUser"/>
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        Task<BaseAPIResponse> RegisterAsync(RegisterRequestDTO model);

        /// <summary>
        /// On successful authentication the API returns a short lived JWT access token that expires after 15 minutes, 
        /// and a refresh token that expires after 7 days in an HTTP Only cookie. The JWT is used for accessing secure 
        /// routes on the API and the refresh token is used for generating new JWT access tokens when (or just before) 
        /// they expire.
        /// </summary>
        /// <param name="model"></param>
        /// <param name="ipAddress"></param>
        /// <returns></returns>
        Task<APIResponseAuthentication> AuthenticateAsync(AuthenticateRequestDTO model, string ipAddress);
        /// <summary>
        /// Get instance of <see cref="BTUser"/> by user Id
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        Task<BTUserDTO?> GetUserByIdAsync(Guid Id);

        /// <summary>
        /// Get instance of <see cref="BTUser"/> by email
        /// </summary>
        /// <param name="Email"></param>
        /// <returns></returns>
        Task<BTUserDTO?> GetUserByEmailAsync(string Email);

        /// <summary>
        /// User must verfiy their email address with the token generated on registration.
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        Task<BaseAPIResponse> VerifyEmailAsync(string token);

    }
}
