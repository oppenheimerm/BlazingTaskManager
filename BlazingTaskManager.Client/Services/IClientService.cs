using BlazingTaskManager.Shared.Domain.DTO.Authentication;
using BlazingTaskManager.Shared.Responses;
using Microsoft.AspNetCore.Components;

namespace BlazingTaskManager.Client.Services
{
    public interface IClientService
    {
        /// <summary>
        /// LoginAsync method to authenticate user and get JWT token
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        /// <exception cref="InvalidOperationException"></exception>
        Task<APIResponseAuthentication> LoginAsync(AuthenticateRequestDTO dto);

        /// <summary>
        /// GetRefreshTokenAsync method to get new JWT token using refresh token
        /// </summary>
        /// <param name="navigationManager"></param>
        /// <param name="localStorageDTO"></param>
        /// <returns></returns>
        Task<APIResponseBTUserDTO?> GetAccount(NavigationManager navigationManager, AuthLocalStorageDTO localStorageDTO);
    }
}
