using CatalogService.Application.DTOs;
using CatalogService.Application.Interfaces;
using Microsoft.Extensions.Configuration;
using SharedKernel.Domain.Exceptions;
using System;
using System.Collections.Generic;
using System.Net.Http.Json;
using System.Text;

namespace CatalogService.Infrastructure.Clients
{

    public class InventoryServiceClient : IInventoryServiceClient
    {
        private readonly HttpClient _httpClient;
        private readonly string _internalApiKey;

        public InventoryServiceClient(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _internalApiKey = configuration["InternalApiKey"] ?? throw new ArgumentNullException("InternalApiKey configuration is missing.");
        }

        public async Task CreateInventoryItemAsync(Guid productId, int availableQuantity)
        {
            var request = new CreateInventoryItemRequestDto
            {
                ProductId = productId,
                AvailableQuantity = availableQuantity
            };

            var httpRequest = new HttpRequestMessage(HttpMethod.Post, "/api/inventories/internal")
            {
                Content = JsonContent.Create(request)
            };
            httpRequest.Headers.Add("X-Internal-Api-Key", _internalApiKey);

            var response = await _httpClient.SendAsync(httpRequest);

            if (!response.IsSuccessStatusCode)
            {
                var errorMessage = await response.Content.ReadAsStringAsync();
                throw new ServiceUnavailableException($"Failed to create inventory item. {errorMessage}");
            }
        }

        public async Task DeleteInventoryItemByProductIdAsync(Guid productId)
        {
            var response = await _httpClient.DeleteAsync($"/api/inventories/product/{productId}");

            if (!response.IsSuccessStatusCode && response.StatusCode != System.Net.HttpStatusCode.NotFound)
            {
                var errorMessage = await response.Content.ReadAsStringAsync();
                throw new ServiceUnavailableException($"Failed to delete inventory item. {errorMessage}");
            }
        }
    }
}
