using Microsoft.AspNetCore.Mvc;
using PaymentService.Application.DTOs;
using PaymentService.Application.Settings;
using Swashbuckle.AspNetCore.Annotations;

namespace PaymentService.Api.Controllers
{
    // Used only by the load tests to switch the gateway variant and failure rate while the service is running
    [ApiController]
    [Route("api/[controller]")]
    public class ExperimentController : ControllerBase
    {
        private readonly ExperimentState _state;
        public ExperimentController(ExperimentState state)
        {
            _state = state;
        }

        [HttpPost("config")]
        [SwaggerOperation(Summary = "Set the gateway variant and failure rate for experiments")]
        public IActionResult SetConfig([FromBody] ExperimentConfigDto dto)
        {
            _state.Set(dto);
            return Ok();
        }
    }
}
