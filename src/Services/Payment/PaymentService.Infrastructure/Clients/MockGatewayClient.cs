using PaymentService.Application.DTOs;
using PaymentService.Application.Interfaces;
using PaymentService.Infrastructure.Handlers;
using Polly.CircuitBreaker;
using Polly.Timeout;
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
            // 1. Create a fresh list to hold the attempts for this specific call
            var attemptsTracker = new List<PaymentAttemptRecord>();

            // 2. Construct the request and attach the tracking list
            var request = new HttpRequestMessage(HttpMethod.Post, "/api/gateway/charge")
            {
                Content = JsonContent.Create(dto)
            };
            request.Options.Set(PaymentAttemptTrackingHandler.TrackingKey, attemptsTracker);

            try
            {
                // 3. Send it (Polly retries will happen inside here)
                var response = await _httpClient.SendAsync(request);

                // Try deserializing the gateway's ChargeResultDto (returned on both 200 OK and 503/402 failures)
                var result = await response.Content.ReadFromJsonAsync<ChargeResultDto>();
                if (result == null)
                {
                    // Fallback if the body wasn't a valid ChargeResultDto
                    var errorContent = await response.Content.ReadAsStringAsync();
                    result = new ChargeResultDto
                    {
                        Success = false,
                        StatusCode = (int)response.StatusCode,
                        Message = $"Gateway returned HTTP {(int)response.StatusCode}: {errorContent}"
                    };
                }

                // 4. Attach the tracked attempts to the result
                result.Attempts = attemptsTracker;
                return result;
            }
            catch (Exception ex)
            {
                // Align exactly with GlobalExceptionHandler mapping
                int statusCode = ex switch
                {
                    TimeoutRejectedException or TaskCanceledException => 504,
                    BrokenCircuitException or HttpRequestException => 503,
                    _ => 500
                };

                // Handles connection refused, wrong port, timeouts, etc.
                return new ChargeResultDto
                {
                    Success = false,
                    StatusCode = statusCode,
                    Message = $"Failed to reach Payment Gateway: {ex.Message}",
                    Attempts = attemptsTracker // Attach attempts even if it ultimately crashed
                };
            }
        }
    }
}
