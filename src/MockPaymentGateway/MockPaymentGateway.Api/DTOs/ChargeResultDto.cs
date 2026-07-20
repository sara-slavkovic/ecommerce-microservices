namespace MockPaymentGateway.Api.DTOs
{
    public class ChargeResultDto
    {
        public bool Success { get; set; }
        public string TransactionId { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
        public int StatusCode { get; set; } = 200;
    }
}
