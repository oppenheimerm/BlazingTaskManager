using BlazingTaskManager.AuthenticationAPI.Data;
using BlazingTaskManager.AuthenticationAPI.Helpers;
using BlazingTaskManager.Shared.Domain;
using BlazingTaskManager.Shared.Domain.DTO.Authentication;
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
        /// Get instance of <see cref="BTUser"/> by user Id
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        Task<BTUser?> GetUserByIdAsync(Guid Id);

        /// <summary>
        /// Get instance of <see cref="BTUser"/> by email
        /// </summary>
        /// <param name="Email"></param>
        /// <returns></returns>
        Task<BTUser?> GetUserByEmailAsync(string Email);

    }
}
