using CartService.Application.DTOs;
using CartService.Application.Interfaces;
using SharedKernel.Domain.Exceptions;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http.Json;
using System.Text;

namespace CartService.Infrastructure.Clients
{
    public class InventoryServiceClient : IInventoryServiceClient
    {
        private readonly HttpClient _httpClient;

        public InventoryServiceClient(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<InventoryAvailabilityDto?> GetInventoryByProductIdAsync(Guid productId)
        {
            var response = await _httpClient.GetAsync($"/api/inventories/product/{productId}");

            if (!response.IsSuccessStatusCode)
            {
                if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    return null;
                }

                var errorMessage = await response.Content.ReadAsStringAsync();
                throw new ServiceUnavailableException($"Failed to fetch inventory item. {errorMessage}");
            }

            return await response.Content.ReadFromJsonAsync<InventoryAvailabilityDto>();
        }
    }
}
