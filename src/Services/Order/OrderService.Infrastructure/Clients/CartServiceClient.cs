using OrderService.Application.DTOs;
using OrderService.Application.Interfaces;
using SharedKernel.Domain.Exceptions;
using System;
using System.Collections.Generic;
using System.Net.Http.Json;
using System.Text;

namespace OrderService.Infrastructure.Clients
{
    public class CartServiceClient : ICartServiceClient
    {
        private readonly HttpClient _httpClient;

        public CartServiceClient(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<CartSnapshotDto?> GetCartByUserIdAsync(Guid userId)
        {
            var response = await _httpClient.GetAsync($"/api/carts/user/{userId}");

            if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                return null;

            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadAsStringAsync();
                throw new ServiceUnavailableException($"Failed to get cart. {error}");
            }

            return await response.Content.ReadFromJsonAsync<CartSnapshotDto>();
        }

        public async Task DeleteCartAsync(Guid userId)
        {
            var response = await _httpClient.DeleteAsync($"/api/carts/user/{userId}");

            if (!response.IsSuccessStatusCode && response.StatusCode != System.Net.HttpStatusCode.NotFound)
            {
                var error = await response.Content.ReadAsStringAsync();
                throw new ServiceUnavailableException($"Failed to delete cart. {error}");
            }
        }
    }
}
