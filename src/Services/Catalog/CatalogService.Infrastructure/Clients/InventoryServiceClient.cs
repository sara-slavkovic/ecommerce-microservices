using CatalogService.Application.DTOs;
using CatalogService.Application.Interfaces;
using SharedKernel.Domain.Exceptions;
using SharedKernel.Infrastructure.Http;
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

            var response = await _httpClient.PostAsJsonAsync("/api/inventories/internal", request);
            await response.EnsureSuccessOrThrowAsync("create inventory item");
        }

        public async Task DeleteInventoryItemByProductIdAsync(Guid productId)
        {
            var response = await _httpClient.DeleteAsync($"/api/inventories/product/{productId}");

            if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                return;

            await response.EnsureSuccessOrThrowAsync("delete inventory item");
        }
    }
}
