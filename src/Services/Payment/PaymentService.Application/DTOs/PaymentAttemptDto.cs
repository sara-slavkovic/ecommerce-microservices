using System;
using System.Collections.Generic;
using System.Text;

namespace PaymentService.Application.DTOs
{
    public class PaymentAttemptDto
    {
        public Guid Id { get; set; }
        public int AttemptNumber { get; set; }
        public int StatusCode { get; set; }
        public string ErrorMessage { get; set; } = string.Empty;
        public DateTime StartedAt { get; set; }
        public DateTime FinishedAt { get; set; }
        public int DurationMs { get; set; }
    }
}
