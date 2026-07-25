using PaymentService.Application.DTOs;
using PaymentService.Application.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace PaymentService.Infrastructure.Handlers
{
    public class PaymentAttemptTrackingHandler : DelegatingHandler
    {
        // A strongly-typed key to identify our tracking list in the request options
        public static readonly HttpRequestOptionsKey<List<PaymentAttemptRecord>> TrackingKey = new("PaymentAttempts");

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var startedAt = DateTime.Now;
            HttpResponseMessage? response = null;
            Exception? exception = null;

            try
            {
                response = await base.SendAsync(request, cancellationToken);
                return response;
            }
            catch (Exception ex)
            {
                exception = ex;
                throw;
            }
            finally
            {
                var finishedAt = DateTime.Now;

                // Extract the list from the request options
                if (request.Options.TryGetValue(TrackingKey, out var attempts) && attempts != null)
                {
                    int statusCode;
                    string? errorMessage = null;
                    if (exception != null)
                    {
                        errorMessage = exception.Message;
                        statusCode = exception switch
                        {
                            TaskCanceledException => 504, // Gateway Timeout
                            HttpRequestException => 503,  // Service Unreachable
                            _ => 500                      // Server Error
                        };
                    }
                    else if (response != null && !response.IsSuccessStatusCode)
                    {
                        statusCode = (int)response.StatusCode;
                        errorMessage = $"Gateway returned HTTP {statusCode}";
                    }
                    else
                    {
                        statusCode = 200; // Success
                    }

                    attempts.Add(new PaymentAttemptRecord(
                        attempts.Count + 1,
                        statusCode,
                        errorMessage,
                        startedAt,
                        finishedAt,
                        (int)(finishedAt - startedAt).TotalMilliseconds
                    ));
                }
            }
        }
    }
}
