using CatalogService.Application.DTOs;
using CatalogService.Application.Interfaces;
using System;
using System.Collections.Generic;
using System.Net.Http.Json;
using System.Text;

namespace CatalogService.Infrastructure.Clients
{

    public class InventoryServiceClient : IInventoryServiceClient
    {
        private readonly HttpClient _httpClient;

        public InventoryServiceClient(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task CreateInventoryItemAsync(Guid productId, int availableQuantity)
        {
            var request = new CreateInventoryItemRequestDto
            {
                ProductId = productId,
                AvailableQuantity = availableQuantity
            };

            var response = await _httpClient.PostAsJsonAsync("/api/inventories", request);

            if (!response.IsSuccessStatusCode)
            {
                var errorMessage = await response.Content.ReadAsStringAsync();
                throw new InvalidOperationException($"Failed to create inventory item. {errorMessage}");
            }
        }

        public async Task DeleteInventoryItemByProductIdAsync(Guid productId)
        {
            var response = await _httpClient.DeleteAsync($"/api/inventories/product/{productId}");

            if (!response.IsSuccessStatusCode && response.StatusCode != System.Net.HttpStatusCode.NotFound)
            {
                var errorMessage = await response.Content.ReadAsStringAsync();
                throw new InvalidOperationException($"Failed to delete inventory item. {errorMessage}");
            }
        }
    }
}
