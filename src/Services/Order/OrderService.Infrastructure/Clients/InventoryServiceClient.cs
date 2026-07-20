using OrderService.Application.DTOs;
using OrderService.Application.Interfaces;
using SharedKernel.Domain.Exceptions;
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

            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadAsStringAsync();
                throw new ServiceUnavailableException($"Failed to reserve stock. {error}");
            }
        }

        public async Task ReleaseStockAsync(InventoryChangeDto dto)
        {
            var response = await _httpClient.PostAsJsonAsync("/api/inventories/release", dto);

            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadAsStringAsync();
                throw new ServiceUnavailableException($"Failed to release stock. {error}");
            }
        }

        public async Task ConfirmStockDeductionAsync(InventoryChangeDto dto)
        {
            var response = await _httpClient.PostAsJsonAsync("/api/inventories/confirm", dto);

            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadAsStringAsync();
                throw new ServiceUnavailableException($"Failed to confirm stock deduction. {error}");
            }
        }
    }
}
