using PaymentService.Application.DTOs;
using System;
using System.Collections.Generic;
using System.Text;

namespace PaymentService.Application.Settings
{
    public sealed class ExperimentState
    {
        // default V1 = Polly configuration from the thesis, so the app behaves as before
        private volatile ExperimentConfigDto _current = new() { Variant = "V1" };
        public ExperimentConfigDto Current => _current;
        public void Set(ExperimentConfigDto config) => _current = config;
    }
}
