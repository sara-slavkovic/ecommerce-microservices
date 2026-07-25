using System;
using System.Collections.Generic;
using System.Text;

namespace PaymentService.Application.DTOs
{
    public record PaymentAttemptRecord(
        int AttemptNumber,
        int StatusCode,
        string? ErrorMessage,
        DateTime StartedAt,
        DateTime FinishedAt,
        int DurationMs
    );
}
