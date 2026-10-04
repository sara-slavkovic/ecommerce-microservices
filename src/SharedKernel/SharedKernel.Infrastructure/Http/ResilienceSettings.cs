using System;
using System.Collections.Generic;
using System.Text;

namespace SharedKernel.Infrastructure.Http
{
    public class ResilienceSettings
    {
        public int MaxRetryAttempts { get; set; } = 3;
        public double DelaySeconds { get; set; } = 1;
        public double AttemptTimeoutSeconds { get; set; } = 30; // Default to 30
        public double TotalRequestTimeoutSeconds { get; set; } = 60;
        public double CircuitBreakerFailureRatio { get; set; } = 0.5;
        public int CircuitBreakerMinimumThroughput { get; set; } = 4;
        public double CircuitBreakerSamplingDurationSeconds { get; set; } = 60;
        public double CircuitBreakerBreakDurationSeconds { get; set; } = 15;
    }
}
