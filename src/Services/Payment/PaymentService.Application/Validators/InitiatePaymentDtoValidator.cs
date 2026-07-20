using FluentValidation;
using PaymentService.Application.DTOs;
using System;
using System.Collections.Generic;
using System.Text;

namespace PaymentService.Application.Validators
{
    public class InitiatePaymentDtoValidator : AbstractValidator<InitiatePaymentDto>
    {
        public InitiatePaymentDtoValidator()
        {
            RuleFor(x => x.OrderId)
                .NotEqual(Guid.Empty).WithMessage("OrderId is required.");

            RuleFor(x => x.Amount)
                .GreaterThan(0).WithMessage("Amount must be greater than zero.");
        }
    }
}
