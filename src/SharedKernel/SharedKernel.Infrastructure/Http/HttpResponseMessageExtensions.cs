using SharedKernel.Domain.Exceptions;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Text.Json;

namespace SharedKernel.Infrastructure.Http
{
    public static class HttpResponseMessageExtensions
    {
        // "this HttpResponseMessage response" means we are adding a method directly to the response object
        public static async Task EnsureSuccessOrThrowAsync(this HttpResponseMessage response, string actionDescription)
        {
            if (response.IsSuccessStatusCode)
                return;

            var rawContent = await response.Content.ReadAsStringAsync();
            string cleanDetail = rawContent;

            // Extract the clean 'detail' field from standard ASP.NET ProblemDetails JSON
            try
            {
                using var jsonDoc = JsonDocument.Parse(rawContent);
                if (jsonDoc.RootElement.TryGetProperty("detail", out var detailProperty))
                {
                    cleanDetail = detailProperty.GetString() ?? rawContent;
                }
            }
            catch (JsonException)
            {
                // Fallback to raw string if response is not valid JSON
            }

            // Map status codes cleanly to Domain Exceptions
            if (response.StatusCode == HttpStatusCode.NotFound)
                throw new NotFoundException($"Not found while trying to {actionDescription}. {cleanDetail}");

            if (response.StatusCode == HttpStatusCode.Conflict ||
               ((int)response.StatusCode >= 400 && (int)response.StatusCode < 500))
            {
                throw new ConflictException($"Failed to {actionDescription}: {cleanDetail}");
            }

            // 500 range errors
            throw new ServiceUnavailableException(
                $"Failed to {actionDescription}. Downstream returned HTTP {(int)response.StatusCode}. {cleanDetail}");
        }
    }
}
