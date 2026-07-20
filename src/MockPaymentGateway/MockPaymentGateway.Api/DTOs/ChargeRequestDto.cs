using MockPaymentGateway.Api.Enums;

namespace MockPaymentGateway.Api.DTOs
{
    public class ChargeRequestDto
    {
        public Guid OrderId { get; set; }
        public decimal Amount { get; set; }
        public string IdempotencyKey { get; set; } = string.Empty;
        public SimulationMode SimulationMode { get; set; } = SimulationMode.Random;
        public int FailCount { get; set; } = 0;
    }
}
