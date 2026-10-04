using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Polly;
using System;
using System.Collections.Generic;
using System.Text;

namespace SharedKernel.Infrastructure.Http
{
    public static class HttpClientBuilderExtensions
    {
        public static IHttpClientBuilder AddCustomResilienceHandler(this IHttpClientBuilder builder, IConfiguration configuration, string configSectionName)
        {
            // 1. Bind the settings from appsettings.json
            var settings = new ResilienceSettings();
            configuration.GetSection(configSectionName).Bind(settings);

            // 2. Configure Polly resilience policies
            builder.AddStandardResilienceHandler(options =>
            {
                options.Retry.MaxRetryAttempts = settings.MaxRetryAttempts;
                options.Retry.BackoffType = DelayBackoffType.Exponential;
                options.Retry.Delay = TimeSpan.FromSeconds(settings.DelaySeconds);
                options.Retry.UseJitter = true;

                options.AttemptTimeout.Timeout = TimeSpan.FromSeconds(settings.AttemptTimeoutSeconds);
                options.TotalRequestTimeout.Timeout = TimeSpan.FromSeconds(settings.TotalRequestTimeoutSeconds);

                options.CircuitBreaker.FailureRatio = settings.CircuitBreakerFailureRatio;
                options.CircuitBreaker.MinimumThroughput = settings.CircuitBreakerMinimumThroughput;
                options.CircuitBreaker.SamplingDuration = TimeSpan.FromSeconds(settings.CircuitBreakerSamplingDurationSeconds);
                options.CircuitBreaker.BreakDuration = TimeSpan.FromSeconds(settings.CircuitBreakerBreakDurationSeconds);
            });

            // 3. Return original builder for further chaining
            return builder;
        }
    }
}
