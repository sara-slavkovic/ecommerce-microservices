using PaymentService.Application.DTOs;
using PaymentService.Application.Interfaces;
using SharedKernel.Domain.Exceptions;
using SharedKernel.Infrastructure.Http;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http.Json;
using System.Text;

namespace PaymentService.Infrastructure.Clients
{
    public class OrderServiceClient : IOrderServiceClient
    {
        private readonly HttpClient _httpClient;

        public OrderServiceClient(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<OrderSnapshotDto?> GetOrderByIdAsync(Guid orderId)
        {
            var response = await _httpClient.GetAsync($"/api/orders/{orderId}");

            if (response.StatusCode == HttpStatusCode.NotFound)
                return null;

            await response.EnsureSuccessOrThrowAsync("get order");
            return await response.Content.ReadFromJsonAsync<OrderSnapshotDto>();
        }

        public async Task CompleteOrderAsync(Guid orderId)
        {
            var response = await _httpClient.PostAsync($"/api/orders/{orderId}/complete", null);
            await response.EnsureSuccessOrThrowAsync("complete order");
        }

        public async Task CancelOrderAsync(Guid orderId)
        {
            var response = await _httpClient.PostAsync($"/api/orders/{orderId}/cancel", null);
            await response.EnsureSuccessOrThrowAsync("cancel order");
        }
    }
}
