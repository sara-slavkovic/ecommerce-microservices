using CartService.Application.DTOs;
using CartService.Application.Interfaces;
using SharedKernel.Domain.Exceptions;
using SharedKernel.Infrastructure.Http;
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

            if (response.StatusCode == HttpStatusCode.NotFound)
                return null;

            await response.EnsureSuccessOrThrowAsync("fetch inventory item");
            return await response.Content.ReadFromJsonAsync<InventoryAvailabilityDto>();
        }
    }
}
