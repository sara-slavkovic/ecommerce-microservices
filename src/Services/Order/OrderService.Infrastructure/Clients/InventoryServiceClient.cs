using OrderService.Application.DTOs;
using OrderService.Application.Interfaces;
using SharedKernel.Domain.Exceptions;
using SharedKernel.Infrastructure.Http;
using System;
using System.Collections.Generic;
using System.Net.Http.Json;
using System.Text;

namespace OrderService.Infrastructure.Clients
{
    public class InventoryServiceClient : IInventoryServiceClient
    {
        private readonly HttpClient _httpClient;

        public InventoryServiceClient(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task ReserveStockAsync(InventoryChangeDto dto)
        {
            var response = await _httpClient.PostAsJsonAsync("/api/inventories/reserve", dto);
            await response.EnsureSuccessOrThrowAsync("reserve stock");
        }

        public async Task ReleaseStockAsync(InventoryChangeDto dto)
        {
            var response = await _httpClient.PostAsJsonAsync("/api/inventories/release", dto);
            await response.EnsureSuccessOrThrowAsync("release stock");
        }

        public async Task ConfirmStockDeductionAsync(InventoryChangeDto dto)
        {
            var response = await _httpClient.PostAsJsonAsync("/api/inventories/confirm", dto);
            await response.EnsureSuccessOrThrowAsync("confirm stock deduction");
        }
    }
}
