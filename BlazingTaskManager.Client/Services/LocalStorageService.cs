﻿using Microsoft.JSInterop;

namespace BlazingTaskManager.Client.Services
{
    /// <summary>
    /// LocalStorageService class facilitates interaction with the browser's local storage through JavaScript.
    /// </summary>
    public class LocalStorageService
    {
        private readonly IJSRuntime _jsRuntime;

        public LocalStorageService(IJSRuntime jsRuntime)
        {
            _jsRuntime = jsRuntime;
        }

        public async Task AddItem(string key, string value)
        {
            await _jsRuntime.InvokeVoidAsync("localStorage.setItem", key, value);
        }

        public async Task RemoveItem(string key)
        {
            await _jsRuntime.InvokeVoidAsync("localStorage.removeItem", key);
        }

        public async Task<string> GetItem(string key)
        {
            return await _jsRuntime.InvokeAsync<string>("localStorage.getItem", key);
        }
    }
}
