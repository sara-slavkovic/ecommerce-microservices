using MockPaymentGateway.Api.DTOs;

namespace MockPaymentGateway.Api.Services
{
    public interface IPaymentSimulator
    {
        Task<ChargeResultDto> SimulateChargeAsync(ChargeRequestDto request);
    }
}
