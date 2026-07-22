using Microsoft.AspNetCore.Mvc;
using PaymentService.Application.DTOs;
using PaymentService.Application.Interfaces;
using Swashbuckle.AspNetCore.Annotations;

namespace PaymentService.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PaymentsController : ControllerBase
    {
        private readonly IPaymentService _paymentService;

        public PaymentsController(IPaymentService paymentService)
        {
            _paymentService = paymentService;
        }

        [HttpGet("{id:guid}")]
        [SwaggerOperation(Summary = "Get payment by ID")]
        public async Task<IActionResult> GetPaymentById(Guid id)
        {
            var payment = await _paymentService.GetPaymentByIdAsync(id);
            if (payment == null) return NotFound();
            return Ok(payment);
        }

        [HttpGet("order/{orderId:guid}")]
        [SwaggerOperation(Summary = "Get payment by order ID")]
        public async Task<IActionResult> GetPaymentByOrderId(Guid orderId)
        {
            var payment = await _paymentService.GetPaymentByOrderIdAsync(orderId);
            if (payment == null) return NotFound();
            return Ok(payment);
        }

        [HttpPost]
        [SwaggerOperation(Summary = "Initiate payment for an order")]
        public async Task<IActionResult> InitiatePayment([FromBody] InitiatePaymentDto dto)
        {
            var payment = await _paymentService.InitiatePaymentAsync(dto);
            return CreatedAtAction(nameof(GetPaymentById), new { id = payment.Id }, payment);
        }
    }
}
