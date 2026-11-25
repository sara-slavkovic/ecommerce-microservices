using System;
using System.Collections.Generic;
using System.Text;

namespace PaymentService.Application.DTOs
{
    // Used only by the load test to select the gateway variant and failure rate
    public class ExperimentConfigDto
    {
        public string Variant { get; set; } = "V1";
        public string? Mode { get; set; }
        public double FailureRate { get; set; }
    }
}
