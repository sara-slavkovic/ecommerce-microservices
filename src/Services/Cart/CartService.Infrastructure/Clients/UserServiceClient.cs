using CartService.Application.DTOs;
using CartService.Application.Interfaces;
using SharedKernel.Infrastructure.Http;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http.Json;
using System.Text;

namespace CartService.Infrastructure.Clients
{
    public class UserServiceClient : IUserServiceClient
    {
        private readonly HttpClient _httpClient;

        public UserServiceClient(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<UserSnapshotDto?> GetUserSnapshotByIdAsync(Guid userId)
        {
            var response = await _httpClient.GetAsync($"/api/users/{userId}/snapshot");

            if (response.StatusCode == HttpStatusCode.NotFound)
                return null;

            await response.EnsureSuccessOrThrowAsync("get user snapshot");
            return await response.Content.ReadFromJsonAsync<UserSnapshotDto>();
        }
    }
}
