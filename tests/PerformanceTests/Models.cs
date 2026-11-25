using System;
using System.Collections.Generic;
using System.Text;

namespace PerformanceTests
{
    // Data shapes used by the load test (they mirror the API responses)
    public record UserDto(Guid Id, string Username);
    public record ProductDto(Guid Id);
    public record CreateCartItemDto(Guid UserId, Guid ProductId, int Quantity);
    public record CreateOrderDto(Guid UserId, string Address, string City, string PostalCode, string Country);
    public record OrderDto(Guid Id, Guid UserId, string Status, decimal TotalAmount);
    public record InitiatePaymentDto(Guid OrderId, decimal Amount);
    public record SeededOrder(Guid OrderId, decimal Amount);
    public record AttemptResponse(int AttemptNumber, int StatusCode);
    public record PaymentResponse(string Status, List<AttemptResponse>? PaymentAttempts);

    // One measured request: latency, payment status, number of gateway calls, HTTP status
    public record Sample(double LatencyMs, string PaymentStatus, int Attempts, int Http);
}
