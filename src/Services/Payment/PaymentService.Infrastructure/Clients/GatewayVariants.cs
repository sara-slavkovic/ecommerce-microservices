using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Http.Resilience;
using PaymentService.Infrastructure.Handlers;
using Polly;
using System;
using System.Collections.Generic;
using System.Text;

namespace PaymentService.Infrastructure.Clients
{
    public static class GatewayVariants
    {
        public static IServiceCollection AddGatewayVariants(this IServiceCollection services, Action<HttpClient> configureClient, Action<IHttpClientBuilder> addV1Resilience)
        {
            IHttpClientBuilder Create(string v) => services.AddHttpClient($"gateway-{v}", configureClient);

            // V0: no Polly
            Create("V0").AddHttpMessageHandler<PaymentAttemptTrackingHandler>();

            // V1: thesis configuration (Table 8), tracking handler goes after it
            var v1 = Create("V1");
            addV1Resilience(v1);
            v1.AddHttpMessageHandler<PaymentAttemptTrackingHandler>();

            // V2: Retry only, 3 retries, no delay
            RetryOnly(Create("V2"), 3, TimeSpan.Zero, DelayBackoffType.Constant, false);

            // R1, R2, R3, R5: Retry only, 1 s delay, exponential, jitter
            foreach (var n in new[] { 1, 2, 3, 5 })
                RetryOnly(Create($"R{n}"), n, TimeSpan.FromSeconds(1), DelayBackoffType.Exponential, true);

            // V3: Circuit Breaker only, same values as in Table 8
            var v3 = Create("V3");
            v3.AddResilienceHandler("V3-cb", b => b.AddCircuitBreaker(new HttpCircuitBreakerStrategyOptions
            {
                FailureRatio = 0.5,
                MinimumThroughput = 4,
                SamplingDuration = TimeSpan.FromSeconds(60),
                BreakDuration = TimeSpan.FromSeconds(15)
            }));
            v3.AddHttpMessageHandler<PaymentAttemptTrackingHandler>();

            return services;
        }

        private static void RetryOnly(IHttpClientBuilder b, int retries, TimeSpan delay, DelayBackoffType type, bool jitter)
        {
            b.AddResilienceHandler($"{b.Name}-retry", p => p.AddRetry(new HttpRetryStrategyOptions
            {
                MaxRetryAttempts = retries,
                Delay = delay,
                BackoffType = type,
                UseJitter = jitter
            }));
            b.AddHttpMessageHandler<PaymentAttemptTrackingHandler>();
        }
    }
}
