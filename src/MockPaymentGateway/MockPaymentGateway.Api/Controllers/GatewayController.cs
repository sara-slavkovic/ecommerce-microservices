using Microsoft.AspNetCore.Mvc;
using MockPaymentGateway.Api.DTOs;
using MockPaymentGateway.Api.Services;
using Swashbuckle.AspNetCore.Annotations;

namespace MockPaymentGateway.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class GatewayController : ControllerBase
    {
        private readonly IPaymentSimulator _paymentSimulator;

        public GatewayController(IPaymentSimulator paymentSimulator)
        {
            _paymentSimulator = paymentSimulator;
        }

        [HttpPost("charge")]
        [SwaggerOperation(Summary = "Simulate a payment gateway charge")]
        public async Task<IActionResult> Charge([FromBody] ChargeRequestDto request)
        {
            var result = await _paymentSimulator.SimulateChargeAsync(request);
            return StatusCode(result.StatusCode, result);
        }
    }
}
