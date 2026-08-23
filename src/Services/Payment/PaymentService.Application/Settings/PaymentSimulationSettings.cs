using System;
using System.Collections.Generic;
using System.Text;

namespace PaymentService.Application.Settings
{
    public record PaymentSimulationSettings
    {
        public string Mode { get; init; } = string.Empty;
        public int FailCount { get; init; }
    }
}
