using BlazingTaskManager.Shared.Domain.DTO.Authentication;
using BlazingTaskManager.Shared.Responses;
using Blazored.LocalStorage;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Text;
using Microsoft.AspNetCore.Components;

namespace BlazingTaskManager.Client.Services
{
    public class ClientService : BaseService, IClientService
    {
        readonly HttpClient? _httpClient;
        readonly IConfiguration _configuration;
        readonly ILocalStorageService _localStorageService;

        public ClientService(HttpClient httpClient, IConfiguration configuration, ILocalStorageService localStorageService)
            : base(httpClient, configuration, localStorageService)
        {
            _httpClient = httpClient;
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
            _localStorageService = localStorageService ?? throw new ArgumentNullException(nameof(localStorageService));
        }

        /// <summary>
        /// LoginAsync method to authenticate user and get JWT token
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        /// <exception cref="InvalidOperationException"></exception>
        public async Task<APIResponseAuthentication> LoginAsync(AuthenticateRequestDTO dto)
        {
            if (_configuration == null)
            {
                throw new InvalidOperationException("Configuration is not initialized.");
            }

            string? baseUrl = _configuration["ConnectionStrings:AccountAPIBaseURL"];
            if (string.IsNullOrEmpty(baseUrl))
            {
                throw new InvalidOperationException("Base URL is not configured.");
            }

            if (_httpClient == null)
            {
                throw new InvalidOperationException("HttpClient is not initialized.");
            }

            var paylod = JsonSerializer.Serialize(dto);
            var request = new HttpRequestMessage(HttpMethod.Post, $"{baseUrl}/login");
            request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            request.Content = new StringContent(paylod, Encoding.UTF8);
            request.Content.Headers.ContentType = new MediaTypeHeaderValue("application/json");

            var response = await _httpClient.PostAsJsonAsync($"{baseUrl}/login", dto);
            var result = await response.Content.ReadFromJsonAsync<APIResponseAuthentication>();
            return result!;
        }

        /// <summary>
        /// GetRefreshTokenAsync method to get new JWT token using refresh token
        /// </summary>
        /// <param name="navigationManager"></param>
        /// <param name="localStorageDTO"></param>
        /// <returns></returns>
        public async Task<APIResponseBTUserDTO?> GetAccount(NavigationManager navigationManager, AuthLocalStorageDTO localStorageDTO)
        {
            if (localStorageDTO is not null && localStorageDTO.JWtToken is not null)
            {
                _httpClient!.DefaultRequestHeaders.Authorization =
                    new AuthenticationHeaderValue("Bearer", localStorageDTO.JWtToken);


                var response = await _httpClient.GetAsync($"{_configuration["ConnectionStringsBaseGatewayURL" + "/accounts"]}/{localStorageDTO.Id}")!;
                bool check = CheckIfUnauthroized(response);
                if (check)
                {
                    var localStorageDTORefresh = await GetRefreshTokenAsync(navigationManager);
                    if (localStorageDTORefresh is not null && !string.IsNullOrEmpty(localStorageDTORefresh.JWtToken) && localStorageDTORefresh.Id != Guid.Empty)
                    {
                        return await GetAccount(navigationManager, localStorageDTORefresh);
                    }
                    else
                    {
                        navigationManager.NavigateTo("/Account/Login", true);
                    }

                }

                return await response.Content.ReadFromJsonAsync<APIResponseBTUserDTO>();
            }
            else
            {
                return new APIResponseBTUserDTO() { Message = "Not Authorized", Success = false, User = null! };
            }
        }
    }
}
