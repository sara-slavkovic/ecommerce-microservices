using Microsoft.Extensions.Caching.Memory;
using MockPaymentGateway.Api.DTOs;
using MockPaymentGateway.Api.Enums;

namespace MockPaymentGateway.Api.Services
{
    public class PaymentSimulatorService : IPaymentSimulatorService
    {
        private readonly IMemoryCache _cache;
        private readonly ILogger<PaymentSimulatorService> _logger;
        private static readonly Random _random = new();

        public PaymentSimulatorService(IMemoryCache cache, ILogger<PaymentSimulatorService> logger)
        {
            _cache = cache;
            _logger = logger;
        }

        public async Task<ChargeResultDto> SimulateChargeAsync(ChargeRequestDto request)
        {
            // 1. TRUE IDEMPOTENCY CHECK (Prevents Double Charging)
            var idempotencyCacheKey = $"idempotency_{request.IdempotencyKey}";
            if (!string.IsNullOrWhiteSpace(request.IdempotencyKey))
            {
                if (_cache.TryGetValue(idempotencyCacheKey, out ChargeResultDto? cachedResult) && cachedResult != null)
                {
                    _logger.LogInformation("=> IDEMPOTENCY HIT! Returning previous exact result for Key {Key} (Double-charge prevented!)", request.IdempotencyKey);
                    return cachedResult; // Return the exact same response as last time!
                }
            }

            // LOG 1: Prove the gateway received a fresh request (this will print multiple times during retries!)
            _logger.LogInformation("=> Gateway received Charge Request for Order {OrderId} in Mode: {Mode}", request.OrderId, request.SimulationMode);

            // Execute the simulation to get the result
            var result = await ProcessSimulationAsync(request);

            // 2. SAVE FINAL RESULT TO IDEMPOTENCY CACHE
            // We ONLY cache final, definitive outcomes (200 Success, 402 Fatal Decline, etc.)
            // We DO NOT cache 503 Transient failures, because we want the client (Polly) to retry those
            if (!string.IsNullOrWhiteSpace(request.IdempotencyKey) && result.StatusCode != 503)
            {
                // Store the final outcome for 24 hours
                _cache.Set(idempotencyCacheKey, result, TimeSpan.FromHours(24));
            }

            return result;
        }

        private async Task<ChargeResultDto> ProcessSimulationAsync(ChargeRequestDto request)
        {
            switch (request.SimulationMode)
            {
                case SimulationMode.AlwaysSucceed:
                    return Success();

                case SimulationMode.AlwaysFail:
                    return Failure("Simulated gateway failure.", 503);

                case SimulationMode.SlowResponse:
                    _logger.LogWarning("Delaying response for 5 seconds...");
                    await Task.Delay(5000); // for timeout
                    return Success();

                case SimulationMode.FailNTimesThenSucceed:
                    return HandleFailNTimesThenSucceed(request);

                case SimulationMode.AlwaysFailFatal:
                    return Failure("Payment declined: invalid card details.", 402);

                case SimulationMode.Random:
                default:
                    return HandleRandom();
            }
        }

        private ChargeResultDto HandleRandom()
        {
            var roll = _random.NextDouble();

            // 70% success, 20% transient failure (retryable), 10% fatal failure (not retryable)
            if (roll < 0.7)
            {
                // LOG 2: Show the random outcome
                _logger.LogInformation("Random roll outcome: Success");
                return Success();
            }

            if (roll < 0.9)
            {
                _logger.LogWarning("Random roll outcome: Transient Failure (503)");
                return Failure("Random transient gateway failure.", 503);
            }

            _logger.LogError("Random roll outcome: Fatal Failure (402)");
            return Failure("Payment declined: insufficient funds.", 402);
        }

        private ChargeResultDto HandleFailNTimesThenSucceed(ChargeRequestDto request)
        {
            if (string.IsNullOrWhiteSpace(request.IdempotencyKey))
            {
                _logger.LogError("Rejecting request: Missing IdempotencyKey");
                return Failure("IdempotencyKey is REQUIRED for FailNTimesThenSucceed mode.", 400);
            }

            var cacheKey = $"attempts:{request.IdempotencyKey}";
            var attemptCount = _cache.GetOrCreate(cacheKey, entry =>
            {
                entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5);
                return 0;
            });

            attemptCount++;
            _cache.Set(cacheKey, attemptCount, TimeSpan.FromMinutes(5));

            // LOG 3: The ultimate proof of the retry cycle
            _logger.LogWarning("Processing attempt {AttemptCount} of {RequiredFails} required fails before success.", attemptCount, request.FailCount);

            if (attemptCount <= request.FailCount)
                return Failure($"Simulated failure, attempt {attemptCount} of {request.FailCount}.", 503);

            _logger.LogInformation("Required failures met. Returning Success!");
            return Success();
        }

        private static ChargeResultDto Success() => new()
        {
            Success = true,
            TransactionId = Guid.NewGuid().ToString(),
            Message = "Payment approved.",
            StatusCode = 200
        };

        private static ChargeResultDto Failure(string message, int statusCode) => new()
        {
            Success = false,
            TransactionId = string.Empty,
            Message = message,
            StatusCode = statusCode
        };
    }
}
