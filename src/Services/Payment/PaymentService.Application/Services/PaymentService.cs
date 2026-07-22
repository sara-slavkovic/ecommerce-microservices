using FluentValidation;
using PaymentService.Application.DTOs;
using PaymentService.Application.Interfaces;
using PaymentService.Domain.Entities;
using PaymentService.Domain.Enums;
using SharedKernel.Domain.Exceptions;
using System;
using System.Collections.Generic;
using System.Text;

namespace PaymentService.Application.Services
{
    public class PaymentService : IPaymentService
    {
        private readonly IPaymentRepository _paymentRepository;
        private readonly IMockGatewayClient _mockGatewayClient;
        private readonly IOrderServiceClient _orderServiceClient;
        private readonly IValidator<InitiatePaymentDto> _initiateValidator;

        public PaymentService(IPaymentRepository paymentRepository, IMockGatewayClient mockGatewayClient, IOrderServiceClient orderServiceClient, IValidator<InitiatePaymentDto> initiateValidator)
        {
            _paymentRepository = paymentRepository;
            _mockGatewayClient = mockGatewayClient;
            _orderServiceClient = orderServiceClient;
            _initiateValidator = initiateValidator;
        }

        public async Task<PaymentDto?> GetPaymentByIdAsync(Guid id)
        {
            var payment = await _paymentRepository.GetPaymentByIdAsync(id);
            if (payment == null) return null;
            return MapToDto(payment);
        }

        public async Task<PaymentDto?> GetPaymentByOrderIdAsync(Guid orderId)
        {
            var payment = await _paymentRepository.GetPaymentByOrderIdAsync(orderId);
            if (payment == null) return null;
            return MapToDto(payment);
        }

        public async Task<PaymentDto> InitiatePaymentAsync(InitiatePaymentDto dto)
        {
            var validationResult = await _initiateValidator.ValidateAsync(dto);
            if (!validationResult.IsValid)
                throw new ValidationException(validationResult.Errors);

            var existingPayment = await _paymentRepository.GetPaymentByOrderIdAsync(dto.OrderId);
            if (existingPayment != null)
                throw new ConflictException("Payment for this order already exists.");

            var order = await _orderServiceClient.GetOrderByIdAsync(dto.OrderId);
            if (order == null)
                throw new NotFoundException($"Order with ID {dto.OrderId} does not exist.");

            if (order.Status != "Pending")
                throw new ConflictException($"Cannot process payment. Order is currently in '{order.Status}' status. Only 'Pending' orders can be paid.");

            // one payment try
            var idempotencyKey = Guid.NewGuid().ToString();
            var startedAt = DateTime.Now;

            var chargeResult = await _mockGatewayClient.ChargeAsync(new ChargeRequestDto
            {
                OrderId = dto.OrderId,
                Amount = dto.Amount,
                IdempotencyKey = idempotencyKey,
                SimulationMode = "Random"
                //SimulationMode = "FailNTimesThenSucceed",
                //FailCount = 2
            });

            var finishedAt = DateTime.Now;

            var paymentId = Guid.NewGuid();
            var payment = new Payment
            {
                Id = paymentId,
                OrderId = dto.OrderId,
                Status = chargeResult.StatusCode == 200 ? PaymentStatus.Succeeded : PaymentStatus.Failed,
                Amount = dto.Amount,
                IdempotencyKey = idempotencyKey,
                CreatedAt = DateTime.Now,
                PaymentAttempts = new List<PaymentAttempt>()
            };

            var attempt = new PaymentAttempt
            {
                Id = Guid.NewGuid(),
                PaymentId = paymentId,
                AttemptNumber = 1,
                StatusCode = chargeResult.StatusCode,
                ErrorMessage = chargeResult.Success ? null : chargeResult.Message,
                StartedAt = startedAt,
                FinishedAt = finishedAt,
                DurationMs = (int)(finishedAt - startedAt).TotalMilliseconds
            };

            payment.PaymentAttempts.Add(attempt);

            await _paymentRepository.AddPaymentAsync(payment);
            await _paymentRepository.SaveChangesAsync();

            if (payment.Status == PaymentStatus.Succeeded)
                await _orderServiceClient.CompleteOrderAsync(payment.OrderId);
            else
                await _orderServiceClient.CancelOrderAsync(payment.OrderId);

            return MapToDto(payment);
        }

        private static PaymentDto MapToDto(Payment payment)
        {
            return new PaymentDto
            {
                Id = payment.Id,
                OrderId = payment.OrderId,
                Status = payment.Status.ToString(),
                Amount = payment.Amount,
                CreatedAt = payment.CreatedAt,
                PaymentAttempts = payment.PaymentAttempts
                    .OrderBy(pa => pa.AttemptNumber)
                    .Select(pa => new PaymentAttemptDto
                    {
                        Id = pa.Id,
                        AttemptNumber = pa.AttemptNumber,
                        StatusCode = pa.StatusCode,
                        ErrorMessage = pa.ErrorMessage ?? string.Empty,
                        StartedAt = pa.StartedAt,
                        FinishedAt = pa.FinishedAt,
                        DurationMs = pa.DurationMs
                    }).ToList()
            };
        }
    }
}
