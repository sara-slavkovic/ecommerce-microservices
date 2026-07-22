using MockPaymentGateway.Api.DTOs;

namespace MockPaymentGateway.Api.Services
{
    public interface IPaymentSimulatorService
    {
        Task<ChargeResultDto> SimulateChargeAsync(ChargeRequestDto request);
    }
}
