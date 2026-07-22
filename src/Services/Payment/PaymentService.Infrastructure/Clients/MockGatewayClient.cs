using PaymentService.Application.DTOs;
using PaymentService.Application.Interfaces;
using System;
using System.Collections.Generic;
using System.Net.Http.Json;
using System.Text;

namespace PaymentService.Infrastructure.Clients
{
    public class MockGatewayClient : IMockGatewayClient
    {
        private readonly HttpClient _httpClient;

        public MockGatewayClient(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<ChargeResultDto> ChargeAsync(ChargeRequestDto dto)
        {
           try
            {
                var response = await _httpClient.PostAsJsonAsync("/api/gateway/charge", dto);

                // Try deserializing the gateway's ChargeResultDto (returned on both 200 OK and 503/402 failures)
                var result = await response.Content.ReadFromJsonAsync<ChargeResultDto>();
                if (result != null)
                {
                    return result;
                }

                // Fallback if the body wasn't a valid ChargeResultDto
                var errorContent = await response.Content.ReadAsStringAsync();
                return new ChargeResultDto
                {
                    Success = false,
                    StatusCode = (int)response.StatusCode,
                    Message = $"Gateway returned HTTP {(int)response.StatusCode}: {errorContent}"
                };
            }
            catch (Exception ex)
            {
                // Handles connection refused, wrong port, timeouts, etc.
                return new ChargeResultDto
                {
                    Success = false,
                    StatusCode = 503,
                    Message = $"Failed to reach Payment Gateway: {ex.Message}"
                };
            }
        }
    }
}
