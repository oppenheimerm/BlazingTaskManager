using BlazingTaskManager.Shared.Domain.DTO.Authentication;
using BlazingTaskManager.Shared.Responses;
using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components;
using System.Text.Json;

namespace BlazingTaskManager.Client.Services
{
    public class BaseService
    {
        readonly HttpClient _httpClient;
        readonly IConfiguration _iconfig;
        readonly ILocalStorageService _localStorageService;
        const string _localStorageKey = "BT_LS";
        public BaseService(HttpClient httpClient, IConfiguration iconfig, ILocalStorageService localStorageService)
        {
            _httpClient = httpClient;
            _iconfig = iconfig;
            _localStorageService = localStorageService;
        }
        public async Task<AuthLocalStorageDTO?> GetRefreshTokenAsync(NavigationManager navigationManager)
        {
            var authData = await _localStorageService.GetItemAsStringAsync(_localStorageKey);
            if (string.IsNullOrEmpty(authData))
            {
                return null!;
            }
            else
            {
                var authLocalStorageDTO = JsonSerializer.Deserialize<AuthLocalStorageDTO>(authData);
                if (authLocalStorageDTO is not null && !string.IsNullOrEmpty(authLocalStorageDTO.RefreshToken))
                {

                    var baseUrl = _iconfig["ConnectionStrings:AccountAPIBaseURL"];
                    //  APIResponseAuthentication
                    var response = await _httpClient.PostAsJsonAsync($"{baseUrl}/refresh-token", new RefreshTokenRequestDTO() { RefreshToken = authLocalStorageDTO.RefreshToken });
                    var result = await response.Content.ReadFromJsonAsync<APIResponseAuthentication>();
                    if (result is not null && result.Success == true)
                    {
                        var authLocalStorage = new AuthLocalStorageDTO()
                        { JWtToken = result.JwtToken, RefreshToken = result.RefreshToken, TimeStamp = DateTime.UtcNow, Id = result.User!.Id };
                        var jsonString = JsonSerializer.Serialize(authLocalStorage);
                        await _localStorageService.SetItemAsStringAsync(_localStorageKey, jsonString);
                        return authLocalStorageDTO;
                    }
                    else
                    {
                        return null!;
                    }
                }
                else
                {
                    return null!;
                }

            }


        }


        public static bool CheckIfUnauthroized(HttpResponseMessage httpResposneMessage)
        {
            if (httpResposneMessage.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                return true;
            else
                return false;
        }
    }
}
