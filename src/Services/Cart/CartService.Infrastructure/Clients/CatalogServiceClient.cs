using CartService.Application.DTOs;
using CartService.Application.Interfaces;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http.Json;
using System.Text;

namespace CartService.Infrastructure.Clients
{
    public class CatalogServiceClient : ICatalogServiceClient
    {
        private readonly HttpClient _httpClient;

        public CatalogServiceClient(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<ProductSnapshotDto?> GetProductSnapshotByIdAsync(Guid productId)
        {
            var response = await _httpClient.GetAsync($"/api/products/{productId}");

            if (!response.IsSuccessStatusCode)
            {
                if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    return null;
                }

                var errorMessage = await response.Content.ReadAsStringAsync();
                throw new InvalidOperationException($"Failed to fetch product. {errorMessage}");
            }

            return await response.Content.ReadFromJsonAsync<ProductSnapshotDto>();
        }
    }
}
