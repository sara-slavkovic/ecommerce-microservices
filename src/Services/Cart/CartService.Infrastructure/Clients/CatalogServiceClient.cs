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

            if (response.StatusCode == HttpStatusCode.NotFound)
            {
                return null;
            }

            await response.EnsureSuccessOrThrowAsync("fetch product");
            return await response.Content.ReadFromJsonAsync<ProductSnapshotDto>();
        }
    }
}
