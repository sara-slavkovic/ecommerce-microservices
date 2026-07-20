using System;
using System.Collections.Generic;
using System.Text;

namespace PaymentService.Domain.Entities
{
    public class PaymentAttempt
    {
        public Guid Id { get; set; }
        public Guid PaymentId { get; set; }
        public int AttemptNumber { get; set; }
        public int StatusCode { get; set; }
        public string ErrorMessage { get; set; } = string.Empty;
        public DateTime StartedAt { get; set; }
        public DateTime FinishedAt { get; set; }
        public int DurationMs { get; set; }

        public Payment Payment { get; set; } = null!;
    }
}
